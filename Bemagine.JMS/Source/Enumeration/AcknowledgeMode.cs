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
    /// Enumerates the methods by which messages are acknowledged as specified by JMS 1.1.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    [DataContract(Name="AcknowledgeMode", Namespace = "http://www.bemagine.com/JMS")]
    public enum AcknowledgeMode
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// With this acknowledgment mode, the session automatically acknowledges a client's 
        /// receipt of a message either when the session has successfully returned from a call 
        /// to receive or when the message listener the session has called to process the message 
        /// successfully returns.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [EnumMember] AutoAcknowledge    = 0,

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// With this acknowledgment mode, the client acknowledges a consumed message by calling 
        /// the message's acknowledge method.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [EnumMember] ClientAcknowledge  = 1,
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// This acknowledgment mode instructs the session to lazily acknowledge the delivery of 
        /// messages.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [EnumMember] DupsOkAcknowledge   = 2,
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// This value is returned from the method getAcknowledgeMode if the session is transacted.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [EnumMember] SessionTransacted  = 3
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
          