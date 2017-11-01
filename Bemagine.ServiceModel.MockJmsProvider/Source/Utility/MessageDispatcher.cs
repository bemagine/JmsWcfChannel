//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.MockJmsProvider.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.MockJmsProvider
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using Bemagine.ServiceModel.Jms;

    using System;
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    // aliases
    //--------------------------------------------------------------------------------------------//

    using OnMessageAction = System.Action<Bemagine.ServiceModel.Jms.IMessage>;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Internal message dispatch provider that simulates message transmission over the wire (i.e.
    /// messages sent by the producer to a JMS destination are routed to the appropriate consumer 
    /// of that JMS destination).
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class MessageDispatcher
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly object _thisLock = new object();
        
        private readonly Dictionary<string, RoundRobinMessageDispatch> _roundRobinDispatchers =
            new Dictionary<string, RoundRobinMessageDispatch>();

        //----------------------------------------------------------------------------------------//
        // member properties - to facilitate unit testing
        //----------------------------------------------------------------------------------------//

        public int NumberOfDestinationRegistrations
        {
            get { return _roundRobinDispatchers.Count; }
        }

        public int NumberOfConsumerRegistrations
        {
            get
            {
                int count = 0;

                foreach (var roundRobinMessageDispatch in _roundRobinDispatchers)
                    count += roundRobinMessageDispatch.Value.NumberOfConsumerRegistrations;

                return count;
            }
        }
        
        //----------------------------------------------------------------------------------------//
        // singleton implementation
        //----------------------------------------------------------------------------------------//

        private static readonly Lazy<MessageDispatcher> _instance = 
            new Lazy<MessageDispatcher>(() => new MessageDispatcher());

        public static MessageDispatcher Instance { get { return _instance.Value; } }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        private MessageDispatcher()
        {
        }

        //----------------------------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//

        public void ClearAllRegistrations()
        {
            lock (_thisLock) _roundRobinDispatchers.Clear();
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Dispatches the message to the next consumer of the destination in turn as governed by
        /// the round robin dispatch / load balancing algorithm.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void DispatchMessage(IDestination destination, IMessage message)
        {
            lock (_thisLock)
            {
                RoundRobinMessageDispatch rrDispatch;
                if (_roundRobinDispatchers.TryGetValue(destination.Name, out rrDispatch))
                {
                    rrDispatch.DispatchMessage(message);
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Registers the consumer's onMessageAction with the dispatch infrastructure.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void RegisterConsumer(IDestination destination, OnMessageAction onMessageAction)
        {
            lock (_thisLock)
            {
                RoundRobinMessageDispatch rrDispatch;
                if (!_roundRobinDispatchers.TryGetValue(destination.Name, out rrDispatch))
                {
                    rrDispatch = new RoundRobinMessageDispatch();
                    _roundRobinDispatchers.Add(destination.Name, rrDispatch);
                }

                rrDispatch.RegisterConsumer(onMessageAction);
            }
        }
        
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
