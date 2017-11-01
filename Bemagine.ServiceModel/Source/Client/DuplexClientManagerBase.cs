//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The DuplexClientManagerBase is an abstraction of the basic extended set of features 
    /// required of a WCF DuplexClientBase instance. These features include the proper construction
    /// of the composed duplex proxy client that supports proper session shutdown handling and
    /// ensuring connectivity prior to making any calls.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public abstract class DuplexClientManagerBase<IServiceContractT> :
        ClientManagerBase<IServiceContractT> where IServiceContractT : class
    {
        #region Proxy Client Implementation
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Internal service proxy client implementation. Should not be exposed from the manager.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected sealed class DuplexProxyClient 
            : DuplexClientBase<IServiceContractT>, IProxyClient<IServiceContractT>
        {
            //------------------------------------------------------------------------------------//
            /// <summary>
            /// Provides a basic detection mechanism for determining when the service is no longer
            /// available. This detection occurs via proxy reasoning that the input session has 
            /// been terminated. A session being terminated does not necessarily mean that the
            /// service has died, but in the context TCP this generally means that the connection
            /// is lost given that the connection represents the session (?).
            /// </summary>
            //------------------------------------------------------------------------------------//

            private sealed class InputSessionShutdownHandler : IInputSessionShutdown
            {
                public void ChannelFaulted(IDuplexContextChannel channel)
                {
                    channel.Abort();
                }

                public void DoneReceiving(IDuplexContextChannel channel)
                {                    
                    if (!ChannelIsClosingOrClosed(channel))
                    {
                        try { channel.Close();  }
                        catch { channel.Abort(); }
                    }
                }

                private bool ChannelIsClosingOrClosed(IDuplexContextChannel channel)
                {
                    return (
                        (channel.State == CommunicationState.Closing) ||
                        (channel.State == CommunicationState.Closed)
                    );
                }
            }

            //------------------------------------------------------------------------------------//
            /// <summary>
            /// Registers InputSessionShutdownHandler endpoint behavior.
            /// </summary>
            //------------------------------------------------------------------------------------//

            private sealed class InputSessionShutownBehavior : IEndpointBehavior
            {
                public void AddBindingParameters(ServiceEndpoint endpoint, 
                    BindingParameterCollection bindingParameters)
                {
                }

                public void ApplyClientBehavior(ServiceEndpoint endpoint, 
                    ClientRuntime clientRuntime)
                {
                    clientRuntime.CallbackDispatchRuntime.InputSessionShutdownHandlers.Add(
                        new InputSessionShutdownHandler());
                }

                public void ApplyDispatchBehavior(ServiceEndpoint endpoint, 
                    EndpointDispatcher endpointDispatcher)
                {
                    endpointDispatcher.DispatchRuntime.InputSessionShutdownHandlers.Add(
                        new InputSessionShutdownHandler());
                }

                public void Validate(ServiceEndpoint endpoint)
                {
                }
            }

            //------------------------------------------------------------------------------------//
            // construction
            //------------------------------------------------------------------------------------//

            public DuplexProxyClient(string endpointConfigurationName, InstanceContext context)
                : base(context, endpointConfigurationName)
            {
                Endpoint.Behaviors.Add(new InputSessionShutownBehavior());
            }

            //------------------------------------------------------------------------------------//
            // IMarketData proxy implementation
            //------------------------------------------------------------------------------------//

            public IServiceContractT ProxyChannel { get { return Channel; } }
            public IClientChannel ProxyInnerChannel { get { return InnerChannel; } }

            public ChannelFactory<IServiceContractT> ProxyChannelFactory
            {
                get { return ChannelFactory; }
            }
        }
        #endregion
   
        #region DuplexClientManagerBase Virtual Interfaces
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates the instance object specifying the this object as the object that will handle
        /// callback operations. Override this property when a object other than the deriving
        /// class will handle callback operations.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected virtual InstanceContext Context 
        {
            get { return new InstanceContext(this); }
        }

        #endregion

        #region Construction -- Deconstruction
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates an instance of the default proxy client. Override to provide a custom
        /// implementation for the proxy client. These clients generally derive from ClientBase
        /// or DuplexClientBase.
        /// </summary>
        //----------------------------------------------------------------------------------------//
        
        protected override IProxyClient<IServiceContractT> CreateInnerProxyClient()
        {
            return new DuplexProxyClient(EndpointConfigurationName, Context);
        }

        #endregion

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        protected DuplexClientManagerBase(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
