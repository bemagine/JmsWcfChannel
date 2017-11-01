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
    using Bemagine.ServiceModel.Jms;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The JMS transport that manages the heartbeat message transmissions of the reliable request
    /// protocol.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class HeartbeatTransport : JmsTransportBase, IMessageListener
    {
        //----------------------------------------------------------------------------------------//
        // constants
        //----------------------------------------------------------------------------------------//

        private const string ServiceNameProperty = "ServiceName";
        private const string ServiceSessionProperty = "ServiceSession";

        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
        
        private readonly bool _isClient;
        private readonly Action<string, string> _onHeartbeat;  
 
        private readonly string _heartbeatTopicName;
        private IDestination _heartbeatTopic;

        private IMessageConsumer _consumer;
        private IMessageProducer _producer;          

        //----------------------------------------------------------------------------------------//
        // member properties
        //----------------------------------------------------------------------------------------//

        //private int MaxMessageSize
        //{
        //    get { return 8142; }
        //}

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public HeartbeatTransport(JmsTransportContext jmsTransportProperties, Uri uri, 
            bool isClient, Action<string, string> onHeartbeat)
                : base(uri, jmsTransportProperties)
        {
            _isClient = isClient;
            _onHeartbeat = onHeartbeat;

            _heartbeatTopicName = String.Format("{0}-HEARTBEAT",
                (uri.Segments.Length > 1) ? uri.Segments[uri.Segments.Length-1] : uri.AbsolutePath
            );

            InitializeTransport();
        }

        //----------------------------------------------------------------------------------------//
        // Transport initialization and shutdown
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Initializes the transport consumer.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void InitializeConsumer()
        {
            _consumer = Session.CreateConsumer(_heartbeatTopic);
            _consumer.MessageListener = this; 
        }
       
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Initializes the transport producer.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void InitializeProducer()
        {
            _producer = Session.CreateProducer(null);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The initialization process involves constructing the JMS transport infrastructure, and
        /// configuring the appropirate input and output endpoints.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void InitializeTransport()
        {
            _heartbeatTopic = Session.CreateTopic(_heartbeatTopicName);

            if (_isClient)
            {
                InitializeConsumer();
            }
            else
            {
                InitializeProducer();
            }
        }

        //----------------------------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//

        public void SendHeartbeat(string serviceName, string serviceSession)
        {
            if (_producer != null)
            {
                IMessage message = Session.CreateMessage();
                message.Destination = _heartbeatTopic;
                message.SetStringProperty(ServiceNameProperty, serviceName);
                message.SetStringProperty(ServiceSessionProperty, serviceSession);

                _producer.Send(_heartbeatTopic, message);
            }
        }

        //----------------------------------------------------------------------------------------//
        // JMS message handling
        //----------------------------------------------------------------------------------------//

        public void OnMessage(IMessage message)
        {
            string serviceName = message.GetStringProperty(ServiceNameProperty);
            string serviceSession = message.GetStringProperty(ServiceSessionProperty);

            if (!serviceName.IsNullOrEmpty() && !serviceSession.IsNullOrEmpty())
                _onHeartbeat(serviceName, serviceSession);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
