//------------------------------------------------------------------------------------------------//
// !!!IMPORTANT!!!
//
// The code in this translation unit was adapted from the Microsoft sample code for WCF.
//
// Copyright (C) 2003-2005 Microsoft Corporation, All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Channels
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Threading;
    using System.Diagnostics;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A generic base class for IAsyncResult implementations that wraps a ManualResetEvent.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal abstract class AsyncResult : IAsyncResult
    {

    //--------------------------------------------------------------------------------------------//
    // data members and member properties
    //--------------------------------------------------------------------------------------------//
                
        private bool _endCalled;
        private Exception _exception;
        private AsyncCallback _callback;

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the operation complete synchronously.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public bool CompletedSynchronously { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the operation has completed.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public bool IsCompleted { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns an instance of the lock used for syncronizing the instance of this class.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        object ThisLock
        {
            get { return this._thisLock; }
        }
        private object _thisLock = new object();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the state property provided by the async caller.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public object AsyncState { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the async wait handle for manual wait invokation by the caller. This property
        /// lazily constructs the WaitHandle when first called.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_manualResetEvent != null)
                {
                    lock (ThisLock)
                    {
                        if (_manualResetEvent == null)
                        {
                            _manualResetEvent = new ManualResetEvent(this.IsCompleted);
                        }
                    }
                }

                return _manualResetEvent;
            }
        }
        private ManualResetEvent _manualResetEvent;

    //--------------------------------------------------------------------------------------------//
    // construction
    //--------------------------------------------------------------------------------------------//

        protected AsyncResult(AsyncCallback callback, object state)
        {
            this._callback = callback;
            this.AsyncState = state;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Call this version of complete when your asynchronous operation is complete. This will 
        /// update the state of the operation and notify the callback.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected void Complete(bool completedSynchronously)
        {
            if (this.IsCompleted)
            {
                throw new InvalidOperationException("Cannot call Complete twice");
            }

            this.CompletedSynchronously = completedSynchronously;

            //------------------------------------------------------------------------------------//
            // If we completedSynchronously, then there is no chance that the manualResetEvent 
            // was created so we do not need to worry about a race condition.
            //------------------------------------------------------------------------------------//

            if (completedSynchronously)
            {
                Debug.Assert(_manualResetEvent == null, 
                    "No ManualResetEvent should be created for a synchronous AsyncResult.");

                this.IsCompleted = true;
            }

            else
            {
                lock (ThisLock)
                {
                    this.IsCompleted = true;
                    if (this._manualResetEvent != null)
                    {
                        this._manualResetEvent.Set();
                    }
                }
            }

            //------------------------------------------------------------------------------------//
            // If the callback throws, there is a bug in the callback implementation.
            //------------------------------------------------------------------------------------//

            try
            {
                if (this._callback != null)
                {
                  this._callback(this);
                }
            }

            catch (Exception) //exception)
            {
                // Inifinite loop used for debugging!!!
                //
                //ThreadPool.QueueUserWorkItem(
                //    (object state) => { Exception e = (Exception) state; throw e; },
                //    new Exception("Async callback exception", exception)
                //);
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Call this version of complete if you raise an exception during processing. In addition 
        /// to notifying the callback, it will capture the exception and store it to be thrown 
        /// during AsyncResult.End().
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected void Complete(bool completedSynchronously, Exception exception)
        {
            this._exception = exception;
            Complete(completedSynchronously);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// End should be called when the End function for the asynchronous operation is complete.
        /// It ensures the asynchronous operation is complete, and does some common validation.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected static void End(AsyncResult asyncResult)
        {
            if (asyncResult._endCalled)
            {
                throw new InvalidOperationException("Async object already ended.");
            }

            asyncResult._endCalled = true;
            if (!asyncResult.IsCompleted)
            {
                using (WaitHandle handle = asyncResult.AsyncWaitHandle)
                {
                  handle.WaitOne();
                }
            }

            if (asyncResult._exception != null)
            {
                throw asyncResult._exception;
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// End should be called when the End function for the asynchronous operation is complete.
        /// It ensures the asynchronous operation is complete, and does some common validation.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
            where TAsyncResult : AsyncResult
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }
            
            TAsyncResult asyncResult = result as TAsyncResult;

            if (asyncResult == null)
            {
                throw new ArgumentException("Invalid async result.", "result");
            }

            if (asyncResult._endCalled)
            {
                throw new InvalidOperationException("Async object already ended.");
            }

            asyncResult._endCalled = true;

            if (!asyncResult.IsCompleted)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }

            if (asyncResult._manualResetEvent != null)
            {
                asyncResult._manualResetEvent.Close();
            }

            if (asyncResult._exception != null)
            {
                throw asyncResult._exception;
            }

            return asyncResult;
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// An AsyncResult that completes as soon as it is instantiated.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class CompletedAsyncResult : AsyncResult
    {
        public CompletedAsyncResult(AsyncCallback callback, object state)
            : base(callback, state)
        {
            Complete(true);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<CompletedAsyncResult>(result);
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A strongly typed AsyncResult.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal class TypedAsyncResult<T> : AsyncResult
    {
        T data;

        public TypedAsyncResult(AsyncCallback callback, object state)
            : base(callback, state)
        {
        }

        public T Data
        {
            get { return data; }
        }

        public void Complete(T data, bool completedSynchronously)
        {
            this.data = data;
            Complete(completedSynchronously);
        }

        public static T End(IAsyncResult result)
        {
            TypedAsyncResult<T> typedResult = AsyncResult.End<TypedAsyncResult<T>>(result);
            return typedResult.Data;
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A strongly typed AsyncResult that completes as soon as it is instantiated.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class TypedCompletedAsyncResult<T> : TypedAsyncResult<T>
    {
        public TypedCompletedAsyncResult(T data, AsyncCallback callback, object state)
            : base(callback, state)
        {
            Complete(data, true);
        }

        public new static T End(IAsyncResult result)
        {
            TypedCompletedAsyncResult<T> completedResult = result as TypedCompletedAsyncResult<T>;
            if (completedResult == null)
            {
                throw new ArgumentException("Invalid async result.", "result");
            }

            return TypedAsyncResult<T>.End(completedResult);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
