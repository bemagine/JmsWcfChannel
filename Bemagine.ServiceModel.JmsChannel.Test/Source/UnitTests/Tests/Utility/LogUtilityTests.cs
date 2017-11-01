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
    //---------------------------------------------------------------------------------------------//
    // using directives
    //---------------------------------------------------------------------------------------------//

    using System.IO;
    
    using NUnit.Framework;
    using Bemagine.ServiceModel.Channels;

    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Bemagine.ServiceModel.Channels.MessageUtility suite of tests.
    /// </summary>
    //---------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class LogUtilityTests
    {
        #region test fixture setup
        //----------------------------------------------------------------------------------------//
        // test setup
        //----------------------------------------------------------------------------------------//

        [TestFixtureSetUp]
        public void TestSetup()
        {
            log4net.Config.XmlConfigurator.Configure( new FileInfo("JmsChannel.Tests.log4net") );
        }
        #endregion

        #region tests
        //----------------------------------------------------------------------------------------//
        // tests
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests the AddHeader and TryGetHeader message extention methods of the class 
        /// Bemagine.ServiceModel.Channels.MessageUtility.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void WriteTracer()
        {
            LogUtility.Tracer();
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
