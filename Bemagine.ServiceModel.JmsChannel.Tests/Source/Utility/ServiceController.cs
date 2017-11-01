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
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.ServiceModel;

    using log4net;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A general use service controller to manage service execution.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class ServiceController<T> where T: class
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private static readonly ILog _logger = LogManager.GetLogger("JmsChannel.Tests");
        private static ServiceHost _serviceHost;

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Generic method for starting a logical (WCF) service.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void StartServiceProcessing()
        {
            try
            {
                TestFrameworkInitializer.AttachConsole();

                if (_serviceHost != null)
                {
                    try { _serviceHost.Close(); }
                    catch { _serviceHost.Abort(); }
                    finally { _serviceHost = null; }
                }

                _serviceHost = new ServiceHost(typeof(T));
                _serviceHost.Open();

                _logger.Info(
                    string.Format("Service started for {0}.", typeof(T).Name)
                );
            }

            catch (Exception e)
            {
                _logger.Error(
                    string.Format("Exception occurred during service initialziation of {0}.", 
                        typeof(T).Name), e
                );
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Generic method for stopping a logical (WCF) service.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void StopServiceProcessing()
        {
            try
            {
                if (_serviceHost != null)
                {
                    try 
                    { 
                        _serviceHost.Close(); 
                        _logger.InfoFormat("Service stopped for {0}.", typeof(T).Name);
                    }
                    catch { _serviceHost.Abort(); }
                    finally { _serviceHost = null; }
                }

                else
                {
                    _logger.Warn("Attempt to stop the service failed. The service was not running.");
                }
            }

            catch (Exception e)
            {
                _logger.Error(
                    "Exception occurred during service deconstruction of the service.", e
                );
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
