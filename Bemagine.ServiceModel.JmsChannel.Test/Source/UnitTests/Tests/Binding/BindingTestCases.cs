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
    using System.Collections;

    using NUnit.Framework;
    using Bemagine.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Test cases for the suite of bindings related tests.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class BindingTestCases
    {
        //----------------------------------------------------------------------------------------//
        // BindingTest enum
        //
        //
        //----------------------------------------------------------------------------------------//

        [Flags]
        private enum BindingTest
        {
            BindingElementToBindingElement = 0x0,
            BindingElementExtentionToBindingElementExtention = 0x1,
            BindingElementExtentionToBindingElement = 0x2
        }

        //----------------------------------------------------------------------------------------//
        // TestCaseData creators
        //----------------------------------------------------------------------------------------//

        private static TestCaseData CreateIReliableRequestContext<SourceT, DestinationT>()
            where SourceT : IReliableRequestContext, new()
            where DestinationT : IReliableRequestContext, new()
        {
            return new TestCaseData(
                BindingTestContext.CreateBindingContext(
                    new SourceT { HeartbeatPeriodicity = 5555 },
                    BindingContextDirection.Source),

                BindingTestContext.CreateBindingContext(
                    new DestinationT(),
                    BindingContextDirection.Destination)
            );
        }

        private static TestCaseData CreateIJmsTransportContext<SourceT, DestinationT>()
            where SourceT : IJmsTransportContext, new()
            where DestinationT : IJmsTransportContext, new()
        {
            return new TestCaseData(
                BindingTestContext.CreateBindingContext(
                    new SourceT
                    {
                        MaxBufferPoolSize = 12345,
                        MaxReceivedMessageSize = 12345,
                        MessageSizeCompressionThreshold = 12345,
                        UserName = "SomeUser",
                        Password = "SomePassword",
                        PersistentDeliveryRequired = false,
                        ExplicitResponseDestinationName = "SomeExplicitDestination",

                        JmsConnectionFactoryType =
                            typeof(MockJmsProvider.MockConnectionFactory).AssemblyQualifiedName
                    },
                    BindingContextDirection.Source),

                BindingTestContext.CreateBindingContext(
                    new DestinationT(),
                    BindingContextDirection.Destination)
            );
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Test cases for BindingElement & BindingElementExtension CopyFrom semantics.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static IEnumerable CopyFromTestCases
        {
            get
            {
                return TestCases(
                    "CopyFrom",
                    BindingTest.BindingElementToBindingElement |
                    BindingTest.BindingElementExtentionToBindingElementExtention);
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Test cases for BindingElement semantics.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static IEnumerable CloneTestCases
        {
            get { return TestCases("Clone", BindingTest.BindingElementToBindingElement); }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private static IEnumerable TestCases(string testName, BindingTest tests)
        {
            if ((tests & BindingTest.BindingElementToBindingElement) == 
                BindingTest.BindingElementToBindingElement)
            {
                yield return CreateIReliableRequestContext<
                    ReliableRequestBindingElement,
                    ReliableRequestBindingElement
                >().SetName(String.Format("ReliableRequestBindingElement {0} Test", testName));

                yield return CreateIJmsTransportContext<
                    JmsTransportBindingElementTestWrapper,
                    JmsTransportBindingElementTestWrapper
                >().SetName(String.Format("JmsTransportBindingElement {0} Test", testName));
            }

            if ((tests & BindingTest.BindingElementExtentionToBindingElementExtention) ==
                BindingTest.BindingElementExtentionToBindingElementExtention)
            {
                yield return CreateIReliableRequestContext<
                    ReliableRequestBindingElementExtension,
                    ReliableRequestBindingElementExtension
                >().SetName(String.Format("ReliableRequestBindingElementExtension {0} Test", testName));

                yield return CreateIJmsTransportContext<
                    JmsQueueTransportBindingElementExtension,
                    JmsQueueTransportBindingElementExtension
                >().SetName(String.Format("JmsQueueTransportBindingElementExtension {0} Test", testName));

                yield return CreateIJmsTransportContext<
                    JmsTopicTransportBindingElementExtension,
                    JmsTopicTransportBindingElementExtension
                >().SetName(String.Format("JmsTopicTransportBindingElementExtension {0} Test", testName));
            }

            if ((tests & BindingTest.BindingElementExtentionToBindingElement) ==
                BindingTest.BindingElementExtentionToBindingElement)
            {
                yield return CreateIReliableRequestContext<
                    ReliableRequestBindingElementExtension,
                    ReliableRequestBindingElement
                >().SetName(
                        String.Format(
                            "ReliableRequestBindingElementExtension to ReliableRequestBindingElement {0} Test", 
                            testName
                    ));

                yield return CreateIJmsTransportContext<
                    JmsQueueTransportBindingElementExtension,
                    JmsTransportBindingElementTestWrapper
                >().SetName(
                        String.Format(
                            "JmsQueueTransportBindingElementExtension to JmsTransportBindingElement {0} Test", 
                            testName
                        ));

                yield return CreateIJmsTransportContext<
                    JmsTopicTransportBindingElementExtension,
                    JmsTransportBindingElementTestWrapper
                >().SetName(
                        String.Format(
                            "JmsTopicTransportBindingElementExtension to JmsTransportBindingElement {0} Test", 
                            testName
                    ));
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
