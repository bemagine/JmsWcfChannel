//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.Tests.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Tests
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Threading;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.Collections.Generic;

    using NUnit.Framework;
    
    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Provides the set of tests for the ServiceModel Publish-Subscribe mechanisms
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class PublishSubscribeTests
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private static ServiceHost _serviceHost;

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Initializes the publication service prior to executing any test cases.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [TestFixtureSetUp]
        public void OnTestFixtureSetup()
        {
            DebugEx.WriteLine("Creating the publisher.");
            _serviceHost = new ServiceHost(typeof(Publisher));
            _serviceHost.Open();

            Publisher.StartPublishing();
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Disposes of the subscriber client and shuts down the service after all test cases
        /// have run.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            ((IDisposable) Subscriber.DefaultInstance).Dispose();

            if (_serviceHost.State == CommunicationState.Opened)
            {
                try { _serviceHost.Close(); }
                catch { _serviceHost.Abort(); }
            }
        }

        //----------------------------------------------------------------------------------------//
        // test methods
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests the basic publish-subscribe choreography between a single subscriber and the
        /// publication service.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void TestBasicPublishSubscribe()
        {
            //------------------------------------------------------------------------------------//
            // The publication event handler
            //------------------------------------------------------------------------------------//

            EventHandler<PublicationEventArgs> eventHandler =
                (statem, eventArgs) => 
                {
                    var opEventArgs = eventArgs as OnPublicationEventArgs;

                    if (opEventArgs != null)
                        Debug.WriteLine(opEventArgs.Publication);

                    _waitHandle.Set();
                };

            //------------------------------------------------------------------------------------//
            // Subscribe for a particular publication
            //------------------------------------------------------------------------------------//

            try { Subscriber.DefaultInstance.SubscribeForPublication(eventHandler); }
            catch (Exception e) 
            { 
                Assert.Fail("{0} -- {1}", e.Message, 
                    (e.InnerException != null) ? e.InnerException.Message : "NONE"); 
            }

            //------------------------------------------------------------------------------------//
            // Receive some publications
            //------------------------------------------------------------------------------------//

            for (int i=0; i<5; ++i)
            {
                Assert.IsTrue(_waitHandle.WaitOne(5000));
            }

            //------------------------------------------------------------------------------------//
            // Unsubscribe for the publication
            //------------------------------------------------------------------------------------//

            Subscriber.DefaultInstance.UnsubscribeForPublication(eventHandler);         
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests the basic publish-subscribe choreography between multiple subscribers and the
        /// publication service.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void TestMultipleubscribers()
        {
            //------------------------------------------------------------------------------------//
            // Create the subscribers
            //------------------------------------------------------------------------------------//

            var subscribers = new List<Subscriber>();

            for (int i=0; i<5; ++i)
                subscribers.Add(new Subscriber());

            //------------------------------------------------------------------------------------//
            // Subscribe for a particular publication
            //------------------------------------------------------------------------------------//

            try 
            { 
                subscribers.ForEach(
                    (subscriber) => 
                    {
                        subscriber.SubscribeForPublication(
                            (state, eventArgs) => 
                            {
                                var opEventArgs = eventArgs as OnPublicationEventArgs;

                                if (opEventArgs != null)
                                {
                                    Debug.WriteLine(
                                        "Subscriber [{0}] -- {1}",
                                        subscriber.SessionId, opEventArgs.Publication);
                                }

                                //_waitHandle.Set();
                            }
                        );
                    }
                );
            }
            catch (Exception e) 
            { 
                Assert.Fail("{0} -- {1}", e.Message, 
                    (e.InnerException != null) ? e.InnerException.Message : "NONE"); 
            }

            //------------------------------------------------------------------------------------//
            // Receive some publications
            //------------------------------------------------------------------------------------//

            Thread.Sleep(10000);

            //------------------------------------------------------------------------------------//
            // Unsubscribe for the publication and cleanup.
            //------------------------------------------------------------------------------------//

            subscribers.ForEach(
                (subscriber) => 
                {
                    subscriber.UnsubscribeForPublicationAllSubscriptions();
                    ((IDisposable) subscriber).Dispose();
                }
            );
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tests the basic publish-subscribe choreography between multiple subscribers and the
        /// publication service.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [Test]
        public void TestResubscriptionAfterFault()
        {
            //------------------------------------------------------------------------------------//
            // The publication event handler
            //------------------------------------------------------------------------------------//

            EventHandler<PublicationEventArgs> eventHandler =
                (state, eventArgs) => 
                {
                    var opEventArgs = eventArgs as OnPublicationEventArgs;

                    if (opEventArgs != null)
                    {
                        DebugEx.WriteLine(
                            "[{0}] {1}", 
                            Subscriber.DefaultInstance.SessionId, 
                            opEventArgs.Publication);
                    }

                    _waitHandle.Set();
                };

            //------------------------------------------------------------------------------------//
            // Subscribe for a particular publication
            //------------------------------------------------------------------------------------//

            try { Subscriber.DefaultInstance.SubscribeForPublication(eventHandler); }
            catch (Exception e) 
            { 
                Assert.Fail("{0} -- {1}", e.Message, 
                    (e.InnerException != null) ? e.InnerException.Message : "NONE"); 
            }

            //------------------------------------------------------------------------------------//
            // Receive some publications
            //------------------------------------------------------------------------------------//

            _waitHandle.Reset();
            for (int i=0; i<5; ++i)
            {
                Assert.IsTrue(_waitHandle.WaitOne(5000));
                _waitHandle.Reset();
            }

            //------------------------------------------------------------------------------------//
            // fault the channel
            //------------------------------------------------------------------------------------//

            Publisher.Shutdown = true;
            _serviceHost.Close();

            OnTestFixtureSetup();
            Thread.Sleep(1000);

            //------------------------------------------------------------------------------------//
            // Receive some publications
            //------------------------------------------------------------------------------------//

            for (int i=0; i<5; ++i)
            {
                Assert.IsTrue(_waitHandle.WaitOne(20000));
                _waitHandle.Reset();
            }

            //------------------------------------------------------------------------------------//
            // Unsubscribe for the publication
            //------------------------------------------------------------------------------------//

            Subscriber.DefaultInstance.UnsubscribeForPublication(eventHandler); 
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
