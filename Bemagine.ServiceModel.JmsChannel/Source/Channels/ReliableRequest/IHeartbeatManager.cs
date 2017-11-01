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

    using System;
    
    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// An empty interface to facilitate unit testing of the ReliableRequestChannel.
    /// </summary>
    /// <remarks>
    /// Accurate and simplified testing of the ReliableRequestChannel requires the ability to
    /// control the timing heartbeat events; otherwise, the testing code would require an 
    /// exorbitant amount of synchronization logic. Clearly, that is not a technical debt worth
    /// paying. The lesser two evils is to provide a constructor in the ReliableRequestChannel
    /// that accepts as a parameter an instance of IHeartbeatManager and another without. In the
    /// latter case, it would simply call the former creating an instance of HeartbeatManager.
    /// The former constructor would be primarily employed in unit testing where a custom
    /// IHeartbeatManager with the requirement that events be explicitly generated; as opposed
    /// to timer generated.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal interface IHeartbeatManager : IDisposable
    {
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//

