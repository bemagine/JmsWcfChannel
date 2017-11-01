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

    using System.Threading;
    using System.Diagnostics;

    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The QueueThrottle class monitors concurrent operations executing relative to the endpoint
    /// that dispatched them. By monitoring the number concurrent operations executing, the queue
    /// throttle can indicate to queue drainers whether or not to consume additional messages from
    /// the queue.
    /// </summary>
    /// <remarks>
    /// Clearly the queue throttle is implemented as IDispatchMessageInspector, which acts as a
    /// proxy for operation excution tracing. Regardless of the service contract context of the 
    /// operation, the methods implemented for this interface are guaranteed to be called in the
    /// appropriate order. This includes one-way messages. Microsoft documentation states that 
    /// "IDispatchMessageInspector objects are always called at the same point during message 
    /// dispatch regardless of whether an operation is one-way or request-reply."  
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal sealed class QueueThrottle : IDispatchMessageInspector
    {
    //--------------------------------------------------------------------------------------------//
    // data members and member properties
    //--------------------------------------------------------------------------------------------//

        public const int InfiniteWaitTimeout = -1;

        private readonly int _maxConcurrentEndpointCalls;
        private int _concurrentCalls;

        //----------------------------------------------------------------------------------------//
        // instance synchronization primitives
        //----------------------------------------------------------------------------------------//

        private readonly object _thisLock = new object();
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(true);       

    //--------------------------------------------------------------------------------------------//
    // construction
    //--------------------------------------------------------------------------------------------//

        public QueueThrottle(int maxConcurrentEndpointCalls)
        {
            _maxConcurrentEndpointCalls = maxConcurrentEndpointCalls;
            _waitHandle = new AutoResetEvent(true);
        }

    //--------------------------------------------------------------------------------------------//
    // public interface
    //--------------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// If the maximum number of concurrent calls executing limit is reach, this method will
        /// block the current thread until a call completes.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public bool SlowDown(int timeout = InfiniteWaitTimeout)
        {
            return _waitHandle.WaitOne(timeout);
        }

    //--------------------------------------------------------------------------------------------//
    // IDispatchMessageInspector
    //--------------------------------------------------------------------------------------------//

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
            lock (_thisLock)
            {
                ++_concurrentCalls;

                if (_concurrentCalls >= _maxConcurrentEndpointCalls)
                    _waitHandle.Reset();
            }

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
            lock (_thisLock)
            {
                --_concurrentCalls;
                Debug.Assert(_concurrentCalls >= 0, "QueueThrottle mismatch in receive/reply");

                if (_concurrentCalls < _maxConcurrentEndpointCalls)
                    _waitHandle.Set();
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
