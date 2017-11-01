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
    using System.Diagnostics;

    using Bemagine.ServiceModel;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class OnPublicationEventArgs : PublicationEventArgs
    {
        public string Publication { get; private set; }

        public OnPublicationEventArgs(string publication)
        {
            Publication = publication;
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A contrived test subscriber client.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class Subscriber : SubscriberBase<ISubscription, IPublication>, IPublication
    {
        private const string SomePublication = "SomePublication";

        //----------------------------------------------------------------------------------------//
        // IPublication interface implementation
        //----------------------------------------------------------------------------------------//

        void IPublication.OnPublication(string publicationData)
        {
            Debug.Assert(publicationData != null);            
            Publish(SomePublication, new OnPublicationEventArgs(publicationData));
        }

        //----------------------------------------------------------------------------------------//
        // ISubscription interface
        //----------------------------------------------------------------------------------------//

        public void SubscribeForPublication(EventHandler<PublicationEventArgs> eventHandler)
        {
            DebugEx.WriteLine("[{0}] subscription registration request", SessionId);

            Subscribe(
                SomePublication,
                () => ProxyChannel.SubscribeForPublication(),
                eventHandler);
        }
        
        public void UnsubscribeForPublication(EventHandler<PublicationEventArgs> eventHandler)
        {
            DebugEx.WriteLine("[{0}] subscription cancellation request", SessionId);

            CancelSubscription(
                SomePublication,
                () => ProxyChannel.UnsubscribeForPublication(),
                eventHandler);
        }

        public void UnsubscribeForPublicationAllSubscriptions()
        {
            DebugEx.WriteLine("[{0}] all subscription cancellation request", SessionId);
            CancelAllSubscriptions(
                SomePublication,
                () => ProxyChannel.UnsubscribeForPublication());
        }

        public void SimulateUnexpectedServiceTermination()
        {
            DebugEx.WriteLine("[{0}] unexpected session termination", SessionId);
            ProxyChannel.SimulateUnexpectedServiceTermination();
        }

        #region Subscriber Default Instance Implementation
        //----------------------------------------------------------------------------------------//
        // This is not a singleton. A default instance is provided for mocking up the singleton
        // use case. The contructor is public to be able to test the multiple subscriber use case.
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        // data members & properties
        //----------------------------------------------------------------------------------------//

        private static readonly Lazy<Subscriber> _instance =
            new Lazy<Subscriber>(() => new Subscriber());

        public static Subscriber DefaultInstance { get { return _instance.Value; } }
        public string SessionId { get { return ProxyInnerChannel.SessionId; } }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public Subscriber() : base("SubscriberEP")
        {
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
