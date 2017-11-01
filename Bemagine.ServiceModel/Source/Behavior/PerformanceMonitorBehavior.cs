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
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Description;

    using Bemagine.ServiceModel.Monitoring;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The PerformanceMonitorBehavior is an endpoint behavior that installs an instance of a
    /// IServicePerformanceCounters dispatch message inspector.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class PerformanceMonitorBehavior : IEndpointBehavior
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly IServicePerformanceCounters _dispatchMessageInspector;

        //----------------------------------------------------------------------------------------//
        // IEndpointBehavior interface implementation
        //----------------------------------------------------------------------------------------//

        public void AddBindingParameters(
          ServiceEndpoint endpoint, 
          BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(
          ServiceEndpoint endpoint, 
          ClientRuntime clientRuntime)
        {
            throw new Exception("Behavior not supported on the consumer side!");
        }

        public void ApplyDispatchBehavior(
          ServiceEndpoint endpoint, 
          EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(_dispatchMessageInspector);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public PerformanceMonitorBehavior(IServicePerformanceCounters dispatchMessageInspector)
        {
            if (dispatchMessageInspector == null)
                throw new ArgumentNullException("dispatchMessageInspector");

            _dispatchMessageInspector = dispatchMessageInspector;
        }
    };
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
