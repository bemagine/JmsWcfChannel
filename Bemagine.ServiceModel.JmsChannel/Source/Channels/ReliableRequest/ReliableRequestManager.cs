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
    using System.Collections.Generic;
    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    // using aliases
    //--------------------------------------------------------------------------------------------//

    using ReliableRequestDictionary = 
        System.Collections.Generic.Dictionary<string, ReliableRequest>;

    using ProcessingServiceDictionary = 
        System.Collections.Generic.Dictionary<string, 
            System.Collections.Generic.Dictionary<string, ReliableRequest>>;

    using RequestInProcessDictionary = 
        System.Collections.Generic.Dictionary<string, string>;

    using ReliableRequestDictionaryKVP = 
        System.Collections.Generic.KeyValuePair<string, ReliableRequest>;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The ReliableRequestManager maintains and transitions the state of reliable requests. A 
    /// reliable request represents a service request that guarantees a level of fault tolerance 
    /// in the event a service instance goes down during message processing. It exists within the 
    /// broader Reliable Request protocol where requests transition from the queued to the 
    /// acknowledged and to either the faulted or dispatched states. In the queued state, requests 
    /// are queued in a central JMS queue awaiting processing by the next available service. On a
    /// dequeue event, the service acknowledges the receipt of the request. At that point the request 
    /// enters into a monitored state that detects when the server processing goes down. In this 
    /// faulted state the request may transition back to the queued state if the maximum number of 
    /// attempts has not been exceeded. In the case where the limit has been exceeded the request 
    /// generates an error response event and transitions to the dispatched state. The state diagram 
    /// below illustrates theses transitions.
    /// </summary>
    /// <remarks>
    /// The tracking of reliable requests only makes sense in the context of a requestor that 
    /// desires a level of fault tolerance in the requests made.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal sealed class ReliableRequestManager
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private static readonly ReliableRequestManager _instance = new ReliableRequestManager();
        private readonly object _instanceLock = new object();

        private readonly ReliableRequestDictionary _queued = new ReliableRequestDictionary();
        private readonly ProcessingServiceDictionary _acknowledged = new ProcessingServiceDictionary();
        private readonly RequestInProcessDictionary _processing = new RequestInProcessDictionary();

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        private ReliableRequestManager()
        {            
        }

        static ReliableRequestManager()
        {            
        }

        //----------------------------------------------------------------------------------------//
        // singleton interface
        //----------------------------------------------------------------------------------------//

        public static ReliableRequestManager Instance { get { return _instance; } }

        //----------------------------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//
    
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the ReliableRequest to the queued state. In this state, the request is 
        /// queued in a JMS provider queue awaiting processing by a service. Faults have no affect 
        /// on the state of messages in the queued state. ReliableRequests already in the 
        /// acknowledged state are ignored.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void Queued(ReliableRequest request)
        {
          //--------------------------------------------------------------------------------------//
          // ignore null requests
          //--------------------------------------------------------------------------------------//

          if (request != null)
          {
            lock (_instanceLock)
            {
                //--------------------------------------------------------------------------------//
                // Ignore requests that have already been acknowledged and are being processed.
                //--------------------------------------------------------------------------------//

                if ( _processing.ContainsKey(request.CorrelationId) )
                    return;

                //--------------------------------------------------------------------------------//
                // Ignore requests already in the queued state. Note, this check was not condensed
                // into the above check to provide clarity.
                //--------------------------------------------------------------------------------//

                if ( !_queued.ContainsKey(request.CorrelationId) )
                {
                    _queued.Add(request.CorrelationId, request);
                    LogUtility.Debug("[CorrelationId : {0}] Queued", request.CorrelationId);
                }
              }
          }
          else
          {
            LogUtility.Warning(
              "Attempted to transition a NULL ReliableRequest to the queued state. This really " +
              "should be an assertion, but a warning should suffice if this occurs.");
          }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the ReliableRequest specified by the correlationId to the acknowledged 
        /// state. Transitioning to this state occurs upon receipt of an acknowledgement message 
        /// from an service indicating that the service is executing the task. Transitioning to this 
        /// state from any other state than queued has no effect.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void Acknowledged(
          string processingServiceName,
          string processingServiceSession,
          string correlationId)
        {
            lock (_instanceLock)
            {
                string processingServiceKey = CreateProcessingServiceKey(
                    processingServiceName, processingServiceSession);

                //--------------------------------------------------------------------------------//
                // Determine if the message exists in the queued state. If not, we have to bail 
                // with an error messsage.
                //--------------------------------------------------------------------------------//

                ReliableRequest queuedRequest;
                if (_queued.TryGetValue(correlationId, out queuedRequest))
                {
                    //----------------------------------------------------------------------------//
                    // Determine if we have an entry for the service processing other requests. If 
                    // not, create the entry.
                    //----------------------------------------------------------------------------//

                    ReliableRequestDictionary serviceRequests;
                    if (!_acknowledged.TryGetValue(processingServiceKey, out serviceRequests))
                    {
                        serviceRequests = new Dictionary<string,ReliableRequest>();
                        _acknowledged.Add(processingServiceKey, serviceRequests);

                        LogUtility.Debug("[CorrelationId : {0}] Acknowledged [Service : {1}]", 
                            correlationId, processingServiceKey);
                    }

                    //----------------------------------------------------------------------------//
                    // Transition the ReliableRequest from the queued state to the acknowledged 
                    // state for the processing service. Duplicate ack's are ignored with a warning 
                    // message.
                    //----------------------------------------------------------------------------//

                    if (!serviceRequests.ContainsKey(correlationId))
                    {
                        serviceRequests.Add(correlationId, queuedRequest);
                    }
                    else
                    {
                      LogUtility.Warning(
                        "Duplicate transition to the acknowledged for the ReliableRequest "+
                        "[CorrelationId : {0} Service : {1}] is being ignored.", 
                        correlationId, processingServiceKey);
                    }

                  //------------------------------------------------------------------------------//
                  // Track the acknowledged ReliableRequest in the master processing
                  // list and remove it from the queue state map.
                  //------------------------------------------------------------------------------//

                  _processing[correlationId] = processingServiceKey;
                  _queued.Remove(correlationId);
                }
                else
                {
                  LogUtility.Warning(
                      "Attempt to transition a ReliableRequest to the acknowledged state failed "+
                      "because it does not exist int the queued state map. [CorrelationId : {0} "+ 
                      " Service : {1}].", correlationId, processingServiceKey);
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the reliable request to the faulted stated upon an service down event. This 
        /// is a transitory state that transitions either back to the queued state if the maximum 
        /// number of retries has NOT been exceeded; otherwise it transitions to the dispatched 
        /// state with an error response.
        /// </summary>
        //----------------------------------------------------------------------------------------//
        
        public void Faulted(
          string processingServiceName,
          string processingServiceSession)
        {
            lock (_instanceLock)
            {
                //--------------------------------------------------------------------------------//
                // Determine if there are any outstanding requests being processed by the service.
                //--------------------------------------------------------------------------------//

                string processingServiceKey = CreateProcessingServiceKey(
                    processingServiceName, processingServiceSession);

                ReliableRequestDictionary serviceRequests;
                if (_acknowledged.TryGetValue(processingServiceKey, out serviceRequests))
                {
                    //----------------------------------------------------------------------------//
                    // Iteraterate over the outstanding requests of the faulted service and
                    // resubmit for processing. Notice that there is no attempt here to
                    // determine if the maximum number of retries has been exceeded. This
                    // determination occurs in the ReliableRequest::send(). This has two
                    // benefits, (1) the implementation here is simplified, and (2) the
                    // ReliableRequest::send() method the proper handling of retry attempts
                    // at the lowest level possible.
                    //----------------------------------------------------------------------------//

                    LogUtility.Warning(
                        "Attempting to reschedule {0} outstanding requests being processed by the "+
                        "now faulted Service.", serviceRequests.Count, processingServiceKey);

                    foreach (ReliableRequestDictionaryKVP kvp in serviceRequests)
                    {
                        ReliableRequest request = kvp.Value;
                        Message message = request.Message;

                        //------------------------------------------------------------------------//
                        // Messages that fault were actively executing requests. The priority of 
                        // these messages are therefore set to the maximum priority to ensure 
                        // timely processing.
                        //------------------------------------------------------------------------//

                        message.AddHeader(Jms.MessagePriority.Highest);

                        _processing.Remove(request.CorrelationId);
                        ReliableRequest.Send(request);
                    }

                    //----------------------------------------------------------------------------//
                    // Remove the service from the map of service requests in the acknowledged state.
                    //----------------------------------------------------------------------------//

                    _acknowledged.Remove(processingServiceKey);
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the reliable request state from either the acknowledged state when a 
        /// response event occurs or the faulted state when a server down event occurs and the 
        /// max retries limit has been exceeded. This is generally an opportunity to release 
        /// resources maintained for the duration of the request.
        /// </summary>
        /// <remarks>
        /// The Dispatched() method cleans up references maintained in the acknowleded and 
        /// processing maps.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        public void Dispatched(string correlationId)
        {
            lock (_instanceLock)
            {
                string processingServiceKey;
                if (_processing.TryGetValue(correlationId, out processingServiceKey))
                {
                    ReliableRequestDictionary serviceRequests;
                    if (_acknowledged.TryGetValue(processingServiceKey, out serviceRequests))
                    {
                        serviceRequests.Remove(correlationId);

                        if (serviceRequests.Count == 0)
                            _acknowledged.Remove(processingServiceKey);

                        LogUtility.Debug("[CorrelationId : {0}] Dispatched [Service : {1}]", 
                            correlationId, processingServiceKey);
                    }

                    _processing.Remove(correlationId);
                }

                _queued.Remove(correlationId);
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transitions the reliable request state from the queued state when a cancellation event 
        /// occurs. The canceled state is an end state for reliable requests.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void Canceled(string correlationId)
        {
            lock (_instanceLock)
            {
                 _queued.Remove(correlationId);
                LogUtility.Debug("[CorrelationId : {0}] Canceled", correlationId);
            } 
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Generates a compond key representing a service instance.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private string CreateProcessingServiceKey(
            string processingServiceName,
            string processingServiceSession)
        {
            return String.Format("{0}.{1}", processingServiceName, processingServiceSession);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
