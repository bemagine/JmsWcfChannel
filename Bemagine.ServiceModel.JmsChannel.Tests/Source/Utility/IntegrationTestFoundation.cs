//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.Tests.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.JmsChannel.Tests
{
    //--------------------------------------------------------------------------------------------//
    // using  directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Reflection;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using NUnit.Framework;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public abstract class IntegrationTestFoundation<IServiceContractT, ServiceT, ServiceClientT>
        where IServiceContractT : class 
        where ServiceT : class
        where ServiceClientT : TaskDuplexClientManagerBase<IServiceContractT>
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testFunction"></param>
        /// <param name="timeout"></param>
        //----------------------------------------------------------------------------------------//

        protected void Run<T>(Func<Task<T>> testFunction, TimeSpan timeout)
        {
            var testName = (new StackTrace()).GetFrame(1).GetMethod().Name;
            Task<T> testTask = testFunction();

            Assert.IsNotNull(
                testTask, 
                String.Format(
                    "Task<T> returned by the testFunction() for test [{0}] must not be null",
                    testName)
            );

            var waitTask = testTask.ContinueWith(
                (t) =>
                {
                    Assert.IsTrue(
                        t.IsCompleted,
                        String.Format("Test [{0}] failed. Error -- {1}", testName, t.Exception)
                    );
                }
            );

            Assert.IsTrue(
                waitTask.Wait(timeout),
                String.Format("Test [{0}] timed out.", testName)
            );
        }

        //----------------------------------------------------------------------------------------//
        // properties
        //----------------------------------------------------------------------------------------//

        private ServiceController<ServiceT> Service { get; set; }
        private ServiceClientT ServiceClient { get; set; }

        //----------------------------------------------------------------------------------------//
        // test fixture setup / tear down
        //----------------------------------------------------------------------------------------//

        [TestFixtureSetUp]
        public void OnTestFixtureSetUp()
        {
            var property =
                typeof(ServiceClientT).GetProperty(
                    "Instance",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            Assert.IsNotNull(property);
            ServiceClient = (ServiceClientT) property.GetValue(null, null);

            Service = new ServiceController<ServiceT>();
            Service.StartServiceProcessing();
        }

        [TestFixtureTearDown]
        public void OnTestFixtureTearDown()
        {
            ((IDisposable) ServiceClient).Dispose();
            Service.StartServiceProcessing();
        }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        protected IntegrationTestFoundation()
        {
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
