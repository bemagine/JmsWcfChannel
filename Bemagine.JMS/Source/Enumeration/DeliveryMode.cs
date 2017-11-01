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
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System.Runtime.Serialization;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Enumerates the message delivery modes specified by JMS 1.1.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    [DataContract(Name="DeliveryMode", Namespace = "http://www.bemagine.com/JMS")]
    public enum DeliveryMode
    {
        [EnumMember] Persistent     = 0,
        [EnumMember] NonPersistent  = 1
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
