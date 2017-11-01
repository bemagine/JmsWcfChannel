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

    using System.Linq;
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Mock implementation of a JMS message that implements the IMessage interface.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal class MockMessage : IMessage
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Converts the message to a the interfaced message type specified by T. Return null
        /// if the conversion fails.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public T Convert<T>() where T : class
        {
            return this as T;
        }

        #region IMessage interface implementation
        //----------------------------------------------------------------------------------------//
        // IMessage interface implementation
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        // properties
        //----------------------------------------------------------------------------------------//

        public string MessageID { get; set; }
        public string CorrelationID { get; set; }
        public byte[] CorrelationIDAsBytes { get; set; }
        public MessagePriority Priority { get; set; }
        public DeliveryMode DeliveryMode { get; set; }
        public IDestination Destination { get; set; }
        public IDestination ReplyTo { get; set; }
        public long Expiration { get; set; }
        public bool Redelivered { get; set; }
        public long Timestamp { get; set; }
        public string JMSType { get; set; }

        //----------------------------------------------------------------------------------------//
        // methods
        //----------------------------------------------------------------------------------------//

        public void Acknowledge()
        {
        }

        public virtual void ClearBody()
        {
        }

        public void ClearProperties()
        {
            _properties.Clear();
        }

        public string[] GetPropertyNames()
        {
            return _properties.Keys.ToArray();
        }

        public bool PropertyExists(string name)
        {
            return _properties.ContainsKey(name);
        }

        public bool GetBooleanProperty(string name)
        {
            return _properties.FindAs<bool>(name);
        }

        public byte GetByteProperty(string name)
        {
            return _properties.FindAs<byte>(name);
        }

        public double GetDoubleProperty(string name)
        {
            return _properties.FindAs<double>(name);
        }

        public float GetFloatProperty(string name)
        {
            return _properties.FindAs<float>(name);
        }

        public int GetIntProperty(string name)
        {
            return _properties.FindAs<int>(name);
        }

        public long GetLongProperty(string name)
        {
            return _properties.FindAs<long>(name);
        }

        public object GetObjectProperty(string name)
        {
            return _properties.FindAs<object>(name);
        }

        public short GetShortProperty(string name)
        {
            return _properties.FindAs<short>(name);
        }

        public string GetStringProperty(string name)
        {
            return _properties.FindAs<string>(name);
        }

        public void SetBooleanProperty(string name, bool value)
        {
            _properties[name] = value;
        }

        public void SetByteProperty(string name, byte value)
        {
            _properties[name] = value;
        }

        public void SetDoubleProperty(string name, double value)
        {
            _properties[name] = value;
        }

        public void SetFloatProperty(string name, float value)
        {
            _properties[name] = value;
        }

        public void SetIntProperty(string name, int value)
        {
            _properties[name] = value;
        }

        public void SetLongProperty(string name, long value)
        {
            _properties[name] = value;
        }

        public void SetObjectProperty(string name, object value)
        {
            _properties[name] = value;
        }

        public void SetShortProperty(string name, short value)
        {
            _properties[name] = value;
        }

        public void SetStringProperty(string name, string value)
        {
            _properties[name] = value;
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
