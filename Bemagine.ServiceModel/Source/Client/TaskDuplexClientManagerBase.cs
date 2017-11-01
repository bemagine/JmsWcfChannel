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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using System.Diagnostics;
    using System.ServiceModel;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The TaskDuplexClientManagerBase establishes a concrete pattern for designing and interacting
    /// with duplex services using TPL tasks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The service interaction model requires that some context or correlation ID be provided as
    /// an argument to request invocations and reponses to the initial requests respond with that
    /// ID. This correlation ID is subsequently used to track and transition the state of the 
    /// request as modeled by a TPL task.
    /// </para>
    /// <para>
    /// Given that this class specifies a pattern for designing and interacting with a service it
    /// seems reasonable not to implement this behavior in DuplexClientManagerBase. This allows
    /// the DuplexClientManagerBase to be used in many different duplex communication scenarios.
    /// </para>
    /// </remarks>
    /// <typeparam name="IServiceContractT">
    /// A class type that represents the contract of the service.
    /// </typeparam>
    //--------------------------------------------------------------------------------------------//

    public abstract class TaskDuplexClientManagerBase<IServiceContractT> :
        DuplexClientManagerBase<IServiceContractT> where IServiceContractT : class
    {

        //----------------------------------------------------------------------------------------//
        // nested types
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Represents the final states of the response / task can transition to.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private enum TaskTransitionState
        {
            Completed, Faulted, Canceled
        }

        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private int _correlationCount;
        private readonly object _serviceEventSync = new object();
        private readonly string _machineName = Environment.MachineName;
        private readonly object _reusableNoStateObject = new object();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private readonly Dictionary<string, Action<object,TaskTransitionState>> _transitioners =
            new Dictionary<string,Action<object, TaskTransitionState>>();

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        protected TaskDuplexClientManagerBase(string endpointConfigurationName) 
            : base(endpointConfigurationName)
        {
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Generates a unique correlation ID used to correlate requests with responses and 
        /// transition the state of the request modeling task.
        /// </summary>
        /// <returns>
        /// A correlation ID in the format of {MachineName}-{CorrelationCount}-{GUID}.
        /// </returns>
        //----------------------------------------------------------------------------------------//

        protected string GenerateCorrelationId()
        {
            int correlationCount = Interlocked.Increment(ref _correlationCount);
            return String.Format("{0}-{1}-{2}", _machineName, correlationCount, Guid.NewGuid());
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Caches a task state transitioner action by its associative correlation ID in the
        /// transitioners cache.
        /// </summary>
        /// <param name="correlationId">
        /// The correlation ID that associates the request task and response.
        /// </param>
        /// <param name="transitioner">
        /// The action that will perform the final state transition of the task when either a
        /// response is received or an internal client fault generated.
        /// </param>
        //----------------------------------------------------------------------------------------//

        private void CacheStateTransitioner(
            string correlationId, Action<object, TaskTransitionState> transitioner)
        {
            lock (_serviceEventSync) _transitioners.Add(correlationId, transitioner);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Removes the reqquest task state tranisitioner action associated with the specified 
        /// correlation ID from the transitioners cache.
        /// </summary>
        /// <param name="correlationId">
        /// The correlation ID that associates the request, response, and modeling task.
        /// </param>
        //----------------------------------------------------------------------------------------//

        private void RemoveCachedTaskStateTransitioner(string correlationId)
        {
            if (_transitioners.ContainsKey(correlationId))
                _transitioners.Remove(correlationId);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// A utility method to normalize task state transitioning.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the state parameter.
        /// </typeparam>
        /// <param name="correlationId">
        /// The correlation ID that associates the request, response, and modeling task.
        /// </param>
        /// <param name="state">
        /// The state represents the final result of the operation. In the case of task completion
        /// this represents the response result, an exception if faulted, or a placeholder object
        /// if cancellation.
        /// </param>
        /// <param name="transitionState">
        /// The final state to transition the request task either when a response is generated or
        /// an internal client manager fault encountered.
        /// </param>
        //----------------------------------------------------------------------------------------//

        private void TransitionTaskCompletionSourceState<T>(
            string correlationId,
            T state,
            TaskTransitionState transitionState)
        {
            lock (_serviceEventSync)
            {
                Action<object, TaskTransitionState> transitioner;
                if (_transitioners.TryGetValue(correlationId, out transitioner))
                {
                    transitioner(state, transitionState);
                    RemoveCachedTaskStateTransitioner(correlationId);
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the request task associated with the correlation ID for requests that 
        /// expects a response result to the completed state. 
        /// </summary>
        /// <typeparam name="T">
        /// The type of the response result.
        /// </typeparam>
        /// <param name="correlationId">
        /// The correlation ID that associates the request task and response.
        /// </param>
        /// <param name="result">
        /// The response result of a successful service request.
        /// </param>
        //----------------------------------------------------------------------------------------//

        protected void CompleteRequest<T>(string correlationId, T result)
        {
            TransitionTaskCompletionSourceState(
                correlationId, result, TaskTransitionState.Completed);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the request task associated with the correlation ID for requests that 
        /// expects an empty response to the completed state. 
        /// </summary>
        /// <param name="correlationId">
        /// The correlation ID that associates the request task and response.
        /// </param>
        //----------------------------------------------------------------------------------------//

        protected void CompleteRequest(string correlationId)
        {
            TransitionTaskCompletionSourceState(
                correlationId, _reusableNoStateObject, TaskTransitionState.Completed);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the request task associated with the correlation ID to the faulted state
        /// when either a failure to produce a result by the service or an internal client manager
        /// exception is encountered.
        /// </summary>
        /// <param name="correlationId">
        /// The correlation ID that associates the request task and response.
        /// </param>
        /// <param name="faultMessage">
        /// The error message that describes the fault.
        /// </param>
        /// <remarks>
        /// Provides an Exception type agnostic mechanism for faulting a request task. Duplex
        /// services that implement one-way calls (as modeled by this class) generally will
        /// respond with an error message. Hence, this overload to address that use case.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        protected void FaultRequest(string correlationId, string faultMessage)
        {
            FaultRequest(correlationId, new Exception(faultMessage));
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the request task associated with the correlation ID to the faulted state
        /// when either a failure to produce a result by the service or an internal client manager
        /// exception is encountered.
        /// </summary>
        /// <param name="correlationId">
        /// The correlation ID that associates the request task and response.
        /// </param>
        /// <param name="fault">
        /// The exception that describes the fault.
        /// </param>
        //----------------------------------------------------------------------------------------//

        protected void FaultRequest(string correlationId, Exception fault)
        {
            TransitionTaskCompletionSourceState(correlationId, fault, TaskTransitionState.Faulted);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the request task to the canceled state.
        /// </summary>
        /// <remarks>
        /// The task associated with the request is canceled, and not any oustanding work to be
        /// performed by the service. A cancelation protocol is required to propagate cancelation
        /// requests to the service.
        /// </remarks>
        /// <param name="correlationId">
        /// The correlation ID that associates the request task and response.
        /// </param>
        //----------------------------------------------------------------------------------------//

        protected void CancelRequest(string correlationId)
        {
            TransitionTaskCompletionSourceState(
                correlationId, _reusableNoStateObject, TaskTransitionState.Canceled);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Dispatches requests to the service in an exception safe manner; exceptions are handled
        /// and an error response dispatched via the resultant task. (Assuming that the task
        /// contruction does not explode in our faces).
        /// </summary>
        /// <remarks>
        /// This overload does not expect a correlation ID given that it is internally generated.
        /// </remarks>
        /// <typeparam name="T">
        /// The type of the response data.
        /// </typeparam>
        /// <param name="requestAction">
        /// The action that takes a correlation ID as an argument and appropriately proxys the
        /// request to the service. This level encapsulation enables this class to be request
        /// agnostic and focus on request task state maintanence.
        /// </param>
        /// <returns>
        /// A task that represents a request that expects a non-empty service response.
        /// </returns>
        //----------------------------------------------------------------------------------------//

        protected Task<T> DispatchRequest<T>(Action<string> requestAction)
        {
            var correlationId = GenerateCorrelationId();
            return DispatchRequest<T>(correlationId, requestAction);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Dispatches requests to the service in an exception safe manner; exceptions are handled
        /// and an error response dispatched via the resultant task. (Assuming that the task
        /// contruction does not explode in our faces).
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload version expects a non-null correlation ID. Initially, the thought was
        /// to completely encapsulate correlation ID generation, but this does not serve facade
        /// services well (i.e. services that forward a call to another service that might not be
        /// another WCF service). In providing this overload the correlation ID of the original
        /// request may be preserved through the call chain. Technically, this is more of a 
        /// support and debug convenience, but I felt that this simplified message tracing
        /// trumps the encapsulation argument. 
        /// </para>
        /// <para>
        /// Exceptions could be allowed to bubble up from this method for the caller to handle.
        /// That would require the caller to specify two error handling mechanisms (1) for 
        /// generated exceptions and (2) errors generated post dispatch (i.e. service faults).
        /// To simplify the interaction, all errors are through the task.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">
        /// The type of the response data.
        /// </typeparam>
        /// <param name="correlationId">
        /// The correlation ID that associates the request task and response.
        /// </param>
        /// <param name="requestAction">
        /// The action that takes a correlation ID as an argument and appropriately proxys the
        /// request to the service. This level encapsulation enables this class to be request
        /// agnostic and focus on request task state maintanence.
        /// </param>
        /// <returns>
        /// A task that represents a request that expects a non-empty service response.
        /// </returns>
        //----------------------------------------------------------------------------------------//

        protected Task<T> DispatchRequest<T>(string correlationId, Action<string> requestAction)
        {
            Debug.Assert(correlationId != null, "correlationId must not be null.");
            Debug.Assert(requestAction != null, "requestAction must not be null.");

            var taskCompletionSource = new TaskCompletionSource<T>();

            try
            {
                //--------------------------------------------------------------------------------//
                //
                //--------------------------------------------------------------------------------//

                CacheStateTransitioner(
                    correlationId,
                    (state, transitionState) =>
                    {
                        switch (transitionState)
                        {
                            case TaskTransitionState.Completed:
                                taskCompletionSource.SetResult((T) state);
                                break;
                            case TaskTransitionState.Faulted:
                                taskCompletionSource.SetException((Exception) state);
                                break;
                            case TaskTransitionState.Canceled:
                                taskCompletionSource.SetCanceled();
                                break;
                        }
                    }
                );

                requestAction(correlationId);
            }

            //------------------------------------------------------------------------------------//
            // error handling
            //------------------------------------------------------------------------------------//

            catch (CommunicationException e)
            {
                var errorMessage =
                    String.Format(
                        "Failed to performed the request [{0}] due to a communucation error. "+
                        "-- Error Message {1}.",
                        (new StackTrace()).GetFrame(1).GetMethod().Name, e.Message);

                FaultRequest(correlationId, new CommunicationException(errorMessage, e));
            }
            catch (Exception e)
            {
                var errorMessage =
                    String.Format(
                        "Failed to performed the request [{0}] due to an unspecified error. " +
                        "-- Error Message {1}.",
                        (new StackTrace()).GetFrame(1).GetMethod().Name, e.Message);

                FaultRequest(correlationId, new Exception(errorMessage, e));
            }

            return taskCompletionSource.Task;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Dispatches requests to the service in an exception safe manner; exceptions are handled
        /// and an error response dispatched via the resultant task. (Assuming that the task
        /// contruction does not explode in our faces).
        /// </summary>
        /// <param name="correlationId">
        /// The correlation ID that associates the request task and response.
        /// </param>
        /// <param name="requestAction">
        /// The action that takes a correlation ID as an argument and appropriately proxys the
        /// request to the service. This level encapsulation enables this class to be request
        /// agnostic and focus on request task state maintanence.
        /// </param>
        /// <returns>
        /// A task that represents a request that expects a empty service response. In this case,
        /// the response generally indicates whether the request completed successfully on the
        /// service. Consider these acknowledgement responses.
        /// </returns>
        //----------------------------------------------------------------------------------------//

        protected Task DispatchRequest(Action<string> requestAction)
        {
            var correlationId = GenerateCorrelationId();
            return DispatchRequest<object>(correlationId, requestAction);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Dispatches requests to the service in an exception safe manner; exceptions are handled
        /// and an error response dispatched via the resultant task. (Assuming that the task
        /// contruction does not explode in our faces).
        /// </summary>
        /// <param name="correlationId">
        /// The correlation ID that associates the request task and response.
        /// </param>
        /// <param name="requestAction">
        /// The action that takes a correlation ID as an argument and appropriately proxys the
        /// request to the service. This level encapsulation enables this class to be request
        /// agnostic and focus on request task state maintanence.
        /// </param>
        /// <returns>
        /// A task that represents a request that expects a empty service response. In this case,
        /// the response generally indicates whether the request completed successfully on the
        /// service. Consider these acknowledgement responses.
        /// </returns>
        //----------------------------------------------------------------------------------------//

        protected Task DispatchRequest(string correlationId, Action<string> requestAction)
        {
            return DispatchRequest<object>(correlationId, requestAction);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
