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
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System.Runtime.Serialization;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [DataContract(Name="ReliableRequestHeader", Namespace="http://www.ubs.com/ReliableRequest")]
    internal class ReliableRequestHeader
    {
        [DataMember(Name="MessageType", Order=0)] 
        public ReliableRequestMessageType MessageType { get; set; }

        [DataMember(Name="ServiceName", Order=1)]  
        public string ServiceName { get; set; }

        [DataMember(Name="ServiceSession", Order=2)]
        public string ServiceSession { get; set; }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
