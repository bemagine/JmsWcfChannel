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
    /// BindingPropertyDefaults is a static class that defines the set of default values that
    /// relate to the set of binding properties exposed by the JmsQueueTransportBindingElement.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class BindingPropertyDefaults
    {
        public const string UserName                 = "";
        public const string Password                 = "";
        public const bool PersistentDeliveryRequired = false;
        public const long MaxBufferPoolSize          = 1024 * 1024;
        public const long MaxReceivedMessageSize     = 64 * 1024;

        public const long MaxBufferPoolSizeMinimum          = 0L;
        public const long MaxReceivedMessageSizeMinimum     = 1L;

        public const string Scheme = "net.jmsq";
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
