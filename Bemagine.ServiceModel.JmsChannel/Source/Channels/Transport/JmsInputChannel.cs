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
    using System.Threading;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The JmsInputChannel implements the receive message loop for handling JMS messages.
    /// </summary>
    /// <remarks>
    /// This functionality was not implemented in the channel listener to simplify the channel
    /// manager interactions. If the message receive loop were implemented in the listener then
    /// the factory would have to aggregate an instance of the listener. This would then require
    /// transport synchronization. Anyway, this listener in traditional TCP/IP can be considered
    /// the process that listens for incoming connections and then delagates communication handling
    /// to a separate component. In this case, the JmsInputChannel.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal sealed class JmsInputChannel : JmsChannelBase, IInputChannel
    {
        #region data members & member properties
        //----------------------------------------------------------------------------------------//
        // data members & member properties
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the address of the endpoint to which messages are sent.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public EndpointAddress LocalAddress 
        { 
            get { return QueueChannelManager.Transport.LocalAddress; }
        }

        private JmsTransport Transport
        {
            get { return QueueChannelManager.Transport; }
        }

        //----------------------------------------------------------------------------------------//
        // The input message queue.
        //----------------------------------------------------------------------------------------//

        private readonly InputQueue<Message> _messageQueue = new InputQueue<Message>();

        private WaitCallback _startReceivingCallback;
        private AsyncCallback _onReceiveCallback;

        //----------------------------------------------------------------------------------------//
        /// To improve transport responsiveness, by default incoming messages are dispatched to
        /// a thread other than the receiving thread. This allows for the receiving thread to
        /// process the next incoming message immediately. Given that that only one listener
        /// channel exists at a given time, it does not make sense to set this value to false
        /// in most circumstances. This is because operation dispatching occurs on the same thread
        /// the message was handled on.
        //----------------------------------------------------------------------------------------//

        private const bool DispatchMessageOnPooledThread = true;
        #endregion

        #region construction
        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public JmsInputChannel(ChannelManagerBase channelManager) : base(channelManager)
        {
        }
        #endregion

        #region message dispatch
        //----------------------------------------------------------------------------------------//
        // message dispatch
        //----------------------------------------------------------------------------------------//

        private void DispatchCallback(object state)
        {
            Dispatch((Message) state);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Hands the message off to other components higher up the channel stack that have 
        /// previously called BeginReceive() and are waiting for messages to arrive on this 
        /// channel. The Message is dispatch on a different pooled thread. This frees up the
        /// receive loop to process additional inbound messages.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        internal void Dispatch(Message message)
        {
            _messageQueue.EnqueueAndDispatch(message, null, !DispatchMessageOnPooledThread);
        }
        #endregion

        #region ChannelBase overrides
        //----------------------------------------------------------------------------------------//
        // ChannelBase overrides
        //----------------------------------------------------------------------------------------//

        protected override void OnOpening()
        {
            _onReceiveCallback = OnReceive;
            base.OnOpening();
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Starts asynchronous receive on our transport.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected override void OnOpened()
        {
            base.OnOpened();            
            _startReceivingCallback = StartReceiving;
            ThreadPool.QueueUserWorkItem(_startReceivingCallback, Transport);
        }

        protected override void OnAbort()
        {
            _messageQueue.Close();
            base.OnAbort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _messageQueue.Close();
            base.OnClose(timeout);
        }
        #endregion

        #region IInputChannel implementation
        //----------------------------------------------------------------------------------------//
        // IInputChannel implementation
        //----------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------//
        // synchonous receive methods
        //----------------------------------------------------------------------------------------//

        public Message Receive()
        {
            return Receive(DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout)
        {
            Message message;

            if (!TryReceive(timeout, out message))
            {
                throw ChannelUtility.CreateReceiveTimedOutException(this, timeout);
            }

            return message;
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            ChannelUtility.ValidateTimeout(timeout);
            return _messageQueue.Dequeue(timeout, out message);                       
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            ChannelUtility.ValidateTimeout(timeout);
            return _messageQueue.WaitForItem(timeout);
        }
        
        //----------------------------------------------------------------------------------------//
        // asynchonous receive methods
        //----------------------------------------------------------------------------------------//

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return BeginReceive(InfiniteTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return BeginTryReceive(timeout, callback, state);
        }
     
        public Message EndReceive(IAsyncResult result)
        {
            return _messageQueue.EndDequeue(result);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ChannelUtility.ValidateTimeout(timeout);
            return _messageQueue.BeginDequeue(timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
           return _messageQueue.EndDequeue(result, out message);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ChannelUtility.ValidateTimeout(timeout);
            return _messageQueue.BeginWaitForItem(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return _messageQueue.EndWaitForItem(result);
        }
        #endregion

        #region message listening framework
        //----------------------------------------------------------------------------------------//
        // message listening framework
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <remarks>
        /// Begins the asynchronous receive loop by calling into the JMS transport asynchronously,
        /// specifying OnReceive() as the AsyncCallback.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        private void StartReceiving(object state)
        {
            var transport = (JmsTransport) state;
            IAsyncResult result = null;

            try
            {
                lock (ThisLock)
                {
                    if (State == CommunicationState.Opened)
                    {
                        result = Transport.BeginReceive(_onReceiveCallback);
                    }
                }

                if (result != null && result.CompletedSynchronously)
                {
                    ContinueReceiving(result, transport);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in receiving from the JMS transport.");
                Debug.WriteLine(e.ToString());
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// After the message has been received by the underlying transport, this method completes
        /// the asynchronous receive operation and dispatches the received message for handling
        /// by upper channels in the stack.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void ContinueReceiving(IAsyncResult receiveResult, JmsTransport transport)
        {
            bool continueReceiving = true;

            while (continueReceiving)
            {
                Message receivedMessage = null;

                if (receiveResult != null)
                {
                    receivedMessage = EndReceive(transport, receiveResult);
                    receiveResult = null;
                }

                //--------------------------------------------------------------------------------//
                // If EndReceive() returns a null message the channel is no longer open. Subsequent
                // message processing is obviously pointless. Let's break!
                //--------------------------------------------------------------------------------//

                if (receivedMessage == null)
                    break;

                //--------------------------------------------------------------------------------//
                // The Receive Loop: The process for receiving is complicated by the fact that
                // it is entirely asynchronous. The loop therefore, is non-traditional, and is
                // as follows:
                //
                // 1. Only one Receive is initiated at a given time. Message processing 
                //    simply transforms JMS messages into WCF messages. Messages are then
                //    dispatched on a ThreadPool thread to the channels above.
                //
                // 2. Once a message is received and processed another receive operation is
                //    invoked.
                //--------------------------------------------------------------------------------//            

                lock (ThisLock)
                {
                    if (State == CommunicationState.Opened)
                    {
                        receiveResult = Transport.BeginReceive(_onReceiveCallback);
                    }
                }                

                if (receiveResult == null || !receiveResult.CompletedSynchronously)
                {
                    continueReceiving = false;
                    Dispatch(receivedMessage);
                }

                else
                {
                    ThreadPool.QueueUserWorkItem(DispatchCallback, receivedMessage);
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Completes the aysnchronous receive operation and queues the next iteration of the
        /// asynchronous receive operations.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private Message EndReceive(JmsTransport transport, IAsyncResult result)
        {
            lock (ThisLock)
            {
                //--------------------------------------------------------------------------------//
                // If we've started the shutdown process, then we've disposed the transport and
                // calls to EndReceive will throw. So, we return null.
                //--------------------------------------------------------------------------------//

                if (State != CommunicationState.Opened)
                {
                    return null;
                }                
                return transport.EndReceive(result);
            }
        }

        //----------------------------------------------------------------------------------------//
        // Called when an ansynchronous receieve operation completes on the listening channel.
        //----------------------------------------------------------------------------------------//

        void OnReceive(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

            ContinueReceiving(result, ((JmsTransport)result.AsyncState));
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
