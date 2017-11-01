//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Channels
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Xml;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class ReliableRequestChannel 
        : ReliableRequestChannelBase<IDuplexChannel>, IDuplexChannel
    {
        #region data members
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly bool _isClient;
        private readonly Action<Message, TimeSpan> _sendDelegate;

        private const int HeartbeatPeriodicity = 8000;
        private readonly IHeartbeatManager _heartbeatManager;
        #endregion

        #region construction
        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public ReliableRequestChannel(ChannelManagerBase channelManagerBase, 
            IDuplexChannel innerChannel, bool isClient)
                : base(channelManagerBase, innerChannel)
        {
            _isClient = isClient;
            _sendDelegate += SendReliableRequest;

            var transportProperties = 
                channelManagerBase.GetProperty<JmsTransportContext>();

            Debug.Assert(
                transportProperties != null,
                "Failed to get the JmsTransportContext via the channelManagerBase.GetProperty() "+
                "method. There's a bug that requires immediate attention!");

            _heartbeatManager = 
                new HeartbeatManager(
                    HeartbeatPeriodicity, 
                    transportProperties,
                    ServiceUri, 
                    isClient);
        }
        #endregion

        #region member properties
        //----------------------------------------------------------------------------------------//
        // member properties
        //----------------------------------------------------------------------------------------//

        public EndpointAddress LocalAddress 
        {
            get { return InnerChannel.LocalAddress; }
        }

        public EndpointAddress RemoteAddress 
        {
            get { return InnerChannel.RemoteAddress; }
        }

        public Uri Via 
        {
            get { return InnerChannel.Via; }
        }

        private Uri ServiceUri
        {
            get
            {
                if (_isClient) return Via;
                return (LocalAddress != null) ? LocalAddress.Uri : null;
            }
        }
        #endregion

        #region utility methods
        //----------------------------------------------------------------------------------------//
        // utility methods
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        // AddReliableRequestHeader()
        //----------------------------------------------------------------------------------------//

        private static void AddReliableRequestHeader(Message message, 
            ReliableRequestMessageType protocol)
        {
            var header = new ReliableRequestHeader
            {
                MessageType = protocol,
                ServiceName = ReliableRequestProperties.ServiceHost,
                ServiceSession = ReliableRequestProperties.ServiceInstance
            };

            message.AddHeader(header);          
        }

        //----------------------------------------------------------------------------------------//
        // IsReliableRequest()
        //----------------------------------------------------------------------------------------//

        private static bool IsReliableRequest(Message message)
        {
            ReliableRequestHeader header;
            if (message.TryGetHeader(out header))
            {
                return (header.MessageType == ReliableRequestMessageType.Request);
            }

            return false;
        }

        //----------------------------------------------------------------------------------------//
        // SendReliableRequest()
        //----------------------------------------------------------------------------------------//

        private void SendReliableRequest(Message message, TimeSpan timeout)
        {   
            if (message != null)
            {     
                //--------------------------------------------------------------------------------//
                // If in the client context send operations are assumed to be requests that require
                // some response, whether synchronous or asynchronous. Thus, we generate a new 
                // reliable request and start the state machine.
                //--------------------------------------------------------------------------------//

                if (_isClient)
                {
                    AddReliableRequestHeader(message, ReliableRequestMessageType.Request);
                    message.Headers.MessageId = new UniqueId(Guid.NewGuid());

                    var request = new ReliableRequest(message, 2, InnerChannel);
                    ReliableRequest.Send(request);

                    LogUtility.Debug("[CorrelationId : {0}] Sent request [Client]", 
                        request.CorrelationId);
                }
                
                //--------------------------------------------------------------------------------//
                // Send operations from the server context indicate a response to some request.
                //--------------------------------------------------------------------------------//

                else
                {
                    AddReliableRequestHeader(message, ReliableRequestMessageType.Response);

                    using (new OperationContextScope(OperationContext.Current))
                    {
                        if (OperationContext.Current != null)
                        {
                            MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
                            message.Headers.MessageId = headers.MessageId;

                            LogUtility.Debug("[CorrelationId : {0}] Sent response [Service]", 
                                headers.MessageId);
                        }
                    }

                    InnerChannel.Send(message, timeout);
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        // ReceiveReliableRequest()
        //----------------------------------------------------------------------------------------//

        private bool ReceiveReliableRequest(Message message)
        {
            bool continueReceiving = false;

            if (message != null)
            {
                //--------------------------------------------------------------------------------//
                // In the client context there are two possible message types that are regarded
                // for handling, request acknowlegements and request responses.
                //--------------------------------------------------------------------------------//

                if (_isClient)
                {
                    ReliableRequestHeader requestHeader;
                    if (message.TryGetHeader(out requestHeader))
                    {
                        if (requestHeader.MessageType == ReliableRequestMessageType.Acknowledgement)
                        {
                            string correlationId = message.Headers.MessageId.ToString();

                            LogUtility.Debug(
                                "[CorrelationId : {0}] Received acknowledgement [Client]", 
                                correlationId);

                            ReliableRequestManager.Instance.Acknowledged(
                                requestHeader.ServiceName,
                                requestHeader.ServiceSession,
                                correlationId);

                            continueReceiving = true;
                        }
                        else if (requestHeader.MessageType == ReliableRequestMessageType.Response)
                        {
                            string correlationId = message.Headers.MessageId.ToString();

                            LogUtility.Debug("[CorrelationId : {0}] Received response [Client]", 
                                correlationId);

                            ReliableRequestManager.Instance.Dispatched(correlationId);
                        }
                    }
                }

                //--------------------------------------------------------------------------------//
                // In the service context, only request messages are expected. In response to the
                // receipt of request message an acknowlegement message is sent to the requestor
                // as part of the reliable request protocol.
                //--------------------------------------------------------------------------------//
                else
                {
                    if (IsReliableRequest(message) && (message.Headers.ReplyTo != null))
                    {
                        Message acknowledgement = 
                            Message.CreateMessage(message.Version, message.Headers.Action);

                        acknowledgement.Headers.To = message.Headers.ReplyTo.Uri;

                        UniqueId messageId = message.Headers.MessageId;
                        acknowledgement.Headers.MessageId = messageId;

                        AddReliableRequestHeader(
                            acknowledgement, 
                            ReliableRequestMessageType.Acknowledgement);

                        acknowledgement.AddHeader(Jms.MessagePriority.Highest);

                        LogUtility.Debug("[CorrelationId : {0}] Received request [Service]", 
                            messageId);

                        InnerChannel.Send(acknowledgement);

                        LogUtility.Debug("[CorrelationId : {0}] Sent acknowledgement [Service]", 
                            messageId);
                    }
                }
            }

            return continueReceiving;
        }
        #endregion

        #region IInputChannel implementation
        //----------------------------------------------------------------------------------------//
        // IInputChannel implementation
        //----------------------------------------------------------------------------------------//

        private AsyncCallback _receiveCallback;

        public Message Receive() 
        {
            return Receive(DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout) 
        {
            Message message = InnerChannel.Receive(timeout);
            
            while (ReceiveReliableRequest(message))
            {
                message = InnerChannel.Receive(timeout);
            }            

            return message;
        }        

        public IAsyncResult BeginReceive(AsyncCallback callback, object state) 
        {
            return BeginReceive(DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state) 
        {
            return InnerChannel.BeginReceive(timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result) 
        {
            Message message = InnerChannel.EndReceive(result);
            while (ReceiveReliableRequest(message))
            {
                IAsyncResult innerResult = 
                    InnerChannel.BeginTryReceive(DefaultReceiveTimeout, _receiveCallback, null);

                message = InnerChannel.EndReceive(innerResult);
            }
            return message;
        }

        public bool TryReceive(TimeSpan timeout, out Message message) 
        {
            return InnerChannel.TryReceive(timeout, out message);
        }        

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, 
            object state) 
        {
            _receiveCallback = callback;
            return InnerChannel.BeginTryReceive(timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message) 
        {
            InnerChannel.EndTryReceive(result, out message); 
            return !ReceiveReliableRequest(message);
        }

        public bool WaitForMessage(TimeSpan timeout) 
        {
            return InnerChannel.WaitForMessage(timeout);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback,
            object state) 
        {
            return InnerChannel.BeginWaitForMessage(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result) 
        {
            return InnerChannel.EndWaitForMessage(result);
        }        
        #endregion

        #region IOuptutChannel implementation
        //----------------------------------------------------------------------------------------//
        // IOuptutChannel implementation
        //----------------------------------------------------------------------------------------//

        public void Send(Message message) 
        {
            Send(message, DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout) 
        {
            SendReliableRequest(message, timeout);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state) 
        {
            return BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, 
            object state) 
        {
            return _sendDelegate.BeginInvoke(message, timeout, callback, state);
        }        

        public void EndSend(IAsyncResult result) 
        {
            _sendDelegate.EndInvoke(result);
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
