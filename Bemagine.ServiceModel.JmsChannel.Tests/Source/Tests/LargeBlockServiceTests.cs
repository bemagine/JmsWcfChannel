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
    using System.Collections.Generic;

    using NUnit.Framework;
    using log4net;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Test suite for the LargeBlock
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class LargeBlockServiceTests :
        IntegrationTestFoundation<ILargeBlock, LargeBlockService, LargeBlockServiceClient>
    {
        //----------------------------------------------------------------------------------------//
        // nested types
        //----------------------------------------------------------------------------------------//

        internal sealed class RequestLargeBlockTestCases
        {
            public IEnumerable<uint> TestCases()
            {
                for (uint i=10; i<=12; ++i)
                    yield return (uint) Math.Pow(2, i);
            }
        }

        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private static readonly ILog _logger = LogManager.GetLogger("JmsChannel.Tests");

        //----------------------------------------------------------------------------------------//
        // test methods
        //----------------------------------------------------------------------------------------//
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Test case for the asynchronous retrieval of large blocks from the LargeBlockService.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test, TestCaseSource(typeof(RequestLargeBlockTestCases), "TestCases")]
        public void RequestLargeBlockTest(uint blockSize)
        {
            _logger.DebugFormat("Requesting block [{0}].", blockSize);            

            var timeout = new TimeSpan(0, 2, 0);
            Run(() => LargeBlockServiceClient.Instance.RequestLargeBlock(blockSize), timeout);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
