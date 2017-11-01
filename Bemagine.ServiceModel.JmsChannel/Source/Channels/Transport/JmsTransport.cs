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
    using System.Threading;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    // using aliases
    //--------------------------------------------------------------------------------------------//

    using WcfMessage = System.ServiceModel.Channels.Message;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Encapsulates the JMS queue transport machine.
    /// </summary>
    /// <remarks>
    /// This is definitely a first draft, get it work implementation. It is robust, but really 
    /// could use a refactor to more appropriately support timeouts.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal sealed class JmsTransport : JmsTransportBase
    {
    //--------------------------------------------------------------------------------------------//
    // data members and member properties
    //--------------------------------------------------------------------------------------------//

        private Jms.IMessageConsumer _consumer;
        private Jms.IMessageProducer _producer;
        private readonly IJmsChannelManager _channelManager;
        
        //----------------------------------------------------------------------------------------//
        // addressing members and properties
        //----------------------------------------------------------------------------------------//

        public Jms.IDestination RemoteDestination { get; private set; }
        public Jms.IDestination LocalDestination { get; private set; }

        public EndpointAddress LocalAddress { get; private set; }

        //----------------------------------------------------------------------------------------//
        // utility properties
        //----------------------------------------------------------------------------------------//

        private MessageEncoder Encoder
        {
            get { return _channelManager.MessageEncoderFactory.Encoder; }
        }

        private BufferManager BufferManager
        {
            get { return _channelManager.BufferManager; }
        }

        private int MaxMessageSize
        {
            get { return (int) TransportProperties.MaxReceivedMessageSize; }
        }

        private int MessageSizeCompressionThreshold
        {
            get { return (int) TransportProperties.MessageSizeCompressionThreshold; }
        }

        private bool IsClient
        {
            get { return _channelManager.IsClient; }
        }

        private Jms.DeliveryMode DeliveryMode
        {
            get 
            { 
                return (TransportProperties.PersistentDeliveryRequired) ? 
                    Jms.DeliveryMode.Persistent : Jms.DeliveryMode.NonPersistent;
            }
        }

    //--------------------------------------------------------------------------------------------//
    // construction
    //--------------------------------------------------------------------------------------------//

        public JmsTransport(IJmsChannelManager channelManager, Uri uri) :
            base(uri, channelManager.TransportProperties)
        {
            _channelManager = channelManager;
            InitializeProducerAndConsumer();
        }
            
    //--------------------------------------------------------------------------------------------//
    // Transport initialization and shutdown
    //--------------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected override void OnClosing()
        {
            if (_consumer != null)
            {
                _consumer.Close();
                _consumer = null;
            }

            if (_producer != null)
            {
                _producer.Close();
                _producer = null;
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private Jms.IDestination CreateDestination(string destinationName)
        {
            return (TransportProperties.Scheme == "net.jmsq") ?
                Session.CreateQueue(destinationName) : Session.CreateTopic(destinationName);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void InitializeProducerAndConsumer()
        {
            //------------------------------------------------------------------------------------//
            // If we are in the server context we construct the listen queue based on the absolute 
            // path of the URI. It is assumed that the queue was defined statically in the JMS 
            // queues.conf, but this is not strictly required. The benefit to a statically defined 
            // queue is authentication. Alas, I regress from my digress. In the client context we 
            // generate the local (response) queue given one of the following conditions (ordered 
            // by precedence):
            //
            // 1. An explicit response queue name was specified.
            //
            // 2. The flag to construct the response queue name from the remote endpoind URI was
            //    set. In this case, ".Response" is appended to the remote queue name.
            //
            // 3. Generate a unique response queue name.
            //------------------------------------------------------------------------------------//

            string absolutePath =
                (Uri.Segments.Length > 1) ? 
                    Uri.Segments[Uri.Segments.Length-1] : 
                    Uri.AbsolutePath;

            if (IsClient)
            {
                string replyToQueueName = TransportProperties.ResponseDestinationName;

                LocalDestination = CreateDestination(replyToQueueName); 
                RemoteDestination = CreateDestination(absolutePath);

                LocalAddress = 
                    new EndpointAddress(string.Format("{0}://{1}/{2}", Uri.Scheme,
                        Uri.Authority, replyToQueueName));
            }

            else
            {
                LocalDestination = CreateDestination(absolutePath);
                LocalAddress = new EndpointAddress(Uri);
            }                        

            //------------------------------------------------------------------------------------//
            // create the listen queue message consumer.
            //------------------------------------------------------------------------------------//

            _consumer = Session.CreateConsumer(LocalDestination);
            _producer = Session.CreateProducer(null);
        }
           
    //--------------------------------------------------------------------------------------------//
    // utility methods
    //--------------------------------------------------------------------------------------------//

        private void ThrowTransportIsClosedException(bool wasSending)
        {
            ChannelUtility.LogErrorAndThrowCommunicationException(
                "Attempted to {0} messages on a closed transport.", null,
                (wasSending) ? "send" : "receive");
        }

        private void SlowDown()
        {
            if (_channelManager.Throttle != null)
                _channelManager.Throttle.SlowDown();
        }

    //--------------------------------------------------------------------------------------------//
    // sending and receiving
    //--------------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Determines the send destination queue for the message given the send context. In the 
        /// client context, the remote context is singular and known. The RemoteDestination initialized
        /// during transport intialization is returned. In the server context (listener) the To
        /// URI represents the unique client queue to send the message. From that URI, the JMS
        /// queue destination is constructed.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        Jms.IDestination GetRemoteDestination(WcfMessage wcfMessage)
        {
            if (IsClient)
            {
                return RemoteDestination;
            }

            Uri to = wcfMessage.Headers.To;            
            if (to == null)
            {
                ChannelUtility.LogErrorAndThrowCommunicationException(
                    "Invalid remote address specified.", null);
            }

            //------------------------------------------------------------------------------------//
            // The queue name is always the last segment of the absolute path of the URI. So, we
            // retrive that value pass the length - 1.
            //------------------------------------------------------------------------------------//

            else if (to.Segments.Length < 2)
            {
                ChannelUtility.LogErrorAndThrowCommunicationException(
                    "Remote queue not specified as part of the URI.", null);
            }

            Debug.Assert(to != null, "to != null");
            return CreateDestination( to.Segments[ to.Segments.Length-1] );
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Transforms a WCF message into an JMS message and sends the data over the JMS bus to
        /// the appropriate queue.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void Send(WcfMessage wcfMessage, TimeSpan timeout)
        {
            //------------------------------------------------------------------------------------//
            // TODO: comsume timeout? This is not a straight forward affair given that send
            // operations are assumed to be asychronous, and JMS send() methods do not
            // provide send timeout support to the JMS server. There is support for message
            // expiry (was the message consumed in a defined period of time). This is not 
            // exactly what we are looking to support since this channel implementation is 
            // optimized for long running asynchronous operations. Queues may be flooded by
            // these long running operations.
            //------------------------------------------------------------------------------------//

            //------------------------------------------------------------------------------------//
            // Determine if we have a valid transport. If not, throw an exception.
            //------------------------------------------------------------------------------------//

            if (IsClosed)
            {
                ThrowTransportIsClosedException(true);
            }

            //------------------------------------------------------------------------------------//
            // Encode the WCF message and transport the bytes in a binary JMS message.
            //------------------------------------------------------------------------------------//

            Jms.IDestination endpointDestination = GetRemoteDestination(wcfMessage);
            
            ArraySegment<byte> buffer = 
                Encoder.WriteMessage(wcfMessage, MaxMessageSize, BufferManager);

            try
            {
                Jms.IBytesMessage jmsMessage = Session.CreateBytesMessage();
                jmsMessage.ReplyTo = (IsClient) ? LocalDestination : null;

                //--------------------------------------------------------------------------------//
                // Determine if the size of uncompressed message exceeds the message size 
                // compression threshold. If so, compress the message bytes. Compression is
                // only performed on messages that exceed some threshold because compression
                // impacts performance. Small messages do not benefit from compression. The
                // threshold should be set to some reasonable size (it defaults to the
                // maximum message size if not specified; threfore, indicating compression 
                // should never be used).
                //--------------------------------------------------------------------------------//

                byte[] destinationArray;
                if (buffer.Count > MessageSizeCompressionThreshold)
                {
                    destinationArray = MessageCompressor.CompressBuffer(buffer);
                    jmsMessage.SetBooleanProperty("compressed", true);
                }
                else
                {
                    destinationArray = new byte[buffer.Count];
                    Array.Copy(buffer.Array, buffer.Offset, destinationArray, 0, buffer.Count);
                }
                
                jmsMessage.WriteBytes(destinationArray);

                _producer.Send(
                    endpointDestination, 
                    jmsMessage,
                    DeliveryMode,
                    jmsMessage.Priority,
                    0 // live on forever
                );
            }

            finally
            {
                if (buffer.Array != null)
                    BufferManager.ReturnBuffer(buffer.Array);

                if (wcfMessage.State != MessageState.Created)
                    wcfMessage.Close();
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Begins the asynchronous receives of JMS messages from the LocalDestination.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public IAsyncResult BeginReceive(AsyncCallback receiveCallback)
        {
            var asyncResult = new TypedAsyncResult<WcfMessage>(receiveCallback, this);
            ThreadPool.QueueUserWorkItem(Receive, asyncResult);
            return asyncResult;
        }

        public WcfMessage EndReceive(IAsyncResult result)
        {
            return TypedAsyncResult<Message>.End(result);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Receives and JMS message from the LocalDestination and transforms it to a WCF message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void Receive(object state)
        {            
            //------------------------------------------------------------------------------------//
            // Determine if we have a valid transport. If not, throw an exception.
            //------------------------------------------------------------------------------------//

            if (IsClosed)
            {
                ThrowTransportIsClosedException(false);
            }

            //------------------------------------------------------------------------------------//
            // Before we attempt to drain a message from the queue we slow down to determine if
            // enough resources are available to process the message. This is part of the queue
            // throttling behavior supported by the JMS Queue Channels.
            //
            // TODO: determin if the JMS connection needs to be paused in order to distribute load
            // appropriately in the scenario where a service instance is being throttled, while 
            // other instances have available processing resources.
            //------------------------------------------------------------------------------------//

            SlowDown();
            Jms.IBytesMessage jmsMessage = null; 
            
            try 
            { 
                Jms.IMessage iMessage = _consumer.Receive(0);
                jmsMessage = (iMessage != null) ? iMessage.Convert<Jms.IBytesMessage>() : null;                
            }
            catch (Jms.JmsException e)
            {
                ChannelUtility.LogErrorAndThrowCommunicationException(
                    "An exception was encountered while calling receive against the JMS server. "+
                    "The exception may have occurred because the JMS server connection was dropped "+
                    "or multiple calls to Receive. See the InnerException for additional details.",
                    e);
            }

            //------------------------------------------------------------------------------------//
            // If the call to receive returns a null JMS message, the transport was shutdown. We
            // complete the asyncronous operations and return to exit the receive call;
            //------------------------------------------------------------------------------------//
            
            if (jmsMessage == null)
            {
                var asyncResult = (TypedAsyncResult<WcfMessage>) state;
                asyncResult.Complete(null, false);

                return;
            }

            //------------------------------------------------------------------------------------//
            // Ensure that the message received does not exceed the maximum message size set 
            // during channel configuration and creation.
            //------------------------------------------------------------------------------------//

            var wcfMessageSize = (int) jmsMessage.BodyLength;
            if (wcfMessageSize > MaxMessageSize)
            {
                ChannelUtility.LogErrorAndThrowCommunicationException(
                    "The received message size ({0}) exceeds the MaxReceivedMessageSize ({1}). "+
                    "Try increasing the maxReceivedMessageSize.", null,
                    wcfMessageSize, MaxMessageSize);
            }

            //------------------------------------------------------------------------------------//
            // Extract the embedded WCF message from the JMS message.
            //------------------------------------------------------------------------------------//

            WcfMessage wcfMessage = null;
            byte[] destinationArray = BufferManager.TakeBuffer(wcfMessageSize);

            try
            {   
                //--------------------------------------------------------------------------------//
                // Determine if the WCF message bytes has been compressed by the sender, and 
                // perform any necessary decompression.
                //--------------------------------------------------------------------------------//

                if (jmsMessage.PropertyExists("compressed"))
                {
                    var compressedBytes = new byte[jmsMessage.BodyLength];
                    jmsMessage.ReadBytes(compressedBytes);

                    MessageCompressor.DecompressBuffer(compressedBytes, destinationArray, 
                          wcfMessageSize);
                }
                else
                {       
                    jmsMessage.ReadBytes(destinationArray);
                }

                var segment = new ArraySegment<byte>(destinationArray, 0, wcfMessageSize);
                wcfMessage = Encoder.ReadMessage(segment, BufferManager);
            }
            finally
            {       
                var asyncResult = (TypedAsyncResult<WcfMessage>) state;
                asyncResult.Complete(wcfMessage, false);
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
