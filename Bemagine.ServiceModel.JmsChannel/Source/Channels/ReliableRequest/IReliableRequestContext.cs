//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla License Version 1.1 
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
    /// An interface defining the contextual data required to configure, construct and support
    /// the functional requirements of the reliable request infrastructure.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public interface IReliableRequestContext
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Copies the members of the specified instance to the current instance.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void CopyFrom(IReliableRequestContext rhs);
       
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the periodicity in milliseconts that heartbeat events occur. In the client 
        /// context the heartbeat event is pulse taking and in the service context generating a 
        /// heartbeat.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        int HeartbeatPeriodicity { get; set; }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The suite of IReliableRequestContext extension methods.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class IReliableRequestContextEx
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// An extension that implements the IReliableRequestContext copy semantics.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static void CopyFromEx(this IReliableRequestContext lhs, IReliableRequestContext rhs)
        {
            lhs.HeartbeatPeriodicity = rhs.HeartbeatPeriodicity;
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
