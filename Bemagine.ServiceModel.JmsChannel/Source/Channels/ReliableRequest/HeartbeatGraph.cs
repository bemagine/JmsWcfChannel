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
    // using aliases
    //--------------------------------------------------------------------------------------------//

    using HeartbeatGraphSet = System.Collections.Generic.HashSet<string>;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The graph that maintains the state transitions of services from the alive to the flatlined
    /// states and vice versa from the perspective of the requestor (i.e. the client).
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class HeartbeatGraph
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly object _instanceLock = new object();
        private HeartbeatGraphSet _alive = new HeartbeatGraphSet();
        private HeartbeatGraphSet _flatlined = new HeartbeatGraphSet();

        //----------------------------------------------------------------------------------------//
        // internal interface - unit test infrastructure
        //----------------------------------------------------------------------------------------//

        internal int AliveCount { get { return _alive.Count; } }
        internal int FlatlinedCount { get { return _flatlined.Count; } }

        //------------------- --------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Generates a pulse for the specified service and session ID. Pulsing an service and 
        /// session ID tuple transitions an the tuple to the alive state.
        ///</summary>
        /// <remarks>
        /// It is not sufficient to monitor services by name alone. A service could fault and be 
        /// respawned within the time horizon for detecting service down events. Therefore, each 
        /// service provides a session ID to identify the service instance processing requests.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        public void Pulse(string serviceName, string serviceSession)
        {
            lock (_instanceLock)
            {
                //--------------------------------------------------------------------------------//
                // ignore invalid engine or session IDs.
                //--------------------------------------------------------------------------------//

                if ((serviceName != null) && (serviceSession != null))
                {                    
                    string serviceKey = String.Format("{0}|{1}", serviceName, serviceSession);
                    // LogUtility.Debug("Reliable request pulse recorded for [{0}]", serviceKey);

                    _alive.Add(serviceKey);
                    _flatlined.Remove(serviceKey);
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// <para>
        /// Taking a pulse represents the boundry for detecting flatlined services. The periodicity 
        /// between pulse measures is governed by external forces that govern the acceptable time 
        /// horizons for pulse events to occur.
        /// </para>
        /// <para>
        /// This method performs two primary functions. The first is to fault service sessions in 
        /// the flatlined state. After faulting flatlined services sessions, the service sessions 
        /// in the alive state are transitioned to the flatlined state. These service sessions are
        /// assumed flatlined until the next pulse event confirms they are alive.
        /// </para>
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void TakePulse()
        {
            lock (_instanceLock)
            {
                //--------------------------------------------------------------------------------//
                // Fault the engines in the flatlined state.
                //--------------------------------------------------------------------------------//

                foreach (string serviceKey in _flatlined)
                {
                    string[] tokens = serviceKey.Split('|');                    
                    ReliableRequestManager.Instance.Faulted(tokens[0], tokens[1]);

                    LogUtility.Error(
                        "Service down event detected for [Service : {0} Session: {1}]. The "+
                        "service may have failed due to some errors or taken down intentionally.",
                        tokens[0], tokens[1]);
                }

                //--------------------------------------------------------------------------------//
                // Clear the flatlined elements that have been faulted.
                //--------------------------------------------------------------------------------//

                _flatlined.Clear();

                //--------------------------------------------------------------------------------//
                // Transition the previously known engines in the alive state to the flatlined 
                // state.
                //--------------------------------------------------------------------------------//

                HeartbeatGraphSet swapSet = _flatlined;
                _flatlined = _alive;
                _alive = swapSet;                
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
