//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.EmsJmsWrapper.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.EmsJmsWrapper
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using Bemagine.ServiceModel.Jms;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Exposes the TIBCO EMS MessageConsumer implementation as an IMessageConsumer.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    internal sealed class EmsConsumer : IMessageConsumer, TIBCO.EMS.IMessageListener
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
        
        private readonly TIBCO.EMS.MessageConsumer _innerConsumer;
        private Jms.IMessageListener _innerListener;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        internal EmsConsumer(TIBCO.EMS.MessageConsumer innerConsumer)
        {
            _innerConsumer = innerConsumer;
        }
        
        #region IMessageConsumer interface implementation
        //----------------------------------------------------------------------------------------//
        // IMessageConsumer interface implementation
        //----------------------------------------------------------------------------------------//

        public void Close()
        {
            SafeProxy.SafeCall( () => _innerConsumer.Close() );
        }

        public IMessage Receive()
        {
            return SafeProxy.SafeCall(
                () => 
                { 
                    TIBCO.EMS.Message message = _innerConsumer.Receive();
                    return (message != null ) ? new EmsMessage(message) : null; 
                }
            );
        }

        public IMessage Receive(long timeout)
        {
            return SafeProxy.SafeCall(
                () => 
                { 
                    TIBCO.EMS.Message message = _innerConsumer.Receive(timeout);
                    return (message != null ) ? new EmsMessage(message) : null; 
                }
            );
        }

        public IMessage ReceiveNoWait()
        {
            return SafeProxy.SafeCall(
                () => 
                { 
                    TIBCO.EMS.Message message = _innerConsumer.ReceiveNoWait();
                    return (message != null ) ? new EmsMessage(message) : null; 
                }
            );
        }

        public string MessageSelector
        {
            get { return _innerConsumer.MessageSelector; }
        }

        public IMessageListener MessageListener 
        { 
            get
            {
                return _innerListener;
            }
            set
            {
                _innerListener = value;
                _innerConsumer.MessageListener = this;
            }
        }
        #endregion

        #region IMessageListener interface implementation
        //----------------------------------------------------------------------------------------//
        // IMessageListener interface implementation
        //----------------------------------------------------------------------------------------//

        public void OnMessage(TIBCO.EMS.Message message)
        {
            SafeProxy.SafeCall(
                () => _innerListener.OnMessage(new EmsMessage(message)));
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
