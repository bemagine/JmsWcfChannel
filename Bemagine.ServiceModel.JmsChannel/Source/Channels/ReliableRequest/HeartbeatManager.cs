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
    using System.Threading;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Manages the creation of the timer mechanisms for either generating heartbeat messages in
    /// the service context or determining the health of known service instances in the client
    /// context.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class HeartbeatManager : IHeartbeatManager
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly HeartbeatGraph _graph = new HeartbeatGraph();
        private readonly HeartbeatTransport _transport;
        private readonly Timer _heartbeatTimer;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public HeartbeatManager(int periodicity, JmsTransportContext jmsTransportProperties, 
            Uri uri, bool isClient)
        {
            _transport = new HeartbeatTransport(jmsTransportProperties, uri, isClient,
                (service, session) => _graph.Pulse(service, session));

            _heartbeatTimer =
                new Timer((isClient) ? (TimerCallback) Stethoscope : Heartbeat);

            _heartbeatTimer.Change(periodicity, (isClient) ? periodicity : (periodicity / 2));
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The TimerCallback that invokes the HeartbeatGraph's SendHeartbeat method that 
        /// sends a broadcast heartbeat message (via a topic) to all listening clients in 
        /// the service context.
        /// </summary>
        //----------------------------------------------------------------------------------------//        

        private void Heartbeat(object state)
        {
            _transport.SendHeartbeat(
                ReliableRequestProperties.ServiceHost, 
                ReliableRequestProperties.ServiceInstance);

            // LogUtility.Debug("Heartbeat generated [Service : {0} Session: {1}].",
            //     ReliableRequestProperties.ServiceHost, ReliableRequestProperties.ServiceInstance);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The TimerCallback that invokes the HeartbeatGraph's TakePulse method that determines
        /// the health of the known service instances in the client context.
        /// </summary>
        //----------------------------------------------------------------------------------------//        

        private void Stethoscope(object state)
        {
            _graph.TakePulse();
            // LogUtility.Debug("Pulse taken.");
        }

        //----------------------------------------------------------------------------------------//  
        // IDisposable Members
        //----------------------------------------------------------------------------------------//  

        public void Dispose()
        {
            _heartbeatTimer.Dispose();
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
