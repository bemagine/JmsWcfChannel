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

namespace Bemagine.ServiceModel.Monitoring
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Diagnostics;
    using System.ServiceModel.Dispatcher;

    using System.ServiceModel;
    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Defines the basic interfaces for service performance counters.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public interface IServicePerformanceCounters : IDispatchMessageInspector
    {
        PerformanceCounter RequestRate { get; }
        PerformanceCounter TotalRequests { get; }
        PerformanceCounter TotalFailures { get; }
        PerformanceCounter TotalCommunicationErrors { get; }

        void IncrementRequests();
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// ServicePerformanceCounters implements IServicePerformanceCounters and creates the basic 
    /// set of service related performance counters.
    /// </summary>
    /// <remarks>
    /// By no means is this implementation exhaustive, and this class should be extended to add
    /// additional counters. It is provided to normalize code using WCF behaviors and interceptors
    /// that perform basic error handling. These classes provide a basic set of functionality that
    /// most WCF services require. Special cases require extension. This can be implemented by
    /// specifying both the OnCreateCounters and OnRegisterCounters delegates. Generally speaking,
    /// the methods for incrementing the general purpose counters are encapsulated in the frame-
    /// work, and should not be called by other mechanisms. (See the PerformanceMonitorBehavior
    /// and ServiceErrorHandler classes for more details). Therefore, extension of this class 
    /// requires encapsulating an instance of this class in an encapsulating class and exposing
    /// only counters of the wrapper class. This will prevent double counting situations.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    public sealed class ServicePerformanceCounters : IServicePerformanceCounters
    {
        //---------------------------------------------------------------------------------------//
        // Performance counters properties
        //---------------------------------------------------------------------------------------//

        public PerformanceCounter RequestRate { get; private set; }
        public PerformanceCounter TotalRequests { get; private set; }
        public PerformanceCounter TotalFailures { get; private set; }
        public PerformanceCounter TotalCommunicationErrors { get; private set; }        

        public string CategoryName { get; private set; }
        public string ServiceName { get; private set; }
        public string Description { get; private set; }

        public string RequestRateCounterName { get; private set; }
        public string TotalRequestsCounterName { get; private set; }
        public string TotalFailuresCounterName { get; private set; }
        public string TotalCommunicationErrorsCounterName { get; private set; }

        //----------------------------------------------------------------------------------------//
        // delegates
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Register an OnRegisterCounters delegate to have custom counters registered.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public delegate void OnRegisterCountersHandler(CounterCreationDataCollection counters);
        public OnRegisterCountersHandler OnRegisterCounters;

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Register an OnCreateCounters to create custom counters.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public delegate void OnCreateCountersHandler();
        public OnCreateCountersHandler OnCreateCounters;

        //----------------------------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//

        public void IncrementRequests()
        {
            RequestRate.Increment();
            TotalRequests.Increment();
        }

        //----------------------------------------------------------------------------------------//
        // private creational methods
        //----------------------------------------------------------------------------------------//

        private void CreatePerformanceCategory()
        {
            var counters = new CounterCreationDataCollection();

            //------------------------------------------------------------------------------------//
            // Total number of serialization requests
            //------------------------------------------------------------------------------------//

            var totalRequestsCounterData =
              new CounterCreationData(
                TotalRequestsCounterName, 
                String.Format(
                    "A cumulative counter of the number of {0} requests made of the service "+
                    "since the start of the service. Resets when the server instance is restarted.",
                    ServiceName),
                PerformanceCounterType.NumberOfItems32);

            counters.Add(totalRequestsCounterData);

            //------------------------------------------------------------------------------------//
            // Total number of serialization failures
            //------------------------------------------------------------------------------------//

            var totalFailuresCounterData =
                new CounterCreationData(
                    TotalFailuresCounterName, 
                    String.Format(
                        "A cumulative counter of the number of {0} request failures since the "+
                        "start of the service. Resets when the server instance is restarted. "+
                        "Tracks service request failures, and not general system failures.", 
                        ServiceName),
                    PerformanceCounterType.NumberOfItems32);

            counters.Add(totalFailuresCounterData);

            //------------------------------------------------------------------------------------//
            // Serialization requests per second
            //------------------------------------------------------------------------------------//

            var requestsPerSecondCounterData =
                new CounterCreationData(
                    RequestRateCounterName,
                    String.Format(
                        "A rate counter of the number of {0} requests made of the service in "+
                        "given second interval.", ServiceName),
                    PerformanceCounterType.RateOfCountsPerSecond32);

            counters.Add(requestsPerSecondCounterData);

            //------------------------------------------------------------------------------------//
            // Total number of communication errors
            //------------------------------------------------------------------------------------//

            var totalCommunicationErrorsCounterData =
                new CounterCreationData(
                    TotalCommunicationErrorsCounterName,
                    "A cumulative counter of the number of service communication failures since "+
                    "the start of the service. Resets when the server instance is restarted. "+
                    "Communication errors indicate a failure to receive or send data over the wire.",
                    PerformanceCounterType.NumberOfItems32);

            counters.Add(totalCommunicationErrorsCounterData);

            //------------------------------------------------------------------------------------//
            // Add any additional custom counters
            //------------------------------------------------------------------------------------//

            if (OnRegisterCounters != null)
                OnRegisterCounters(counters);

            //------------------------------------------------------------------------------------//
            // Remove existing performance category and add the new
            //------------------------------------------------------------------------------------//

            if (PerformanceCounterCategory.Exists(CategoryName))
              PerformanceCounterCategory.Delete(CategoryName);

            PerformanceCounterCategory.Create(
              CategoryName, 
              Description,
              PerformanceCounterCategoryType.SingleInstance, 
              counters);
        }

        private void CreatePerformanceCounters()
        {
            RequestRate = new PerformanceCounter(CategoryName, RequestRateCounterName, false);
            TotalRequests = new PerformanceCounter(CategoryName, TotalRequestsCounterName, false);
            TotalFailures = new PerformanceCounter(CategoryName, TotalFailuresCounterName, false);

            TotalCommunicationErrors = 
              new PerformanceCounter(CategoryName, TotalCommunicationErrorsCounterName, false);

            if (OnCreateCounters != null)
                OnCreateCounters();
        }

        //----------------------------------------------------------------------------------------//
        // IDispatchMessageInspector implementation
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Increments the concurrent call counter, and resets the wait handle if this value is
        /// greater than or equal to the max allowed concurrent calls on the endpoint. 
        /// </summary>
        /// <remarks>
        /// The DispatchRuntime calls this method on receipt of a new message. This indicates the
        /// start of a new operation.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, 
            InstanceContext instanceContext)
        {
            IncrementRequests();
            return null;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Decrements the concurrent call counter, and sets the wait handle if this value is
        /// less than the max allowed concurrent calls on the endpoint.
        /// </summary>
        /// <remarks>
        /// The DispatchRuntime calls this method on after an operation completes regardless if
        /// the OperationContract indicates a one-way call. Just to note, in the case of a one-way
        /// call the reply message will be null.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public ServicePerformanceCounters(string categoryName, string serviceName, string description)
        {
            //-----------------------------------------------------------------------------------//
            // Argument validation
            //-----------------------------------------------------------------------------------//

            if (categoryName == null)
                throw new ArgumentNullException("categoryName");

            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            if (description == null)
                throw new ArgumentNullException("description");

            //-----------------------------------------------------------------------------------//
            // Local properties initialization 
            //-----------------------------------------------------------------------------------//

            CategoryName = categoryName;
            ServiceName = serviceName;
            Description = description;

            RequestRateCounterName = String.Format("{0} requests / second", ServiceName);
            TotalRequestsCounterName = String.Format("# of {0} requests", ServiceName);
            TotalFailuresCounterName = String.Format("# of {0} failures", ServiceName);
            TotalCommunicationErrorsCounterName = "# of communication errors";

            //-----------------------------------------------------------------------------------//
            // Performance counters creation
            //-----------------------------------------------------------------------------------//
            
            CreatePerformanceCategory();
            CreatePerformanceCounters();
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
