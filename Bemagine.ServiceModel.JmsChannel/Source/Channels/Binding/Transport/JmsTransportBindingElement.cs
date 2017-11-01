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

    //--------------------------------------------------------------------------------------------//
    // using aliases
    //--------------------------------------------------------------------------------------------//

    using BPD = BindingPropertyDefaults;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The JmsQueueTransportBindingElement class provides all of the standard features of a 
    /// transport binding element required to determine if a property is supported, a channel
    /// manager can be built, and to build channel managers as requested.
    /// </summary>
    /// <remarks>
    /// The JMS queue transport only supports sessionless DuplexChannels. See the DuplexChannel 
    /// class for additional remarks on this limitation.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal sealed class JmsTransportBindingElement : TransportBindingElement
    {
        //----------------------------------------------------------------------------------------//
        // data members & member properties
        //----------------------------------------------------------------------------------------//

        private readonly JmsTransportContext _transportContext;

        public override long MaxBufferPoolSize
        {
            get { return _transportContext.MaxBufferPoolSize; }
            set { _transportContext.MaxBufferPoolSize = value; }
        }

        public override long MaxReceivedMessageSize
        {
            get { return _transportContext.MaxReceivedMessageSize; }
            set { _transportContext.MaxReceivedMessageSize = value; }
        }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public JmsTransportBindingElement(string scheme)
        {
            _transportContext = 
                new JmsTransportContext(scheme)
                {
                    UserName = BPD.UserName,
                    Password = BPD.Password,
                    ExplicitResponseDestinationName = ""
                };

            MaxBufferPoolSize = BPD.MaxBufferPoolSize;
            MaxReceivedMessageSize = BPD.MaxReceivedMessageSize;
            ManualAddressing = false;
        }

        //----------------------------------------------------------------------------------------//
        // conversion operator
        //----------------------------------------------------------------------------------------//

        public static implicit operator JmsTransportContext(JmsTransportBindingElement element)
        {
            return element._transportContext;
        }

        //----------------------------------------------------------------------------------------//
        // TransportBindingElement overrides
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the transport scheme supported by the channel stack.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override string Scheme { get { return _transportContext.Scheme; } }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Clones the transport binding element instance.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override BindingElement Clone()
        {
            var clone = new JmsTransportBindingElement(Scheme);
            clone._transportContext.CopyFrom(_transportContext);

            return clone;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Implements the GetProperty method for inspecting the shape of the channel stack. 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context", "The BindingContext is not valid");
            
            if (context.BindingParameters.Find<MessageEncodingBindingElement>() == null)
                context.BindingParameters.Add(new BinaryMessageEncodingBindingElement());

            return context.GetInnerProperty<T>();
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the type of the TChannel is IDuplexChannel.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return (typeof(TChannel) == typeof(IDuplexChannel));
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the type of the TChannel is IDuplexChannel.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            return (typeof(TChannel) == typeof(IDuplexChannel));
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Constructs the channel factory stack.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(
            BindingContext context)
        {
            return (IChannelFactory<TChannel>)((object) new JmsChannelFactory(this, context));
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Constructs the channel listener stack.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(
            BindingContext context)
        {
            return (IChannelListener<TChannel>) new JmsChannelListener(this, context);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
