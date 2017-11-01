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

    using System;
    using System.ServiceModel;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The ClientManagerBase is an abstraction of the basic extended set of features 
    /// required of a WCF ClientBase instance. These features include the proper construction
    /// of the composed  proxy client that supports proper session shutdown handling and
    /// ensuring connectivity prior to making any calls.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public abstract class ClientManagerBase<IServiceContractT> : IDisposable
        where IServiceContractT : class
    {
        #region Proxy Client Implementation
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Internal service proxy client implementation. Should not be exposed from the manager.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected sealed class ProxyClient 
            : ClientBase<IServiceContractT>, IProxyClient<IServiceContractT>
        {
            //------------------------------------------------------------------------------------//
            // construction
            //------------------------------------------------------------------------------------//

            public ProxyClient(string endpointConfigurationName)
                : base(endpointConfigurationName)
            {                
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

        #region ClientManagerBase Instance Implementation
        //----------------------------------------------------------------------------------------//
        // data members and member properties
        //----------------------------------------------------------------------------------------//

        private IProxyClient<IServiceContractT> _proxyClient;
        private readonly object _thisLock = new object();
        private readonly object _communicationLock = new object();
        private bool _isDisposed;

        private readonly string _nullProxyClientErrorMessage;
        protected string EndpointConfigurationName { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Ensures connectivity to the specified service endpoint and returns the proxy channel.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected IServiceContractT ProxyChannel 
        { 
            get 
            { 
                EnsureConnectivity();
                return _proxyClient.ProxyChannel; 
            } 
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        ///  Ensures connectivity to the specified service endpoint and returns the inner channel
        ///  of the proxy. This allows the derived class to interact with the WCF layer if 
        ///  required (i.e. register event handlers, etc.).
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected IClientChannel ProxyInnerChannel 
        { 
            get 
            { 
                EnsureConnectivity();
                return _proxyClient.ProxyInnerChannel; 
            } 
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the current state of the communcation-oriented object.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected CommunicationState ProxyState
        {
            get { return _proxyClient.State; }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the proxy client factory communication state is created.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected bool IsFactoryCreated
        {
            get { return _proxyClient.ProxyChannelFactory.State == CommunicationState.Created; }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the proxy client communication state is created, opening, or opened.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected bool IsCreatedOrOpened
        {
            get 
            {
                return (
                    (_proxyClient.State == CommunicationState.Created) || 
                    (_proxyClient.State == CommunicationState.Opening) ||
                    (_proxyClient.State == CommunicationState.Opened)
                );
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the channel close operation was initiated by the client or false when
        /// the close was initiated in response to the service closing the channel (faulted or
        /// closed).
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected bool ClientInitiatedClose { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the object used for synchronizing access to the 'this' context.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected object ThisLock 
        { 
            get { return _thisLock; } 
        }
           
                
        #region ClientManagerBase Private Interfaces
        //----------------------------------------------------------------------------------------//
        // private interfaces
        //----------------------------------------------------------------------------------------//        

        private void EnsureConnectivity()
        {
            if ((_proxyClient == null) || (_proxyClient.State != CommunicationState.Opened))
            {
                lock (_communicationLock)
                {
                    if ((_proxyClient != null) && !IsCreatedOrOpened)
                    {
                        //_proxyClient.Abort();
                        _proxyClient = null;
                    }

                    if (_proxyClient == null)
                    {
                        try
                        {
                            _proxyClient = CreateInnerProxyClient();

                            //--------------------------------------------------------------------//
                            // It may strike you as odd that we only call the proxy client's Open()
                            // method if the channel factory is in the created state. There's a
                            // known WCF issue regarding redundant Open calls to the underlying
                            // channel. The following link documents the issue.
                            //
                            // http://support.microsoft.com/kb/2015845/en
                            //
                            // An example of code that elicits the redundant call is as follows.
                            //
                            // ClientBase<IServiceContractT> wcfClient = 
                            //     new ClientBase<IServiceContractT>();
                            //
                            // wcfClient.OnOpened += OnOpened;
                            // wcfClient.Open();
                            //
                            // In the example above, setting the OnOpened handler opens the 
                            // underlying channel factory. Thus the subsequent call to proxy
                            // client generates an InvalidOperationException. The factory will
                            // defers opens the underlying channel until the first request is
                            // made. This is a known issue that Microsoft will hopefully address.
                            // Setting the event handler, in my opinion should not change the
                            // state of the underlying channel stack. Exceptional cases such as
                            // this leads to unnecessary code complexity. All we can do at this
                            // point is verify the state of the channel factory.
                            //--------------------------------------------------------------------//

                            if (IsFactoryCreated)
                                _proxyClient.Open();
                        }
                        catch (Exception e)
                        {
                            throw new CommunicationException(_nullProxyClientErrorMessage, e);
                            // TODO: trace logging
                        }
                    }
                }
            }
        }
        #endregion

        #region Construction -- Deconstruction
        //----------------------------------------------------------------------------------------//
        // construction / deconstruction
        //----------------------------------------------------------------------------------------//

        protected ClientManagerBase(string endpointConfigurationName)
        {
            EndpointConfigurationName = endpointConfigurationName;

            _nullProxyClientErrorMessage =
                string.Format(
                    "The service client for the endpoint configuration {0} was not properly "+
                    "instantiated. This generally indicates a mis-configuration of the service "+
                    "endpoint. If you are an user seeing this message, please provide the "+
                    "information in this message to the support team.", EndpointConfigurationName);
        }
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates an instance of the default proxy client. Override to provide a custome
        /// implementation for the proxy client. These clients generally derive from ClientBase
        /// or DuplexClientBase.
        /// </summary>
        //----------------------------------------------------------------------------------------//
        
        protected virtual IProxyClient<IServiceContractT> CreateInnerProxyClient()
        {
            IProxyClient<IServiceContractT> createdProxyClient =
                new ProxyClient(EndpointConfigurationName);

            createdProxyClient.Faulted += OnProxyChannelFaulted;
            return createdProxyClient;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// General handler for channel faults that faults the underlying proxy channel.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private void OnProxyChannelFaulted(object sender, EventArgs eventArgs)
        {
            _proxyClient.Abort();
            // TODO: Trace logging
        }
        
        //----------------------------------------------------------------------------------------//
        // The Dispose() method was intentionally made explicit to prevent callers from implicitly
        // invoking this method during the application lifetime. It is provided to ensure graceful
        // shutdown and should be called at application exit.
        //----------------------------------------------------------------------------------------//

        void IDisposable.Dispose()
        {
            if (!_isDisposed)
            {
                ClientInitiatedClose = true;

                if (_proxyClient != null)
                {
                    if (_proxyClient.State == CommunicationState.Faulted)
                        _proxyClient.Abort();

                    else
                    {
                        try { _proxyClient.Close(); }
                        catch (Exception) { _proxyClient.Abort(); }
                    }

                    _proxyClient = null;
                }
            }
            _isDisposed = true;
        }
        #endregion
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
