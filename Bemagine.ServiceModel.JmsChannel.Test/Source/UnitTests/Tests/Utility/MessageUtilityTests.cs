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

    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Channels;

    using NUnit.Framework;
    using Bemagine.ServiceModel.Channels;

    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Bemagine.ServiceModel.Channels.MessageUtility suite of tests.
    /// </summary>
    //---------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class MessageUtilityTests
    {
        #region nested types
        //----------------------------------------------------------------------------------------//
        // nested types
        //----------------------------------------------------------------------------------------//

        [DataContract(
            Name = "SomeMessageHeader", 
            Namespace = "http://www.bemagine.com/JmsChannel/Test")
        ]
        private sealed class SomeMessageHeader
        {
            [DataMember]
            private int SomeMember { get; set; }

            public SomeMessageHeader()
            {
                SomeMember = (new Random()).Next(0, 1000000);
            }

            private bool Equals(SomeMessageHeader other)
            {
                return SomeMember == other.SomeMember;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is SomeMessageHeader && Equals((SomeMessageHeader) obj);
            }

            public override int GetHashCode()
            {
                return SomeMember;
            }

            public static bool operator ==(SomeMessageHeader a, SomeMessageHeader b)
            {
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }

                if (a.IsNull() || b.IsNull())
                {
                    return false;
                }

                return a.SomeMember == b.SomeMember;
            }

            public static bool operator !=(SomeMessageHeader a, SomeMessageHeader b)
            {
                return !(a == b);
            }
        }
        #endregion

        #region data members & setup
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private Message _message;
        
        //----------------------------------------------------------------------------------------//
        // test setup
        //----------------------------------------------------------------------------------------//

        [SetUp]
        public void TestSetup()
        {
            _message = BemagineEx.CreateDefaultMessage();
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
        public void AddThenGetHeader()
        {
            var inHeader = new SomeMessageHeader();
            SomeMessageHeader outHeader;

            Assert.IsTrue(_message.AddHeader(inHeader));
            Assert.IsTrue(_message.TryGetHeader(out outHeader));
            Assert.IsTrue(inHeader == outHeader);
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
