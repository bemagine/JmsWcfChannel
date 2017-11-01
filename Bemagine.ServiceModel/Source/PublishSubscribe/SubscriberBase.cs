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

using Action = System.Action;

namespace Bemagine.ServiceModel
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Threading;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.Configuration;
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    // using aliases
    //--------------------------------------------------------------------------------------------//

    using PublicationEventHandler = System.EventHandler<PublicationEventArgs>;

    using SubscriptionDictionary = 
        System.Collections.Generic.Dictionary<string, System.EventHandler<PublicationEventArgs>>;
   
    using SubscriptionActionDictionary = 
        System.Collections.Generic.Dictionary<string, Action>;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Represents the subscriber of broadcast events as part of the publisher-subscriber message 
    /// exchange pattern.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public abstract class SubscriberBase<ISubscriptionContractT, IPublicationContractT> 
        : DuplexClientManagerBase<ISubscriptionContractT>
            where ISubscriptionContractT : class
            where IPublicationContractT : class
    {
        //----------------------------------------------------------------------------------------//
        // data members and member properties
        //----------------------------------------------------------------------------------------//

        private readonly object _sync = new object();
        private readonly object _onUnexpectedSessionTerminationSync = new object();

        private readonly SubscriptionDictionary _subscriptions = new SubscriptionDictionary();
        private readonly SubscriptionActionDictionary _subscriptionActions = 
            new SubscriptionActionDictionary();

        protected bool HasSubscriptions { get { return _subscriptionActions.Count > 0; } }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        static SubscriberBase()
        {
            PubSubContractValidator.AllOperationsAreOneWay<ISubscriptionContractT>();
            PubSubContractValidator.AllOperationsAreOneWay<IPublicationContractT>();
        }

        protected SubscriberBase(string endpoingConfigurationName)
            : base(endpoingConfigurationName)
        {        
            UnexpectedSessionTermination += OnUnexpectedSessionTermination;
            MaxResubscriptionAttempts = 9;
        }

        protected SubscriberBase(string endpoingConfigurationName, int maxResubscriptionAttempts)
            : base(endpoingConfigurationName)
        {        
            UnexpectedSessionTermination += OnUnexpectedSessionTermination;
            MaxResubscriptionAttempts = maxResubscriptionAttempts;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates an instance of the default proxy client. Override to provide a custome
        /// implementation for the proxy client.
        /// </summary>
        //----------------------------------------------------------------------------------------//
        
        protected override IProxyClient<ISubscriptionContractT> CreateInnerProxyClient()
        {
            var duplexProxyClient = (DuplexProxyClient) base.CreateInnerProxyClient();
         
            duplexProxyClient.ProxyInnerChannel.Closed += 
                (state, eventArgs) =>
                {
                    lock (_onUnexpectedSessionTerminationSync)
                    {
                        if (UnexpectedSessionTermination != null)
                            UnexpectedSessionTermination(state, eventArgs);
                    }
                };
            
            return duplexProxyClient;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Calculates the time to wait in milliseconds before attempting to reconnect to a 
        /// publisher after unexpected termination of the session. The value is calculated by
        /// multiplying 100 by the nth value of the fibonacci series.
        /// </summary>
        //----------------------------------------------------------------------------------------//
        
        private int GeometricBackoff(int n)
        {
            double sqrtOfFive = Math.Sqrt(5);

            return (int) (
                (Math.Pow((1+sqrtOfFive), n) - Math.Pow((1-sqrtOfFive), n)) /
                    (Math.Pow(2, n) * sqrtOfFive)
            ) * 100;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The maximum number of resubscription attempts to perform before generating an error.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The minimum value is 1 and the maximum value is 9.
        /// </para>
        /// <para>
        /// There's a wait period between each resubscription event. The maximum wait on the last
        /// iteration try is calculated from this value.
        /// </para>
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        [IntegerValidator(MinValue=1, MaxValue=9)]
        protected int MaxResubscriptionAttempts { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Delegate defining the handler signature for unexpected session termination events.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private delegate void UnexpectedSessionTerminationEventHandler(
            object state, EventArgs eventArgs);

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Event for handling unexpected session termination events.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private event UnexpectedSessionTerminationEventHandler UnexpectedSessionTermination;

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Handler for unexpected session termination events.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The event handler unregisters itself to prevent recursion during communication state
        /// transitions. Consider the scenario where the publisher is down for the count. When
        /// we attempt to perform the resubscription we by virtue of the request attempt to 
        /// reestablish a connection. If this fails, the communication object state transitions
        /// to closed. This subsequently calls the OnClosed event. The same event that got us
        /// here in the first place; thus recursion.
        /// </para>
        /// <para>
        /// See "Understanding State Changes" http://msdn.microsoft.com/en-us/library/ms789041.aspx
        /// for a detailed discussion of WCF state transitions.
        /// </para>
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        private void OnUnexpectedSessionTermination(object state, EventArgs eventArgs)
        {
            if (!ClientInitiatedClose)
            {
                lock (_onUnexpectedSessionTerminationSync)
                    UnexpectedSessionTermination -= OnUnexpectedSessionTermination;
                
                Resubscribe();
                
                lock(_onUnexpectedSessionTerminationSync)
                    UnexpectedSessionTermination += OnUnexpectedSessionTermination;
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// When a session with the publisher unexpectedly terminates this handler attempts to
        /// resubscribe to current subscriptions with another publisher instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// There are a number of reasons for session termination. The session may terminate with
        /// a given publisher due to some unhandled fault. In this case, the publisher may likely
        /// be available to establish a new connection. Alternatively, the publisher instance may
        /// suddenly abort, thus terminating all sessions it maintained. In this case, the logic
        /// does not change, but it assumes that multiple instances of the publication service
        /// exist. This is a reasonable assumption given that most modern service environments 
        /// employ some fault tolerance mechanism. In either case, if there's no publisher 
        /// available after some number of reconnection attempts an error is generated.
        /// </para>
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        private void Resubscribe()
        {
            lock (_sync)
            {                    
                if (HasSubscriptions)
                {
                    for (int i=0; i<MaxResubscriptionAttempts; ++i)
                    {
                        Thread.Sleep(GeometricBackoff(2*i));
                        
                        try
                        {
                            foreach (KeyValuePair<string, Action> kvp in _subscriptionActions)
                                kvp.Value();

                            if (ProxyState == CommunicationState.Opened)
                                break;
                        }
                        catch
                        {
                            if (i == (MaxResubscriptionAttempts-1))
                            {
                               throw new CommunicationException(
                                    "Failed to resubscribe to publications after an unexpected "+
                                    "session termination with the publisher.");
                            }
                        }                        
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        // protected interfaces
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Routes a subscription request for a particular publication provided by the publisher       
        /// and registers the endpoint subsriber interested in the publication.
        /// </summary>
        /// <remarks>
        /// If already subscribed to a particular publication a subsequent request to the publisher
        /// is not generated; the publication endpoint event handler is added to the list of end-
        /// points interested in the publication.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        protected void Subscribe(
            string subscriptionToken,
            Action subscribeAction,
            PublicationEventHandler publicationEndpointEventHandler)
        {
            Debug.Assert(subscriptionToken != null);
            Debug.Assert(subscribeAction != null);
            Debug.Assert(publicationEndpointEventHandler != null);

            lock (_sync)
            {
                if (!_subscriptions.ContainsKey(subscriptionToken))
                {
                    subscribeAction();
                    _subscriptions.Add(subscriptionToken, publicationEndpointEventHandler);
                    _subscriptionActions.Add(subscriptionToken, subscribeAction);
                    
                }
                else  _subscriptions[subscriptionToken] += publicationEndpointEventHandler;
            }            
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Removes the publication endpoint event handler from the list of event handlers 
        /// interested in the specified publication. If no endpoint subscribes exist, a cancel
        /// subscription request is routed to the publisher to stop the transmission of data
        /// for that publication.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected void CancelSubscription(
            string subscriptionToken,
            Action cancelSubscriptionAction,
            PublicationEventHandler publicationEndpointEventHandler)
        {
            Debug.Assert(subscriptionToken != null);
            Debug.Assert(cancelSubscriptionAction != null);
            Debug.Assert(publicationEndpointEventHandler != null);

            lock (_sync)
            {
                if (_subscriptions.ContainsKey(subscriptionToken))
                {
                    _subscriptions[subscriptionToken] -= publicationEndpointEventHandler;

                    if (_subscriptions[subscriptionToken] == null)
                    {
                        _subscriptions.Remove(subscriptionToken);
                        _subscriptionActions.Remove(subscriptionToken);
                        cancelSubscriptionAction();
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Removes all publication endpoint event handlers for a particular publication and
        /// routes a request to the publisher to stop the transmission of data for that 
        /// publicaiton.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected void CancelAllSubscriptions(
            string subscriptionToken,
            Action cancelSubscriptionAction)
        {
            Debug.Assert(subscriptionToken != null);
            Debug.Assert(cancelSubscriptionAction != null);            

            lock (_sync)
            {
                if (_subscriptions.ContainsKey(subscriptionToken))
                {
                    _subscriptions.Remove(subscriptionToken);
                    _subscriptionActions.Remove(subscriptionToken);
                    cancelSubscriptionAction();
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Forwards the publication data received from the publisher to the set of endpoint 
        /// handlers.
        /// </summary>
        //----------------------------------------------------------------------------------------//
        
        protected void Publish(string subscriptionToken, PublicationEventArgs eventArgs)
        {
            Debug.Assert(subscriptionToken != null);

            lock (_sync)
            {
                if (_subscriptions.ContainsKey(subscriptionToken))
                {
                    _subscriptions[subscriptionToken](this, eventArgs);
                }
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
