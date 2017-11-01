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
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    // aliases
    //--------------------------------------------------------------------------------------------//

    using OnMessageAction = System.Action<Bemagine.ServiceModel.Jms.IMessage>;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Manages the dispatch mechanism for messages across one or more consumers via the round robin
    /// load balancing algorithm (i.e. messages are dispatched to each consumer in turn as messages
    /// arrive).
    /// </summary>
    /// <remarks>
    /// Thread synchronization is not implemented in this class. Therefore, proper measures are 
    /// required by the users of this class to provide thread synchronization.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal sealed class RoundRobinMessageDispatch
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly LinkedList<OnMessageAction> _consumerRegistrations = 
            new LinkedList<OnMessageAction>();

        //----------------------------------------------------------------------------------------//
        // member properties - to facilitate unit testing
        //----------------------------------------------------------------------------------------//

        public int NumberOfConsumerRegistrations
        {
            get { return _consumerRegistrations.Count; }
        }

        //----------------------------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Dispatches the message to the next registered consumer (i.e. round robin).
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void DispatchMessage(IMessage message)
        {
            OnMessageAction onMessageAction = _consumerRegistrations.First.Value;
            onMessageAction(message);

            _consumerRegistrations.RemoveFirst();
            _consumerRegistrations.AddLast(onMessageAction);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Adds the consumer's OnMessageAction callback to the collection of consumer callbacks.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void RegisterConsumer(OnMessageAction onMessageAction)
        {
            _consumerRegistrations.AddLast(onMessageAction);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
