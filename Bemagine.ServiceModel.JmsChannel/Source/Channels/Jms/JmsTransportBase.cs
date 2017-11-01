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

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal abstract class JmsTransportBase // : Jms.IExceptionListener
    {
        #region data members & member properties
        //----------------------------------------------------------------------------------------//
        // data members & member properties
        //----------------------------------------------------------------------------------------//

        protected Uri Uri { get; private set; }
        protected JmsTransportContext TransportProperties { get; private set; }

        private Jms.IConnectionFactory ConnectionFactory { get; set; }
        private Jms.IConnection Connection { get; set; }
        protected Jms.ISession Session { get; private set; }

        //----------------------------------------------------------------------------------------//
        // transport state & synchronization
        //----------------------------------------------------------------------------------------//

        protected object ThisLock
        {
            get { return _thisLock; }
        }
        private readonly object _thisLock = new object();

        protected bool IsClosed
        {
            get { lock (ThisLock) return _closed; }
        }
        private volatile bool _closed;
        #endregion

        #region construction
        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        protected JmsTransportBase(Uri uri, JmsTransportContext transportProperties)
        {
            Uri = uri;
            TransportProperties = transportProperties;

            Initialize();
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Initializes the JMS transport.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void Initialize()
        {
            string jmsProvider = Uri.Authority;

            InitializeConnectionFactory(jmsProvider);
            InitializeConnection(jmsProvider);
            InitializeSession(jmsProvider);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Initializes the JMS connection factory.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void InitializeConnectionFactory(string jmsProvider)
        {
            try
            {
                Type connectionFactoryType =
                    Type.GetType(TransportProperties.JmsConnectionFactoryType);

                ConnectionFactory =
                    Activator.CreateInstance(connectionFactoryType, jmsProvider,
                        (int) TransportProperties.MaxReceivedMessageSize) as Jms.IConnectionFactory;
            }
            catch (Exception exception)
            {
                ChannelUtility.LogErrorAndThrowCommunicationException(
                    "Failed to create an instance of the IConnectionFactory of type [{0}]. Ensure " +
                    "that the IConnectionFactory implementation type is properly specified using " +
                    "the \"jmsConnectionFactoryType\" transport property and the constructor of " +
                    "that implementation accepts two arguments (string, int).", exception,
                    TransportProperties.JmsConnectionFactoryType);
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Initializes the JMS connection.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void InitializeConnection(string jmsProvider)
        {
            try
            {
                Connection = ConnectionFactory.CreateConnection(
                    TransportProperties.UserName, TransportProperties.Password);
            }
            catch (Exception exception)
            {
                ChannelUtility.LogErrorAndThrowCommunicationException(
                    "Failed to connect to the JMS Server on {0}. Verify that server name and " +
                    "and user credentials are correct.", exception, jmsProvider);
            }

            // Connection.ExceptionListener = this;
            Connection.Start();
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Initializes the JMS connection session.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void InitializeSession(string jmsProvider)
        {
            try
            {
                Session = Connection.CreateSession(false, Jms.AcknowledgeMode.AutoAcknowledge);
            }
            catch (Exception exception)
            {
                ChannelUtility.LogErrorAndThrowCommunicationException(
                    "Failed create a session on the JMS Server on {0}.", exception, jmsProvider);
            }
        }
        #endregion

        #region public interface
        //----------------------------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected virtual void OnClosing()
        {
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Closes down the JMS transport elements. Subsequent calls to this close method result
        /// in a NOOP.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void Close()
        {
            if (!_closed)
            {
                lock (ThisLock)
                {
                    if (!_closed)
                    {
                        OnClosing();

                        if (Session != null)
                        {
                            Session.Close();
                            Session = null;
                        }

                        if (Connection != null)
                        {
                            Connection.Close();
                            Connection = null;
                        }
                        _closed = true;
                    }
                }
            }
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
