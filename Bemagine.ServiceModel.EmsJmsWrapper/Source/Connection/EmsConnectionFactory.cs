//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.EmsJmsWrapper.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.EmsJmsWrapper
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using Bemagine.ServiceModel.Jms;

    using System;
    using System.Configuration;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Exposes the TIBCO EMS ConnectionFactory implementation as an IConnectionConnection.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    internal sealed class EmsConnectionFactory : IConnectionFactory
    {
        //----------------------------------------------------------------------------------------//
        // data members and member properties
        //----------------------------------------------------------------------------------------//
        
        private readonly TIBCO.EMS.ConnectionFactory _innerConnectionFactory;
        private int MaxMessageSize { get; set; }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public EmsConnectionFactory(string emsServer, int maxMessageSize)
        {
            //------------------------------------------------------------------------------------//
            // Load the emsConnectionFactory configuration section.
            //
            // If no such configuration of the EMS connection factory create the default 
            // configuration. Property values of the configuration represent the default
            // configuration values.
            //------------------------------------------------------------------------------------//

            var configuration =
                ConfigurationManager.GetSection("emsConnectionFactory")
                    as EmsConnectionFactoryConfigurationSection ?? 
                        new EmsConnectionFactoryConfigurationSection();

            //------------------------------------------------------------------------------------//
            // Construct the inner EMS ConnectionFactory.
            //------------------------------------------------------------------------------------//

            _innerConnectionFactory = new TIBCO.EMS.ConnectionFactory("tcp://" + emsServer);

            //------------------------------------------------------------------------------------//
            // Set the EMS ConnectionFactory propery values.
            //------------------------------------------------------------------------------------//

            _innerConnectionFactory.SetConnAttemptCount(
                configuration.ConnectionAttempts);

            _innerConnectionFactory.SetConnAttemptDelay(
                configuration.ConnectionAttemptDelay);

            _innerConnectionFactory.SetConnAttemptTimeout(
                configuration.ConnectionAttemptTimeout);

            _innerConnectionFactory.SetReconnAttemptCount(
                configuration.ReconnectionAttempts);

            _innerConnectionFactory.SetReconnAttemptDelay(
                configuration.ReconnectionAttemptDelay);

            _innerConnectionFactory.SetReconnAttemptTimeout(
                configuration.ReconnectionAttemptTimeout);

            MaxMessageSize = maxMessageSize;
            InitializeSocketBufferSizes();
        }

        //----------------------------------------------------------------------------------------//
        // CreateConnection()
        //----------------------------------------------------------------------------------------//  

        public IConnection CreateConnection()
        {
            return SafeProxy.SafeCall(
                () => { return new EmsConnection(_innerConnectionFactory.CreateConnection()); }
            );
        }

        //----------------------------------------------------------------------------------------//
        // CreateConnection()
        //----------------------------------------------------------------------------------------// 

        public IConnection CreateConnection(string username, string password)
        {
            return SafeProxy.SafeCall(
                () =>
                {
                    return new EmsConnection(
                        _innerConnectionFactory.CreateConnection(username, password));
                });
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Ensures that the underlying TCP sockets used by JMS have sufficient buffer resources
        /// available to handle the configured maximum message size. This is critically important
        /// for the handling of large buffered messages.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void InitializeSocketBufferSizes()
        {
            const int bytesPerKilobyte = 0x400;
            int maxMessageSizeInKb = (int)Math.Ceiling(((double) MaxMessageSize / bytesPerKilobyte));

            TIBCO.EMS.Tibems.SetSocketReceiveBufferSize(maxMessageSizeInKb);
            TIBCO.EMS.Tibems.SetSocketSendBufferSize(maxMessageSizeInKb);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
