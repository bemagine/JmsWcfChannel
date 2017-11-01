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
    using System.Threading.Tasks;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// WCF client for the Echo Service.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class EchoServiceClient : TaskDuplexClientManagerBase<IEcho>, IEchoResponse
    {        
        #region EchoServiceClient Instance Implementation
        #region IEchoResponse Implementation
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Aysnchronous response handler for successful calls to AsuncGreet().
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void IEchoResponse.OnAsyncGreet(string correlationId, string message)
        {
            CompleteRequest(correlationId, message);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Aysnchronous response handler for un-successful calls to OnAsyncGreet().
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void IEchoResponse.OnAsyncGreetFailure(
            string correlationId, string errorMessage)
        {
            FaultRequest(correlationId, errorMessage);
        }        
        #endregion

        #region EchoServiceClient Public Interfaces
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Asyncronous Echo Service greeting dispatch.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public Task<string> AsyncGreet(string message)
        {
            return DispatchRequest<string>(
                (correlationId) => ProxyChannel.AsyncGreet(correlationId, message));
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Syncronous Echo Service greeting dispatch.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public string SyncGreet(string message)
        {
            return ProxyChannel.SyncGreet(message);
        }
        #endregion

        #region EchoServiceClient Singleton Implementation
        //----------------------------------------------------------------------------------------//
        // data members & properties
        //----------------------------------------------------------------------------------------//

        private static readonly Lazy<EchoServiceClient> _instance =
            new Lazy<EchoServiceClient>(() => new EchoServiceClient());

        public static EchoServiceClient Instance { get { return _instance.Value; } }
        
        //----------------------------------------------------------------------------------------//
        // static construction - given the non-trivial construction of the EchoServiceClient, 
        // proper synchronization is required.
        //----------------------------------------------------------------------------------------//

        private EchoServiceClient() : base("EchoServiceEP")
        {
        }        
        #endregion
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
