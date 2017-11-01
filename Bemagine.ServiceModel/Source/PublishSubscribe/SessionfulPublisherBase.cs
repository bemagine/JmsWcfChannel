//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.ServiceModel;
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Represents the publisher or broadcaster of data as part of the publisher-subscriber
    /// message exchange pattern. Subscriptions are maintained within the context of a session. 
    /// The session models the subscriber that may subscribe to any number of publications 
    /// offered by the publisher.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public abstract class SessionfulPublisherBase<ISubscriptionContractT, IPublicationContractT>
        where ISubscriptionContractT : class
        where IPublicationContractT : class
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly IPublicationContractT _publicationCallback;
        private readonly HashSet<string> _instanceSubscriptions = new HashSet<string>();

        private readonly object _sessionSync = new object();

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        static SessionfulPublisherBase()
        {
            PubSubContractValidator.AllOperationsAreOneWay<ISubscriptionContractT>();
            PubSubContractValidator.AllOperationsAreOneWay<IPublicationContractT>();
        }

        protected SessionfulPublisherBase()
        {
            _publicationCallback = 
                OperationContext.Current.GetCallbackChannel<IPublicationContractT>();

            //------------------------------------------------------------------------------------//
            // When the channel is closed all subscriptions for the session are canceled. This 
            // ensures that the publication process does not attempt to publish data to phantom 
            // subscribers.
            //------------------------------------------------------------------------------------//

            OperationContext.Current.Channel.Closing += OnCloseCancelAllSubscriptions;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Channel event handler to cancel all subscriptions when the session is closing.
        /// </summary>
        //----------------------------------------------------------------------------------------//        

        private void OnCloseCancelAllSubscriptions(object state, EventArgs eventArgs)
        {
            lock (_sessionSync)
            {
                foreach (string subscription in _instanceSubscriptions)
                {
                    PublicationSessionsManager<IPublicationContractT>.Instance.CancelSubscription(
                        subscription, _publicationCallback);
                }
            } 
        }

        //----------------------------------------------------------------------------------------//
        // protected interfaces
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Registers the subscriber's subscription for a particular publication.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected void RegisterSubscription(string subscriptionToken)
        {
            lock (_sessionSync)
            {
                PublicationSessionsManager<IPublicationContractT>.Instance.RegisterSubscription(
                    subscriptionToken, _publicationCallback);

                _instanceSubscriptions.Add(subscriptionToken);
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Cancels the subscriber's subscription for a particular publication.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected void CancelSubscription(string subscriptionToken)
        {
            lock (_sessionSync)
            {
                PublicationSessionsManager<IPublicationContractT>.Instance.CancelSubscription(
                    subscriptionToken, _publicationCallback);

                _instanceSubscriptions.Remove(subscriptionToken);
            }
        }
       
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Publishes data to all subscribers of the publication.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected static void Publish(string subscriptionToken, 
            Action<IPublicationContractT> onPublication)
        {
             PublicationSessionsManager<IPublicationContractT>.Instance.Publish(
                 subscriptionToken, onPublication);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
