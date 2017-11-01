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
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Maintains the list of subscribers subscribed to a publication. Implementers of the 
    /// IPublicationContractT callback contract endpoint represent the subscribers.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class PublicationSessionsManager<IPublicationContractT>
        where IPublicationContractT : class
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
       
        private readonly Dictionary<string, HashSet<IPublicationContractT>> _subscriptions =
            new Dictionary<string, HashSet<IPublicationContractT>>();

        private readonly object _instanceSync = new object();

        //----------------------------------------------------------------------------------------//
        // singleton implementation
        //----------------------------------------------------------------------------------------//

        private static PublicationSessionsManager<IPublicationContractT> _instance =
            new PublicationSessionsManager<IPublicationContractT>();

        public static PublicationSessionsManager<IPublicationContractT> Instance 
        { 
            get { return _instance; }
        }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        static PublicationSessionsManager()
        {
        }

        private PublicationSessionsManager()
        {
        }

        //----------------------------------------------------------------------------------------//
        // public interfaces
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Registers the subscriber's subscription for a particular publication.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void RegisterSubscription(string subscriptionToken, 
            IPublicationContractT publicationCallback)
        {
            lock (_instanceSync)
            {
                if (!_subscriptions.ContainsKey(subscriptionToken))
                     _subscriptions.Add(subscriptionToken, new HashSet<IPublicationContractT>());
                
                _subscriptions[subscriptionToken].Add(publicationCallback);
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Cancels the subscriber's subscription for a particular publication.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void CancelSubscription(string subscriptionToken, 
            IPublicationContractT publicationCallback)
        {
            lock (_instanceSync)
            {
                _subscriptions[subscriptionToken].Remove(publicationCallback);
            }
        }
       
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Publishes data to all subscribers of the publication.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void Publish(string subscriptionToken, Action<IPublicationContractT> onPublication)
        {
            lock (_instanceSync)
            {
                HashSet<IPublicationContractT> callbacks;
                if (_subscriptions.TryGetValue(subscriptionToken, out callbacks))
                {
                    foreach (IPublicationContractT callback in callbacks)
                    {
                        //------------------------------------------------------------------------//
                        // It is possible that a session is attempting to cancel a subscription
                        // while a publication event is being fired. This should not impact other
                        // subscribers. Ignore the exception and carry on.
                        //------------------------------------------------------------------------//

                        try { onPublication(callback); }
                        catch { }
                    }
                }
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
