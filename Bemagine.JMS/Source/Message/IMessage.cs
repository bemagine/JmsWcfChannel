//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.Jms.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Jms
{
    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The base JMS message interfaces.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public interface IMessage
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Converts the message to a the interfaced message type specified by T. Return null
        /// if the conversion fails.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        T Convert<T>() where T: class;

        //----------------------------------------------------------------------------------------//
        // properties
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the message ID.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        string MessageID { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the correlation ID for the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        string CorrelationID { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the bytes representation of the correlation ID for the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        byte[] CorrelationIDAsBytes { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the message priority level.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        MessagePriority Priority { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the DeliveryMode value specified for this message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        DeliveryMode DeliveryMode { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the Destination object for this message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IDestination Destination { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the Destination object to which a reply to this message should be sent.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IDestination ReplyTo { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the message's expiration value.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        long Expiration { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets an indication of whether this message is being redelivered.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        bool Redelivered { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the message timestamp.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        long Timestamp { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the message type identifier supplied by the client when the message was sent.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        string JMSType { get; set; }

        //----------------------------------------------------------------------------------------//
        // methods
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Acknowledges all consumed messages of the session of this consumed message.
        /// </summary>
        //----------------------------------------------------------------------------------------//
        
        void Acknowledge();
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Clears the message body.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void ClearBody();
              
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Clears a message's properties.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void ClearProperties();             

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the value of the bool property with the specified name.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        bool GetBooleanProperty(string name);
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the value of the byte property with the specified name.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        byte  GetByteProperty(string name);                
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the value of the double property with the specified name.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        double GetDoubleProperty(string name);

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the value of the float property with the specified name.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        float GetFloatProperty(string name);

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the value of the int property with the specified name.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        int GetIntProperty(string name);         

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the value of the long property with the specified name.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        long GetLongProperty(string name);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the value of the Java object property with the specified name.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        object GetObjectProperty(string name);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns an Enumeration of all the property names.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        string[] GetPropertyNames();                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the value of the short property with the specified name.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        short GetShortProperty(string name);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the value of the String property with the specified name.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        string GetStringProperty(string name);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Indicates whether a property value exists.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        bool PropertyExists(string name);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sets a bool property value with the specified name into the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void SetBooleanProperty(string name, bool value);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sets a byte property value with the specified name into the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void SetByteProperty(string name, byte value);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sets a double property value with the specified name into the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void SetDoubleProperty(string name, double value);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sets a float property value with the specified name into the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void SetFloatProperty(string name, float value);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sets an int property value with the specified name into the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void SetIntProperty(string name, int value);            

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sets a long property value with the specified name into the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void SetLongProperty(string name, long value);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sets a Java object property value with the specified name into the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void SetObjectProperty(string name, object value);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sets a short property value with the specified name into the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void SetShortProperty(string name, short value);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sets a String property value with the specified name into the message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void SetStringProperty(string name, string value);
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
