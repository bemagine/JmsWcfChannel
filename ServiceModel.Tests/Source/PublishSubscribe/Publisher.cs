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
    using System.ServiceModel;

    using Bemagine.ServiceModel;
    
    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A contrived test publisher service.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class Publisher 
        : SessionfulPublisherBase<ISubscription, IPublication>, ISubscription
    {
        private const string TestPublication = "TestPublication";
        private readonly string _sessionId;

        public Publisher()
        {
            _sessionId = OperationContext.Current.SessionId;
        }

        //----------------------------------------------------------------------------------------//
        // ISubscription interface implementation
        //----------------------------------------------------------------------------------------//

        public void SubscribeForPublication()
        {            
            RegisterSubscription(TestPublication);
            DebugEx.WriteLine("[{0}] subscription registered", _sessionId);
        }

        public void UnsubscribeForPublication()
        {
            CancelSubscription(TestPublication);
            DebugEx.WriteLine("[{0}] subscription unregistered", _sessionId);
        }

        public void SimulateUnexpectedServiceTermination()
        {
            DebugEx.WriteLine("[{0}] unexpectedly terminated", _sessionId);
            OperationContext.Current.Channel.Abort();
        }

        //----------------------------------------------------------------------------------------//
        // static construction
        //----------------------------------------------------------------------------------------//

        public static void StartPublishing()
        {
            DebugEx.WriteLine("Publication pump started.");
            ThreadPool.QueueUserWorkItem(
                (state) => 
                { 
                    Shutdown = false;
                    PublicationLoop(); 
                });
        }

        //----------------------------------------------------------------------------------------//
        // publications
        //----------------------------------------------------------------------------------------//

        public static bool Shutdown { get; set; }

        private static void PublicationLoop()
        {
            for (int i=0; !Shutdown; ++i)
            {
                Thread.Sleep(100);
                DebugEx.WriteLine("Publishing [{0}]", i);

                int capture = i;

                Publish(
                    TestPublication,
                    (callback) =>
                    {
                        callback.OnPublication(String.Format("Publication [{0}]", capture));
                    });
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
