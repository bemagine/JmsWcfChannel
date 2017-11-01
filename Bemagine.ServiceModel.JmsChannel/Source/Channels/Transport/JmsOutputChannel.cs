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
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The JmsOutputChannel implements the behaviors for sending messages via an JMS queue
    /// transport.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class JmsOutputChannel : JmsChannelBase, IOutputChannel
    {
    //--------------------------------------------------------------------------------------------//
    // data members and member properties
    //--------------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the address of the endpoint to which messages are sent.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public EndpointAddress RemoteAddress { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the URI that contains the transport address to which messages are sent.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public Uri Via { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the local address associated with the JmsTransport. This is used in the 
        /// client context to set the replyTo message header of the outgoing message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private EndpointAddress LocalAddress 
        {
            get { return QueueChannelManager.Transport.LocalAddress; }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The SendDelegate provides a mechanism for supporting aysnchronous send operations.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private delegate void SendDelegate(Message message, TimeSpan timeout);
        private readonly SendDelegate _sendDelegate;

    //--------------------------------------------------------------------------------------------//
    // construction
    //--------------------------------------------------------------------------------------------//

        public JmsOutputChannel(
            ChannelManagerBase channelManager, 
            EndpointAddress remoteAddress, 
            Uri via) 
                : base(channelManager)
        {
            RemoteAddress = remoteAddress;
            Via = via;

            _sendDelegate += Send;
        }

    //--------------------------------------------------------------------------------------------//
    // IOutputChannel members
    //--------------------------------------------------------------------------------------------//
    
        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

        //----------------------------------------------------------------------------------------//
        /// <remarks>
        /// Given the context of the send operation, whether a client request or service response, 
        /// the appropriate addressing headers are set (if not already set by channels higher up
        /// in the channel stack).
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        public void Send(Message message, TimeSpan timeout)
        {
            ThrowIfDisposedOrNotOpen();
            ChannelUtility.ValidateTimeout(timeout);
            
            //------------------------------------------------------------------------------------//
            // If the send is initiated in the client context the To and ReplyTo headers are set 
            // to the Via and LocalAddress addresses respectively.
            //------------------------------------------------------------------------------------//

            if (IsClient)
            {    
                if (message.Headers.To == null)
                    message.Headers.To = Via;

                if (message.Headers.ReplyTo == null)
                    message.Headers.ReplyTo = LocalAddress;
                
                if (message.Headers.MessageId == null)
                    message.Headers.MessageId = new System.Xml.UniqueId();
            }

            //------------------------------------------------------------------------------------//
            // If the send is initiated in the service context the message is in response to some
            // request. Therefore, we obtain the OperationContext in which the request was made,
            // extract the ReplyTo URI from the incoming message headers, and set the outgoing To
            // message header to the ReplyTo URI.
            //
            // There are a number of considerations to the approach taken here. The first thing
            // to consider is what the channel is attempting to solve. Namely, a single channel
            // exists at any given time that has to route response back to an individual caller.
            // In this context, the channel cannot know or track request responses and must rely
            // on some additional context for information. In the case of responses from server
            // channels the reply is extracted from the current calling context (OperationContext).
            // This assumes that the service implements an operation that performs some work and
            // with in the same calling context provides an response (aysnc or sync). The channel
            // can then extract the reply to URI from the request message. 
            //
            // Now consider an operation that is async that invokes another async operation and 
            // waits for a response. Upon arrival of that response it may callback to the initial
            // caller in a different thread than the initial call occurred. The original request
            // message has been disposed. Therefore, an OperationContext extension is required to
            // maintain the context of the original call. AsyncEmsOperationContext provides that
            // mechanism. 
            //------------------------------------------------------------------------------------//

            else
            {
                if (AsyncOperationContext.Current == null)
                {
                    using (new OperationContextScope(OperationContext.Current))
                    {
                        MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;

                        if ((headers != null) && 
                            (headers.ReplyTo != null) && 
                            (message.Headers.To == null))
                        {
                            message.Headers.To = headers.ReplyTo.Uri;
                        }
                    }
                }
                else
                {
                    message.Headers.To = AsyncOperationContext.Current.ReplyToUri;
                }
            }

            QueueChannelManager.Transport.Send(message, timeout);
        }        

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, 
            object state)
        {
            return _sendDelegate.BeginInvoke(message, timeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return _sendDelegate.BeginInvoke(message, DefaultSendTimeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            _sendDelegate.EndInvoke(result);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
