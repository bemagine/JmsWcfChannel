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

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Test suite for the Bemagine.ServiceModel.MockJmsProvider.RoundRobinMessageDispatch class.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class RoundRobinMessageDispatchTests
    {
        //----------------------------------------------------------------------------------------//
        // tests
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests the single consumer registration and message dispatch mechanics.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void RegisterSingleConsumerAndDispatchMessage()
        {
            var roundRobinDispatch = new MockJmsProvider.RoundRobinMessageDispatch();
            bool dispatchSuccess = false;

            roundRobinDispatch.RegisterConsumer((message) => dispatchSuccess = true);
            roundRobinDispatch.DispatchMessage(new MockJmsProvider.MockMessage());

            Assert.IsTrue(dispatchSuccess);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests multiple consumer registration and message dispatch mechanics.
        /// </summary>
        /// <remarks>
        /// To validate this test case, n number of consumers are registered each with an ID from
        /// the sequence of natural numbers up to and including n. Subsequently, if n messages
        /// are dispatched then the sum of those IDs is n(n+1) / 2. The rationale follows that
        /// if the round robin algorithm is implemented correctly each and every consumer callback
        /// will be invoked at most once. Now, we can extend the logic to multiples of n. Consider
        /// the case where 2n messages are generated each consumer callback would be invoked at
        /// most twice and the sum would equal 2 * (n/(n+1) / 2). So, the progression continues.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        [TestCase(5, 1)]
        [TestCase(10, 2)]
        public void RegisteMultipleConsumersAndDispatchMessages(int nConsumers, int multiplier)
        {
            var roundRobinDispatch = new MockJmsProvider.RoundRobinMessageDispatch();
            int dispatchCount = 0;

            for (int i=1; i <= nConsumers; ++i)
            {
                int capturedI = i;

                //--------------------------------------------------------------------------------//
                // ReSharper disable AccessToModifiedClosure
                //
                // In this case, given the logic of the test dispatchCount is intended to be 
                // modified.

                roundRobinDispatch.RegisterConsumer((message) => dispatchCount += capturedI);
                
                // ReSharper restore AccessToModifiedClosure
                //--------------------------------------------------------------------------------//
            }

            for (int i = 1; i <= nConsumers * multiplier; ++i)
                roundRobinDispatch.DispatchMessage(new MockJmsProvider.MockMessage());

            Assert.IsTrue( dispatchCount == (multiplier * nConsumers.SequenceSum()) );
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
