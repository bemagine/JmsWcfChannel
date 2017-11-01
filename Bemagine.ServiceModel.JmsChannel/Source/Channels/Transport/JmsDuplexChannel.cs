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
    /// JmsDuplexChannel encapsulates an JMS queue input and output channels, and proxies
    /// calls to the appropriate composed channel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Clearly, TIBCO provides a WCF solution for binding to JMS services, but lacks the ability
    /// to reply to client specific, sessionless duplex endpoints. When an a sessionless duplex 
    /// endpoint is configured for asynchronous operations, the TIBCO EMS WCF channel appends 
    /// ".callback" to the incoming JMS queue name. This means that any client producing message
    /// to some queue may consume any response, regardless if it were in response to that client's
    /// request (or any request at all). 
    /// </para>
    /// <para>
    /// Sessionizing the communication channel does elicit the expected behavior for correlated
    /// client / service interactions. This naturally occurs as an affect of sessionization. At
    /// its core sessions provide message correlation. Sessionization is generally reserved for 
    /// those situations where state is maintained on the service instance. This means that the
    /// clients sessions must be bound to a particular service instance. As a side effect, load
    /// on the system may become unbalanced. 
    /// </para>
    /// <para>
    /// Consider the scenario where requests made of a particular service (perhaps a mortgage
    /// cashflow generation engine) vary in complexity and time. It is clear that an asynchornous
    /// message exchange pattern is required in this situation. Given that the such operations
    /// vary in complexity and time, sessionization would lead imbalance of load across the 
    /// system.
    /// </para>
    /// <para>
    /// It is possible (I believe) to configure sessionless duplex channels to route messages 
    /// accordingly using filters and other JMS facilities, but this adds unnessary environment 
    /// complexity. This complexity ultimately leads to a higher maintanence cost. I think that
    /// the requirement for sessionless duplex client specific routing is so primal that it 
    /// warrants implementation as a WCF channel. The behavior here should not require significant
    /// effort on the implementor's part to get working. 
    /// </para>
    /// <para>
    /// The duplex channel implementation provided supports client specific JMS callback queues. 
    /// Additionally, this duplex channel supports a singular callback queue for the scenarios
    /// where responses can be processed by any consumer. In both these cases, service endpoint 
    /// throttling can be configured to pause draining of incoming queues. Thus, allowing for free
    /// resources to processes messages.
    /// </para>
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal sealed class JmsDuplexChannel : JmsChannelBase, IDuplexChannel
    {
        //----------------------------------------------------------------------------------------//
        // data members and member properties
        //----------------------------------------------------------------------------------------//

        private readonly JmsInputChannel _inputChannel;
        private readonly JmsOutputChannel _outputChannel;

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the address on which the input channel listens for and receives messages.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public EndpointAddress LocalAddress 
        {
            get { return _inputChannel.LocalAddress; }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the address of the endpoint to which messages are sent.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public EndpointAddress RemoteAddress 
        {
            get { return _outputChannel.RemoteAddress; }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the URI that contains the transport address to which messages are sent.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public Uri Via 
        {
            get { return _outputChannel.Via; }
        }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public JmsDuplexChannel(
            ChannelManagerBase channelManager,
            EndpointAddress remoteAddress,
            Uri via)
                : base(channelManager)
        {
            _outputChannel = new JmsOutputChannel(channelManager, remoteAddress, via);
            _inputChannel = new JmsInputChannel(channelManager);
        }

        #region CommunicationObject
        //----------------------------------------------------------------------------------------//
        // CommunicationObject overrides
        //----------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------//
        // channel open methods
        //----------------------------------------------------------------------------------------//

        protected override void OnOpen(TimeSpan timeout)
        {
            _outputChannel.Open();
            _inputChannel.Open();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, 
            Object state)
        {
            OnOpen(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        //----------------------------------------------------------------------------------------//
        // channel close / abort methods
        //----------------------------------------------------------------------------------------//

        protected override void OnAbort()
        {
            _outputChannel.Abort();
            _inputChannel.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _outputChannel.Close();
            _inputChannel.Close();
        }
        #endregion

        #region IInputChannel implementation
        //----------------------------------------------------------------------------------------//
        // IInputChannel implementation
        //----------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------//
        // synchronous receive methods
        //----------------------------------------------------------------------------------------//

        public Message Receive() 
        {
            return _inputChannel.Receive();
        }

        public Message Receive(TimeSpan timeout) 
        {
            return _inputChannel.Receive(timeout);
        }        

        public bool TryReceive(TimeSpan timeout, out Message message) 
        {
            bool messageReceived = _inputChannel.TryReceive(timeout, out message);
            return messageReceived;
        }

        public bool WaitForMessage(TimeSpan timeout) 
        {
            return _inputChannel.WaitForMessage(timeout);
        }
        
        //----------------------------------------------------------------------------------------//
        // asynchronous receive methods
        //----------------------------------------------------------------------------------------//

        public IAsyncResult BeginReceive(AsyncCallback callback, object state) 
        {
            return _inputChannel.BeginReceive(DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state) 
        {
            return _inputChannel.BeginReceive(timeout, callback, state);
        }        

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, 
            object state) 
        {
            return _inputChannel.BeginTryReceive(timeout, callback, state);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback,
            object state) 
        {
            return _inputChannel.BeginWaitForMessage(timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result) 
        {
            return _inputChannel.EndReceive(result);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message) 
        {
            return _inputChannel.EndTryReceive(result, out message);
        }

        public bool EndWaitForMessage(IAsyncResult result) 
        {
            return _inputChannel.EndWaitForMessage(result);
        }
        #endregion

        #region IOuptutChannel implementation
        //----------------------------------------------------------------------------------------//
        // IOuptutChannel implementation
        //----------------------------------------------------------------------------------------//

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, 
            object state) 
        {
            return _outputChannel.BeginSend(message, timeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state) 
        {
            return BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public void EndSend(IAsyncResult result) 
        {
            _outputChannel.EndSend(result);
        }

        public void Send(Message message, TimeSpan timeout) 
        {
            _outputChannel.Send(message, timeout);
        }

        public void Send(Message message) 
        {
            Send(message, DefaultSendTimeout);
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
