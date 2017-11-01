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
    using System.ServiceModel.Channels;
    using System.Collections.ObjectModel;

    //--------------------------------------------------------------------------------------------//
    // using aliases
    //--------------------------------------------------------------------------------------------//

    using QueueThrottleSetterProperty = System.Action<QueueThrottle>;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The JmsQueueListener initializes the JmsTransport and creates instances of the
    /// JmsDuplexChannel. Only one transport is created per instance of the listener. 
    /// This allows a mapping of a listener to an JMS server instance.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal class JmsChannelListener
        : ChannelListenerBase<IDuplexChannel>, IJmsChannelManager
    {
        //----------------------------------------------------------------------------------------//
        // data members and member properties
        //----------------------------------------------------------------------------------------//

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

        public bool IsClient { get { return false; } }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the JmsTransport initialized by the channel manager.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public JmsTransport Transport { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the QueueThrottle associated with the transport. This is set by the
        /// specification of the QueueThrottlingBehavior.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public QueueThrottle Throttle { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Specifies the URI that the channel will listen on.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override Uri Uri { get { return _listenUri; } }       
        private readonly Uri _listenUri;

        //----------------------------------------------------------------------------------------//
        // The channel associated with this listener.
        //----------------------------------------------------------------------------------------//

        private JmsDuplexChannel _currentChannel;
        private readonly object _currentChannelLock = new object();    
    
        //----------------------------------------------------------------------------------------//
        // Listener action property for setting the queue throttle property.
        //----------------------------------------------------------------------------------------//

        private readonly QueueThrottleSetterProperty _queueThrottleSetterProperty;

        //----------------------------------------------------------------------------------------//
        // asynchronous delegates
        //----------------------------------------------------------------------------------------//

        private readonly Action<TimeSpan> _asyncOpen;
        private readonly Action<TimeSpan> _asyncClose;
        private readonly Func<TimeSpan, IDuplexChannel> _asyncAcceptChannel;
        private readonly Func<TimeSpan, bool> _asyncWaitForChannel;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public JmsChannelListener(
            JmsTransportBindingElement bindingElement,
            BindingContext bindingContext)
                : base(bindingContext.Binding)
        {
            //------------------------------------------------------------------------------------//
            // delegate assignments
            //------------------------------------------------------------------------------------//

            _asyncOpen = OnOpen;
            _asyncClose = OnClose;
            _asyncAcceptChannel = OnAcceptChannel;
            _asyncWaitForChannel = OnWaitForChannel;

            //------------------------------------------------------------------------------------//
            //
            //------------------------------------------------------------------------------------//

            TransportProperties = bindingElement;
            _queueThrottleSetterProperty = queueThrottle => Throttle = queueThrottle;

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

            //------------------------------------------------------------------------------------//
            // Initialize the Listen URI
            //------------------------------------------------------------------------------------//

            _listenUri = 
                new Uri(bindingContext.ListenUriBaseAddress, bindingContext.ListenUriRelativeAddress);
        }

        //----------------------------------------------------------------------------------------//
        // ChannelListenerBase overrides
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The standard IChannel object query interface. This override intercepts queries
        /// to the encoder, and marshalls these queries to the embedded MessageEncoderFactory.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override T GetProperty<T>()
        {
            var messageEncoderProperty =   MessageEncoderFactory.Encoder.GetProperty<T>();
            if (messageEncoderProperty != null)
            {
                return messageEncoderProperty;
            }

            if (typeof(T) == typeof(QueueThrottleSetterProperty))
            {
                return _queueThrottleSetterProperty as T;
            }

            if (typeof(T) == typeof(JmsTransportContext))
            {
                return TransportProperties as T;
            }

            return base.GetProperty<T>();
        }

        //----------------------------------------------------------------------------------------//
        // channel close
        //----------------------------------------------------------------------------------------//         

        protected override void OnClose(TimeSpan timeout)
        {
            lock (ThisLock)
            {
                if (Transport != null)
                    Transport.Close();
            }
        }

        protected override void OnClosed()
        {
            if (BufferManager != null)
            {
                BufferManager.Clear();
            }
            base.OnClosed();
        }

        protected override void OnAbort()
        {
            lock (ThisLock)
            {
                //--------------------------------------------------------------------------------//
                // We cannot assume that the listener opened successfully because Abort may be 
                // called at anytime. Therefore, the transport may not have been created.
                //--------------------------------------------------------------------------------//

                if (Transport != null)
                    Transport.Close();
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, 
            object state)
        {
            return _asyncClose.BeginInvoke(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            _asyncClose.EndInvoke(result);
        }

        //----------------------------------------------------------------------------------------//
        // channel open
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates the JMS queue transport that the channels will use to send and receive JMS
        /// messages.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected override void OnOpen(TimeSpan timeout)
        {
            if (Transport == null)
            {
                lock (ThisLock)
                {
                    if (Transport == null)
                        Transport = new JmsTransport(this, _listenUri);
                }
            }
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

        //----------------------------------------------------------------------------------------//
        // channel accept
        //----------------------------------------------------------------------------------------//

        protected override IDuplexChannel OnAcceptChannel(TimeSpan timeout)
        {
            ChannelUtility.ValidateTimeout(timeout);

            //------------------------------------------------------------------------------------//
            // The communication model with the JMS provider does not model the TCP accept 
            // paradigm. There will only ever be one channel. In this regard, channel creation
            // only occurs once. All other calls to OnAcceptChannel yields a null result.
            //------------------------------------------------------------------------------------//

            JmsDuplexChannel channel;
            if (CreateOrRetrieveChannel(out channel))
            {
                return channel;
            }
            return null;
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, 
            AsyncCallback callback, object state)
        {
            return _asyncAcceptChannel.BeginInvoke(timeout, callback, state);
        }

        protected override IDuplexChannel OnEndAcceptChannel(IAsyncResult result)
        {
           return _asyncAcceptChannel.EndInvoke(result);
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return false;
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, 
            AsyncCallback callback, object state)
        {
            return _asyncWaitForChannel.BeginInvoke(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return _asyncWaitForChannel.EndInvoke(result);
        }

        //----------------------------------------------------------------------------------------//
        // channel creation
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// If the channel for this listener exists, then set the newChannel that channel. Otherwise, 
        /// create the channel and set newChannel to reference that instance. Returns true if the
        /// channel was newly created.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private bool CreateOrRetrieveChannel(out JmsDuplexChannel newChannel)
        {
            bool channelCreated = false;

            if (((newChannel = _currentChannel) == null) && !IsDisposed)
            {
                lock (_currentChannelLock)
                {
                    if (((newChannel = _currentChannel) == null) && !IsDisposed)
                    {
                        newChannel = new JmsDuplexChannel(this, null, null);
                        newChannel.Closed += OnChannelClosed;

                        _currentChannel = newChannel;
                        channelCreated = true;
                    }
                }
            }

            return channelCreated;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// When the channel closes reset the currentChannel reference to null. This will permit
        /// a subsequent channel to be created (if required).
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void OnChannelClosed(object sender, EventArgs args)
        {
            var channel = sender as JmsDuplexChannel;

            lock (_currentChannelLock)
            {
                if (channel == _currentChannel)
                {
                    _currentChannel = null;
                }
            }
        }    
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
