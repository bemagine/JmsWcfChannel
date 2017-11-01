//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.Test.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.JmsChannel.Test
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using NUnit.Framework;
    using Bemagine.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal enum BindingContextDirection
    {
        Source, Destination
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A factory for creating IBindingTestContext instances.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class BindingTestContext
    {
        //----------------------------------------------------------------------------------------//
        // IBindingTestContext implementations
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Generic abstract base implementation of IBindingTestContext.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private abstract class BindingTestContextWrapperBase<T> : IBindingTestContext where T : class
        {
            protected T _innerContext;

            protected BindingTestContextWrapperBase(T innerContext, BindingContextDirection direction)
            {
                _innerContext = innerContext;
                IsMutable = (direction == BindingContextDirection.Destination);
            }

            protected T ExtractInnerContext(IBindingTestContext rhs)
            {
                var rhsContext = rhs as BindingTestContextWrapperBase<T>;

                Assert.IsNotNull(
                    rhsContext, 
                    String.Format("Cast to {0} failed.", typeof(BindingTestContextWrapperBase<T>).Name)
                );

                return rhsContext._innerContext;
            }

            public CastT ContextAs<CastT>() where CastT : class
            {
                return _innerContext as CastT;
            }

            public void ResetContext<ContextT>(ContextT context) where ContextT : class
            {
                var resetContext = context as T;
                Assert.IsNotNull(resetContext, "Failed to cast the ResetContext to type T");
                _innerContext = resetContext;
            }

            public bool IsMutable { get; private set; }
            public abstract void CopyFrom(IBindingTestContext rhs);
            public abstract void AssertAreEqual(IBindingTestContext rhs);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// IBindingTestContext implementation wrapping the IReliableRequestContext.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private sealed class ReliableRequestTestContextWrapper
            : BindingTestContextWrapperBase<IReliableRequestContext>
        {
            public ReliableRequestTestContextWrapper(
                IReliableRequestContext innerContext,
                BindingContextDirection direction)
                    : base(innerContext, direction)
            {
            }

            public override void CopyFrom(IBindingTestContext rhs)
            {
                Assert.IsTrue(IsMutable);
                _innerContext.CopyFrom(ExtractInnerContext(rhs));
            }

            public override void AssertAreEqual(IBindingTestContext rhs)
            {
                var rhsInnerContext = ExtractInnerContext(rhs);

                Assert.AreEqual(
                    _innerContext.HeartbeatPeriodicity, 
                    rhsInnerContext.HeartbeatPeriodicity);
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// IBindingTestContext implementation wrapping the IJmsTransportContext.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private sealed class JmsTransportTestContextWrapper
            : BindingTestContextWrapperBase<IJmsTransportContext>
        {
            public JmsTransportTestContextWrapper(
                IJmsTransportContext innerContext, 
                BindingContextDirection direction)
                    : base(innerContext, direction)
            {
            }

            public override void CopyFrom(IBindingTestContext rhs)
            {
                Assert.IsTrue(IsMutable);
                _innerContext.CopyFrom(ExtractInnerContext(rhs));
            }

            public override void AssertAreEqual(IBindingTestContext rhs)
            {
                var rhsInnerContext = ExtractInnerContext(rhs);

                Assert.AreEqual(
                    _innerContext.MaxBufferPoolSize, 
                    rhsInnerContext.MaxBufferPoolSize);

                Assert.AreEqual(
                    _innerContext.MaxReceivedMessageSize, 
                    rhsInnerContext.MaxReceivedMessageSize);

                Assert.AreEqual(
                    _innerContext.MessageSizeCompressionThreshold, 
                    rhsInnerContext.MaxReceivedMessageSize);

                Assert.AreEqual(
                    _innerContext.UserName, 
                    rhsInnerContext.UserName);

                Assert.AreEqual(
                    _innerContext.Password, 
                    rhsInnerContext.Password);

                Assert.AreEqual(
                    _innerContext.PersistentDeliveryRequired, 
                    rhsInnerContext.PersistentDeliveryRequired);

                Assert.AreEqual(
                    _innerContext.ExplicitResponseDestinationName, 
                    rhsInnerContext.ExplicitResponseDestinationName);
                        
                Assert.AreEqual(
                    _innerContext.JmsConnectionFactoryType, 
                    rhsInnerContext.JmsConnectionFactoryType);      
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Factory method that creates the IBindingTestContext instances based on the type of the
        /// generic parameter T.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static IBindingTestContext CreateBindingContext<T>(
            T innerContext, 
            BindingContextDirection direction)
        {
            //------------------------------------------------------------------------------------//
            // The Factory
            //------------------------------------------------------------------------------------//

            var asIReliableRequestContext = innerContext as IReliableRequestContext;
            if (asIReliableRequestContext != null)
                return new ReliableRequestTestContextWrapper(asIReliableRequestContext, direction);

            var asIJmsTransportContext = innerContext as IJmsTransportContext;
            if (asIJmsTransportContext != null)
                return new JmsTransportTestContextWrapper(asIJmsTransportContext, direction);

            //------------------------------------------------------------------------------------//
            // Well, we tried! But the truth is T is not a recognized type. So, we will do what
            // any rationale entity would do when they fail to service a request, hork!
            //------------------------------------------------------------------------------------//

            var errorMessage = String.Format(
                "Failed to create an IBindingTestContext as the innerContext, {0} is not recognized",
                typeof(T).Name
            );

            Assert.Fail(errorMessage);

            //------------------------------------------------------------------------------------//
            // The above assertion does not substitute for a return or throw statement to indicate
            // that all paths return. So, we will simply throw an exception that may never get
            // generated, assuming the assertion breaks control immediately.
            //------------------------------------------------------------------------------------//

            throw new ArgumentException("innerContext", errorMessage);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
