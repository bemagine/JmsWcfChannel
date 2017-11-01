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
    /// Exposes the TIBCO EMS MessageProducer implementation as an IMessageProducer.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    internal sealed class EmsProducer : IMessageProducer
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
        
        private readonly TIBCO.EMS.MessageProducer _innerProducer;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        internal EmsProducer(TIBCO.EMS.MessageProducer innerProducer)
        {
            _innerProducer = innerProducer;
        }

        #region IMessageProducer interface implementation
        //----------------------------------------------------------------------------------------//
        // IMessageProducer interface implementation
        //----------------------------------------------------------------------------------------//

        public void Close()
        {
            SafeProxy.SafeCall( () => _innerProducer.Close() );
        }

        public MessagePriority Priority
        {
            get { return (MessagePriority) _innerProducer.Priority; }
            set { _innerProducer.Priority = (int) value; }
        }

        public DeliveryMode DeliveryMode
        {
            get
            {
                return (_innerProducer.DeliveryMode == TIBCO.EMS.DeliveryMode.NON_PERSISTENT) ?
                    DeliveryMode.NonPersistent : DeliveryMode.Persistent;
            }
            set
            {
                _innerProducer.DeliveryMode =
                    (value == DeliveryMode.NonPersistent) ?
                        TIBCO.EMS.DeliveryMode.NON_PERSISTENT :
                        TIBCO.EMS.DeliveryMode.PERSISTENT;
            }
        }

        public long TimeToLive
        {
            get { return _innerProducer.TimeToLive; }
            set { _innerProducer.TimeToLive = value; }
        }

        public IDestination Destination
        {
            get 
            {  
                return SafeProxy.SafeCall(
                    () => { return new EmsDestination(_innerProducer.Destination); }
                );
            }
        }

        public bool DisableMessageID
        {
            get { return _innerProducer.DisableMessageID; }
            set { _innerProducer.DisableMessageID = value; }
        }

        public bool DisableMessageTimestamp
        {
            get { return _innerProducer.DisableMessageTimestamp; }
            set { _innerProducer.DisableMessageTimestamp = value; }
        }

        public void Send(IMessage message)
        {
            SafeProxy.SafeCall( () => _innerProducer.Send((EmsMessage) message) );
        }

        public void Send(IDestination destination, IMessage message)
        {
            SafeProxy.SafeCall(
                () => 
                {
                    _innerProducer.Send((EmsDestination) destination, (EmsMessage) message);
                });
        }

        public void Send(IDestination destination, IMessage message, DeliveryMode deliveryMode, 
            MessagePriority priority, long timeToLive)
        {
            SafeProxy.SafeCall(
                () => 
                {
                    _innerProducer.Send(
                        (EmsDestination) destination, 
                        (EmsMessage) message,
                        (deliveryMode == Jms.DeliveryMode.Persistent) ? 
                            TIBCO.EMS.MessageDeliveryMode.Persistent : 
                            TIBCO.EMS.MessageDeliveryMode.NonPersistent,
                        (int) priority,
                        timeToLive);
                });
        }

        public void Send(IMessage message, DeliveryMode deliveryMode, MessagePriority priority, 
            long timeToLive)
        {
            SafeProxy.SafeCall(
                () => 
                {
                    _innerProducer.Send(
                        (EmsMessage) message,
                        (deliveryMode == Jms.DeliveryMode.Persistent) ? 
                            TIBCO.EMS.MessageDeliveryMode.Persistent : 
                            TIBCO.EMS.MessageDeliveryMode.NonPersistent,
                        (int) priority,
                        timeToLive);
                });
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
