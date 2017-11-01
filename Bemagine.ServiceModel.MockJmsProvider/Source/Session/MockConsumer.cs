//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.MockJmsProvider.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.MockJmsProvider
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using Bemagine.ServiceModel.Jms;

    using System;
    using System.Threading;
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Mock implementation of a JMS Consumer that implements the IMessageConsumer interface.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class MockConsumer : IMessageConsumer
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private bool _closed;
        private readonly object _thisLock = new object();
        private readonly Queue<IMessage> _messageQueue = new Queue<IMessage>();
        private readonly AutoResetEvent _messageEvent = new AutoResetEvent(false);

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        internal MockConsumer(IDestination destination)
        {
            MessageDispatcher.Instance.RegisterConsumer(destination, OnMessage);
        } 
  
        internal MockConsumer(IDestination destination, string messageSelector)
        {
            MessageSelector = messageSelector;
            MessageDispatcher.Instance.RegisterConsumer(destination, OnMessage);
        }

        //----------------------------------------------------------------------------------------//
        // OnMessage dispatch action
        //----------------------------------------------------------------------------------------//
    
        private void OnMessage(IMessage message)
        {
            if (MessageListener != null)
            {
                MessageListener.OnMessage(message);  
            }
            else
            {
                lock (_thisLock)
                {
                    _messageQueue.Enqueue(message);
                }
                _messageEvent.Set();
            }
        }

        #region IMessageConsumer interface implementation
        //----------------------------------------------------------------------------------------//
        // IMessageConsumer interface implementation
        //----------------------------------------------------------------------------------------//

        public string MessageSelector { get; private set; }
        public IMessageListener MessageListener { get; set; }

        public void Close()
        {
             lock (_thisLock)
             {
                _closed = true;
             }
            _messageEvent.Set();
        }

        public IMessage Receive()
        {
            IMessage message = null;
            _messageEvent.WaitOne();            

            if (!_closed)
            {
                lock (_thisLock)
                {
                    if (!_closed)
                    {
                        message = _messageQueue.Dequeue();
                    }
                }
            }            

            return message;
        }

        public IMessage Receive(long timeout)
        {
            IMessage message = null;

            Func<bool> waitAction = 
                (timeout == 0) ?
                    (Func<bool>) (() => _messageEvent.WaitOne()) :
                    (() => _messageEvent.WaitOne((int) timeout));

            if (waitAction() && !_closed)
            {
                lock (_thisLock)
                {
                    if (!_closed)
                    {
                        message = _messageQueue.Dequeue();
                    }
                }
            }            

            return message;            
        }

        public IMessage ReceiveNoWait()
        {
            IMessage message = null;

            if (!_closed)
            {
                lock (_thisLock)
                {
                    if (!_closed)
                    {
                        try { message = _messageQueue.Dequeue(); }
                        catch { }
                    }
                }
            }            

            return message;
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
