//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Channels
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Diagnostics;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Description;

    //--------------------------------------------------------------------------------------------//
    // using aliases
    //--------------------------------------------------------------------------------------------//

    using QueueThrottleSetterProperty = System.Action<QueueThrottle>;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The QueueThrottlingBehavior is an endpoint behavior provided to support service side queue
    /// throttling (draining). It installs the QueueThrottle class as an endpoint dispatch message 
    /// inspector.
    /// </summary>
    /// <remarks>
    /// The QueueThrottlingBehavior applys only to services, so why implement it as an endpoint
    /// behavior? The principal tenet of the queue throttling behavior is to throttle queue 
    /// draining by a listener (endpoint URI). Installing a queue throttling as a service behavior
    /// would make it difficult to know which endpoint to apply the behavior to. It is clear that
    /// this behavior applies to a very specific case. Specifically, to throttle queue draining
    /// by the custom JMS queue listener/channel.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    public sealed class QueueThrottlingBehavior : Attribute, IEndpointBehavior
    {
        //----------------------------------------------------------------------------------------//
        /// <remarks>
        /// Provides the ability to pass custom data to binding elements to support the endpoint 
        /// configuration.
        /// </remarks>
        //----------------------------------------------------------------------------------------// 

        public void AddBindingParameters(ServiceEndpoint endpoint, 
            BindingParameterCollection bindingParameters)
        {            
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Throws an InvalidOperationException as queue throttling is no a behavior supported
        /// by client endpoints.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void ApplyClientBehavior(ServiceEndpoint endpoint, 
            ClientRuntime clientRuntime)
        {
            throw new InvalidOperationException(
                "QueueThrottling behavior cannot be applied to client endpoints.");
        }
    
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Adds a new instance of the QueueThrottle dispatch message inspector to the endpoint's
        /// dispatch runtime message inspectors list.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, 
            EndpointDispatcher endpointDispatcher)
        {
            var throttle = new QueueThrottle(MaxConcurrentEndpointCalls);

            IChannelListener listener = endpointDispatcher.ChannelDispatcher.Listener;
            Debug.Assert(listener != null, "listener != null");

            //------------------------------------------------------------------------------------//
            // The QueueThrottle dispatch message inspector might monitor the inflow and outflow
            // of messages from an endpoint, but the manager needs to synchronize with that 
            // QueueThrottle instance. So, here we set throttle member of the instance of the
            // IJmsQueueChannelManager associated with the endpoint. This also has the side effect
            // of ensuring that the behavior applies to the current listening context.
            //------------------------------------------------------------------------------------//            

            var queueThrottleSetterProperty = listener.GetProperty<QueueThrottleSetterProperty>();

            if (queueThrottleSetterProperty != null)
                queueThrottleSetterProperty(throttle);

            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(throttle);
        }        

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Inspects the service endpoint to verify if the behavior is appropriate for the 
        /// specified endpoint scheme.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void Validate(ServiceEndpoint endpoint)
        {       
            if (endpoint.ListenUri.Scheme != BindingPropertyDefaults.Scheme)
            {
                throw new InvalidOperationException( 
                    string.Format(
                        "The QueueThrottling behavior is not valid for endpoint the scheme {0}. " +
                        "Verify the endpoint scheme is intended. If not set the scheme to {1} to " +
                        "apply this behavior.", endpoint.ListenUri.Scheme, 
                        BindingPropertyDefaults.Scheme));
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the maximum number of concurrent calls executing for a given endpoint. 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public int MaxConcurrentEndpointCalls { get; set; }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
