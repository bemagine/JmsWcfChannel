//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.Test.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.JmsChannel.Test
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using NUnit.Framework;
    using Bemagine.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The suite of tests focused on the ReliableRequest channel's HeartbeatGraph that manages
    /// the state transitions of known service sessions between the alive and flatlined states.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class HeartbeatGraphTests
    {
        //----------------------------------------------------------------------------------------//
        // tests
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Simulates the events that transition the states of the service session tracked by the
        /// HeartbeatGraph.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void StateTransitions()
        {
            var graph = new HeartbeatGraph();

            //------------------------------------------------------------------------------------//
            // Pulse to simulate a service session (i.e. instance) announcing its presence. This 
            // transitions the service session to the alive state. After pulsing the number of 
            // service sessions in the alive state should be 1 and the flatlined state 0.
            //------------------------------------------------------------------------------------//

            graph.Pulse("StateTransitionsServiceName", "StateTransitionsServiceSession");

            Assert.IsTrue(graph.AliveCount == 1);
            Assert.IsTrue(graph.FlatlinedCount == 0);

            //------------------------------------------------------------------------------------//
            // Taking a pulse determines if any services remain in the flatlined state since the 
            // last time the pulse was taken. In this case, those services are faulted.
            //
            // This call to TakePulse() simulates the case where no service sessions are in the 
            // flatlined state. The TakePulse() method swaps the engine sessions in the alive 
            // state with the ones in the flatlined state. As a result, the AliveCount should 
            // always be zero after all calls to take pulse, and the FlatlinedCount should be 
            // equal to the AliveCount prior to the call.
            //------------------------------------------------------------------------------------//

            graph.TakePulse();

            Assert.IsTrue(graph.AliveCount == 0);
            Assert.IsTrue(graph.FlatlinedCount == 1);

            //------------------------------------------------------------------------------------//
            // Calling TakePulse() a second time without calling pulse for the engine session 
            // simulates the flatlined engine session scenario. Post call, both the alive and 
            // flatlined counts should be zero.
            //------------------------------------------------------------------------------------//

            graph.TakePulse();

            Assert.IsTrue(graph.AliveCount== 0);
            Assert.IsTrue(graph.FlatlinedCount == 0);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
