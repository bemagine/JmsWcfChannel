//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.Tests.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.JmsChannel.Tests
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    
    using NUnit.Framework;
    using log4net;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// This test suite employs the a simple EchoService to test the front-to-back communication
    /// infrastructure from a high level service model perspective. The actual channel used whether
    /// a mock, perhaps in memory, channel or a wired channel does effect the test.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class EchoServiceTests
        : IntegrationTestFoundation<IEcho, EchoService, EchoServiceClient>
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private static readonly ILog _logger = LogManager.GetLogger("JmsChannel.Tests");

        //----------------------------------------------------------------------------------------//
        // test methods
        //----------------------------------------------------------------------------------------//
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void AsyncGreetTest()
        {
            _logger.DebugFormat("[EchoServiceTest::AsyncGreetTest] Sending async greeting.");
            Run(() => EchoServiceClient.Instance.AsyncGreet("Saluations"), new TimeSpan(0, 0, 10));
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
