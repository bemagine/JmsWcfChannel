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

    using System.Collections;
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Exposes the TIBCO EMS Message implementation as an IMessage.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public class EmsMessage : IMessage
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
        
        private readonly TIBCO.EMS.Message _innerMessage;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        internal EmsMessage(TIBCO.EMS.Message innerMessage)
        {
            _innerMessage = innerMessage;
        }

        //----------------------------------------------------------------------------------------//
        // implicit conversion operator
        //----------------------------------------------------------------------------------------//

        public static implicit operator TIBCO.EMS.Message(EmsMessage emsMessage)
        {
            return emsMessage._innerMessage;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Converts the message to a the interfaced message type specified by T. Return null
        /// if the conversion fails.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public T Convert<T>() where T: class
        {
            T iMessage = null;

            if (typeof(T) == typeof(IBytesMessage))
            {
                iMessage = new EmsBytesMessage((TIBCO.EMS.BytesMessage) _innerMessage) as T;
            }

            return iMessage;
        }

        #region IMessage interface implementation
        //----------------------------------------------------------------------------------------//
        // IMessage interface implementation
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        // properties
        //----------------------------------------------------------------------------------------//
        
        public string MessageID
        {
            get { return _innerMessage.MessageID; }
            set { _innerMessage.MessageID = value; }
        }

        public string CorrelationID
        {
            get { return _innerMessage.CorrelationID; }
            set { _innerMessage.CorrelationID = value; }
        }

        public byte[] CorrelationIDAsBytes
        {
            get { return _innerMessage.CorrelationIDAsBytes; }
            set { _innerMessage.CorrelationIDAsBytes = value; }
        }

        public MessagePriority Priority
        {
            get { return (MessagePriority) _innerMessage.Priority; }
            set { _innerMessage.Priority = (int) value; }
        }

        public DeliveryMode DeliveryMode
        {
            get
            {
                return (_innerMessage.DeliveryMode == TIBCO.EMS.DeliveryMode.NON_PERSISTENT) ?
                    DeliveryMode.NonPersistent : DeliveryMode.Persistent;
            }
            set
            {
                _innerMessage.DeliveryMode =
                    (value == DeliveryMode.NonPersistent) ?
                        TIBCO.EMS.DeliveryMode.NON_PERSISTENT :
                        TIBCO.EMS.DeliveryMode.PERSISTENT;
            }
        }

        public IDestination Destination
        {
            get { return new EmsDestination(_innerMessage.Destination); }
            set
            {
                SafeProxy.SafeCall( () => _innerMessage.Destination = value as EmsDestination );
            }
        }

        public IDestination ReplyTo
        {
            get { return new EmsDestination(_innerMessage.ReplyTo); }
            set
            {
                SafeProxy.SafeCall( () => _innerMessage.ReplyTo = value as EmsDestination );
            }
        }

        public long Expiration
        {
            get { return _innerMessage.Expiration; }
            set { _innerMessage.Expiration = value; }
        }

        public bool Redelivered
        {
            get { return _innerMessage.Redelivered; }
            set { _innerMessage.Redelivered = value; }
        }

        public long Timestamp
        {
            get { return _innerMessage.Timestamp; }
            set { _innerMessage.Timestamp = value; }
        }

        public string JMSType
        {
            get { return _innerMessage.MsgType; }
            set { _innerMessage.MsgType = value; }
        }

        //----------------------------------------------------------------------------------------//
        // methods
        //----------------------------------------------------------------------------------------//

        public void Acknowledge()
        {
            SafeProxy.SafeCall( () => _innerMessage.Acknowledge() );
        }

        public void ClearBody()
        {
            SafeProxy.SafeCall( () => _innerMessage.ClearBody() );
        }

        public void ClearProperties()
        {
            SafeProxy.SafeCall( () => _innerMessage.ClearProperties() );
        }

        public string[] GetPropertyNames()
        {
            return SafeProxy.SafeCall( 
                () => 
                {
                    List<string> propertyNames = new List<string>();
                    foreach (var propertyName in (IEnumerable) _innerMessage.PropertyNames)
                    {
                       propertyNames.Add((string) propertyName);
                    }

                    return propertyNames.ToArray();
                } 
            );
        }

        public bool PropertyExists(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.PropertyExists(name); } );
        }

        public bool GetBooleanProperty(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.GetBooleanProperty(name); } );
        }

        public byte GetByteProperty(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.GetByteProperty(name); } );
        }

        public double GetDoubleProperty(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.GetDoubleProperty(name); } );
        }

        public float GetFloatProperty(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.GetFloatProperty(name); } );
        }

        public int GetIntProperty(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.GetIntProperty(name); } );
        }

        public long GetLongProperty(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.GetLongProperty(name); } );
        }

        public object GetObjectProperty(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.GetObjectProperty(name); } );
        }        

        public short GetShortProperty(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.GetShortProperty(name); } );
        }

        public string GetStringProperty(string name)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.GetStringProperty(name); } );
        }

        public void SetBooleanProperty(string name, bool value)
        {
            SafeProxy.SafeCall( () => _innerMessage.SetBooleanProperty(name, value) );
        }

        public void SetByteProperty(string name, byte value)
        {
            SafeProxy.SafeCall( () => _innerMessage.SetByteProperty(name, value) );
        }

        public void SetDoubleProperty(string name, double value)
        {
            SafeProxy.SafeCall( () => _innerMessage.SetDoubleProperty(name, value) );
        }

        public void SetFloatProperty(string name, float value)
        {
            SafeProxy.SafeCall( () => _innerMessage.SetFloatProperty(name, value) );
        }

        public void SetIntProperty(string name, int value)
        {
            SafeProxy.SafeCall( () => _innerMessage.SetIntProperty(name, value) );
        }

        public void SetLongProperty(string name, long value)
        {
            SafeProxy.SafeCall( () => _innerMessage.SetLongProperty(name, value) );
        }

        public void SetObjectProperty(string name, object value)
        {
            SafeProxy.SafeCall( () => _innerMessage.SetObjectProperty(name, value) );
        }

        public void SetShortProperty(string name, short value)
        {
            SafeProxy.SafeCall( () => _innerMessage.SetShortProperty(name, value) );
        }

        public void SetStringProperty(string name, string value)
        {
            SafeProxy.SafeCall( () => _innerMessage.SetStringProperty(name, value) );
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
