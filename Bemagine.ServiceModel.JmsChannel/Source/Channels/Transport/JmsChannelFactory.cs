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
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Collections.ObjectModel;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The JmsQueueChannelFactory initializes the JmsTransport and creates instances of
    /// the JmsDuplexChannel. Only one transport is created per instance of the factory. 
    /// This allows a mapping of a factory to an JMS server instance.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class JmsChannelFactory 
        : ChannelFactoryBase<IDuplexChannel>, IJmsChannelManager
    {
    //--------------------------------------------------------------------------------------------//
    // data members and member properties
    //--------------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//    
        /// <summary>
        /// The buffer manager used by channels to transform messages.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public BufferManager BufferManager { get; private set; }
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The encoder factory that creates encoders to encode and decode messages. The default
        /// encoder is the BinaryMessageEncoder
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public MessageEncoderFactory MessageEncoderFactory { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The set of JMS queue channel properties required to construct the channels.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public JmsTransportContext TransportProperties { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the channel stack was instantiated in the client context.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public bool IsClient { get { return true; } }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the JmsTransport initialized by the channel manager.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public JmsTransport Transport { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Throttling is not supported for factories (clients). Get returns null, which 
        /// essentially represents an infinite consumer. Attempting to set the instance 
        /// generates an InvalidOperationException.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public QueueThrottle Throttle 
        { 
            get 
            { 
                return null; 
            }
            
            set 
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Throttling of factories (clients) is not supported. If you seeing this " +
                        "exception then upstream checks failed or the support requirements have " +
                        "changed. Either way some investigation is required."));
            }
        }

        //----------------------------------------------------------------------------------------//
        // asynchronous delegates
        //----------------------------------------------------------------------------------------//

        private readonly Action<TimeSpan> _asyncOpen;

    //--------------------------------------------------------------------------------------------//
    // construction
    //--------------------------------------------------------------------------------------------//

        public JmsChannelFactory(
            JmsTransportBindingElement bindingElement,
            BindingContext bindingContext)
                : base(bindingContext.Binding)
        {
            _asyncOpen = OnOpen;
            TransportProperties = bindingElement;

            //------------------------------------------------------------------------------------//
            // Message encoder factor creation
            //------------------------------------------------------------------------------------//

            Collection<MessageEncodingBindingElement> encoderElements =
                bindingContext.BindingParameters.FindAll<MessageEncodingBindingElement>();

            if (encoderElements.Count > 1)
            {
                throw new InvalidOperationException(
                    "It is not valid to specify more than one MessageEncodingBindingElement.");
            }

            MessageEncoderFactory = 
                (encoderElements.Count == 1) ?
                    encoderElements[0].CreateMessageEncoderFactory() :
                    ChannelDefaults.MessageEncoderFactory;

            //------------------------------------------------------------------------------------//
            // Buffer manager creation
            //------------------------------------------------------------------------------------//

            BufferManager = BufferManager.CreateBufferManager(
                bindingElement.MaxBufferPoolSize, (int) bindingElement.MaxReceivedMessageSize);
        }

    //--------------------------------------------------------------------------------------------//
    // ChannelFactoryBase overrides
    //--------------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The standard IChannel object query interface. This override intercepts queries
        /// to the encoder, and marshalls these queries to the embedded MessageEncoderFactory.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override T GetProperty<T>()
        {
            var messageEncoderProperty = MessageEncoderFactory.Encoder.GetProperty<T>();
            if (messageEncoderProperty != null)
            {
                return messageEncoderProperty;
            }

            if (typeof(T) == typeof(JmsTransportContext))
            {
                return TransportProperties as T;
            }

            return base.GetProperty<T>();
        }

    //--------------------------------------------------------------------------------------------//
    // channel opening
    //--------------------------------------------------------------------------------------------//

        protected override void OnOpen(TimeSpan timeout)
        {            
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, 
            object state)
        {            
            return _asyncOpen.BeginInvoke(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _asyncOpen.EndInvoke(result);
        }

    //--------------------------------------------------------------------------------------------//
    // channel close
    //--------------------------------------------------------------------------------------------//         

        protected override void OnClosed()
        {   
            base.OnClosed();

            if (BufferManager != null)
                BufferManager.Clear();

            if (Transport != null)
                Transport.Close();
        }

    //--------------------------------------------------------------------------------------------//
    // channel creation
    //--------------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates the JMS queue transport that the channels will use to send and receive JMS
        /// messages.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void CreateTransport(Uri via)
        {
            if (Transport == null)
            {
                lock (ThisLock)
                {
                    if (Transport == null)
                        Transport = new JmsTransport(this, via);
                }
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// OnCreateChannel performs two tasks in response to a create channel event. First, the
        /// Tibco JMS transport is initialized. Second, creates an JmsDuplexChannel instance.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected override IDuplexChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via)
        {
            ValidateCreateChannel();
            CreateTransport(via);

            return new JmsDuplexChannel(this, remoteAddress, via);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
