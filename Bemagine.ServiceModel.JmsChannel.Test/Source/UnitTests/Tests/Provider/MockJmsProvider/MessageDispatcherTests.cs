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

    using System;
    using NUnit.Framework;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Test suite for the Bemagine.ServiceModel.MockJmsProvider.MessageDispatcher class.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class MessageDispatcherTests
    {
        //----------------------------------------------------------------------------------------//
        // tests
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests the single desitination with a single consumer registration and message dispatch 
        /// mechanics.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void RegisterSingleDestinationSingleConsumerAndDispatchMessage()
        {
            var messageDispatcher = MockJmsProvider.MessageDispatcher.Instance;
            var destination = new MockJmsProvider.MockDestination("SomeDestination");

            bool dispatchSuccess = false;
            messageDispatcher.RegisterConsumer(destination, (message) => dispatchSuccess = true);
            messageDispatcher.DispatchMessage(destination, new MockJmsProvider.MockMessage());

            Assert.IsTrue(dispatchSuccess);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests multuple desitination each with multiple consumers registration and message 
        /// dispatch mechanics.
        /// </summary>
        /// <remarks>
        /// See the RoundRobinMessageDispatcherTests.RegisteMultipleConsumersAndDispatchMessages
        /// remarks for the test rationale.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        [TestCase(3, 5, 1)]
        [TestCase(4, 10, 3)]
        public void RegisterMultipleDestinationsMultipleConsumersAndDispatchMessage(
            int nDestinations, int nConsumers, int multiplier)
        {
            var messageDispatcher = MockJmsProvider.MessageDispatcher.Instance;
            int dispatchCount = 0;

            //------------------------------------------------------------------------------------//
            // Register m consumers for n destinations.
            //------------------------------------------------------------------------------------//

            for (int i = 1; i <= nDestinations; ++i)
            {
                var destination =
                    new MockJmsProvider.MockDestination(String.Format("Destination-{0}", i));

                for (int j = 1; j <= nConsumers; ++j)
                {
                    int capturedI = i;
                    int capturedJ = j;

                    //----------------------------------------------------------------------------//
                    // ReSharper disable AccessToModifiedClosure
                    //
                    // In this case, given the logic of the test dispatchCount is intended to be 
                    // modified.

                    messageDispatcher.RegisterConsumer(
                        destination,
                        (message) => dispatchCount += capturedI + capturedJ);

                    // ReSharper restore AccessToModifiedClosure
                    //----------------------------------------------------------------------------//
                }
            }

            //------------------------------------------------------------------------------------//
            // Validate the registrations.
            //------------------------------------------------------------------------------------//

            Assert.IsTrue(messageDispatcher.NumberOfDestinationRegistrations == nDestinations);
            Assert.IsTrue(messageDispatcher.NumberOfConsumerRegistrations == (nDestinations * nConsumers));

            //------------------------------------------------------------------------------------//
            // Foreach desitnation dispatch nCosumer * multiplier messages.
            //------------------------------------------------------------------------------------//

            for (int i = 1; i <= nDestinations; ++i)
            {
                var destination =
                    new MockJmsProvider.MockDestination(String.Format("Destination-{0}", i));

                for (int j = 1; j <= nConsumers * multiplier; ++j)
                    messageDispatcher.DispatchMessage(destination, new MockJmsProvider.MockMessage());
            }

            //------------------------------------------------------------------------------------//
            // Validate the dispatches
            //------------------------------------------------------------------------------------//

            var expectedDispatchCount = 
                (nConsumers * nDestinations.SequenceSum()) + (nDestinations * nConsumers.SequenceSum());

            expectedDispatchCount *= multiplier;

            Assert.IsTrue(dispatchCount == expectedDispatchCount);

            //------------------------------------------------------------------------------------//
            // The message dispatcher must be cleared of all registrations to ensure that subsequent
            // test cases perform correctly. This is because any latent registrations will impact
            // the expected results as they may be invoked instead of the current test context
            // lambdas.
            //------------------------------------------------------------------------------------//

            messageDispatcher.ClearAllRegistrations();
            Assert.IsTrue(messageDispatcher.NumberOfDestinationRegistrations == 0);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
