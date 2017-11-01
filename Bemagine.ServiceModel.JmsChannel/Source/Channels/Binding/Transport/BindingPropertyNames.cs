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
    /// <summary>
    /// BindingPropertyNames is a static class that defines the set of property names exposed by
    /// the JmsQueueTransportBindingExtensionElement.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class BindingPropertyNames
    {
        public const string UserName                    = "userName";
        public const string Password                    = "password";
        public const string PersistentDeliveryRequired  = "persistentDeliveryRequired";
        public const string MaxBufferPoolSize           = "maxBufferPoolSize";             
        public const string MaxReceivedMessageSize      = "maxReceivedMessageSize";
        public const string ExplicitResponseQueueName   = "explicitResponseQueueName";
        public const string JmsConnectionFactoryType    = "jmsConnectionFactoryType";

        public const string MessageSizeCompressionThreshold
            ="messageSizeCompressionThreshold";
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
