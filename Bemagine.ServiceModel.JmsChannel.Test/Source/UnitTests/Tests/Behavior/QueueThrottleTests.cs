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
    /// Verifies and validates the behavior of the queue throttling IDispatchMessageInspector
    /// behavior.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class QueueThrottleTests
    {
        //----------------------------------------------------------------------------------------//
        // tests
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests that the call to QueueThrottle.SlowDown() does not block when the number of 
        /// simulated concurrent calls does not exceed the throttle threshold.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void NotThrottled( [Range(1, 4)] int throttleThreshold )
        {
            var queueThrottle = new QueueThrottle(throttleThreshold);
            queueThrottle.SimulateMessageReception(throttleThreshold - 1);
            Assert.That(queueThrottle.SlowDown(50), Is.True);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests that the call to QueueThrottle.SlowDown() does blocks when the number of 
        /// simulated concurrent calls does exceeds the throttle threshold. 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void Throttled( [Range(1, 4)] int throttleThreshold )
        {
            var queueThrottle = new QueueThrottle(throttleThreshold);
            queueThrottle.SimulateMessageReception(throttleThreshold);

            Assert.That(queueThrottle.SlowDown(50), Is.False);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests that the call to QueueThrottle.SlowDown() does blocks when the number of 
        /// simulated concurrent calls does exceeds the throttle threshold and does not block
        /// after a simulated response.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void Unthrottled( [Range(1, 4)] int throttleThreshold )
        {
            var queueThrottle = new QueueThrottle(throttleThreshold);

            queueThrottle.SimulateMessageReception(throttleThreshold);
            Assert.That(queueThrottle.SlowDown(50), Is.False);

            queueThrottle.SimulateMessageReply(throttleThreshold);
            Assert.That(queueThrottle.SlowDown(50), Is.True);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
