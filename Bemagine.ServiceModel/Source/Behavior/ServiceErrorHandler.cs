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

namespace Bemagine.ServiceModel.Behavior
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Collections.ObjectModel;

    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Description;
    
    using Bemagine.ServiceModel.Monitoring;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The ServiceErrorHandler provides basic WCF error handling and performance monitoring
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public class ServiceErrorHandler : IErrorHandler, IServiceBehavior
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly IServicePerformanceCounters _performanceCounters;

        //----------------------------------------------------------------------------------------//
        // delegates
        //----------------------------------------------------------------------------------------//

        public delegate void OnErrorHandler(Exception e);
        private readonly OnErrorHandler _onError;

        //----------------------------------------------------------------------------------------//
        // IServiceBehavior interface
        //----------------------------------------------------------------------------------------//

        public void AddBindingParameters(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints, 
            BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
        }

        public void ApplyDispatchBehavior(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
            foreach(ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                channelDispatcher.ErrorHandlers.Add(this); 
            }
        }

        //----------------------------------------------------------------------------------------//
        // IErrorHandler interface
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Specifies error handling semantics and returns a boolean value to the dispatcher that
        /// indicates if the session and possibly the instance context should be aborted.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public bool HandleError(Exception error)
        {
            Type errorType = error.GetType();

            if (errorType == typeof(CommunicationException))
            {
                _performanceCounters.TotalCommunicationErrors.Increment();
                _onError(error);
            }

            else
            {      
                _performanceCounters.TotalFailures.Increment();
                _onError(error);
            }

            // Returning true indicates that the error has been handled and that the the dispatcher
            // should NOT abort the session or instance context.
            return true;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Provides the mechanism to create a custom fault message that is returned to the client.
        /// If the fault message is null the default fault message is returned.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public virtual void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            //------------------------------------------------------------------------------------//
            // The following is an example of how to generate custom fault messages. This needs 
            // some consideration as the messages sent back to the client must be understandable 
            // (this means cutting through the techno-jargon.
            //------------------------------------------------------------------------------------//

            //if (error.GetType() != typeof(CommunicationException))
            //{
            //    FaultException faultException = 
            //        new FaultException(
            //            String.Format(
            //                "An error occurred while transforming your data to an appropriate client format. "+
            //                "Here are the technical details: {0}. Please provide these details to the support "+
            //                "team."));

            //    MessageFault messageFault = faultException.CreateMessageFault();	  
            //    fault = Message.CreateMessage(version, messageFault, faultException.Action);
            //}
        }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public ServiceErrorHandler(
            IServicePerformanceCounters performanceCounters,
            OnErrorHandler onError)
        {
            if (performanceCounters == null)
                throw new ArgumentNullException("performanceCounters");

            if (onError == null)
                throw new ArgumentNullException("onError");

            _performanceCounters = performanceCounters;
            _onError = onError;
        }
    }

}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
