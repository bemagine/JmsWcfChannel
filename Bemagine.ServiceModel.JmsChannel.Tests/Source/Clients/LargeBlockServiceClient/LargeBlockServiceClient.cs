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
    /// WCF client for the LargeBlock Service
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    public sealed class LargeBlockServiceClient 
        : TaskDuplexClientManagerBase<ILargeBlock>, ILargeBlockResponse
    {        
        #region LargeBlockServiceClient Instance Implementation
        #region ILargeBlockResponse Implementation
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Aysnchronous response handler for successful calls to RequestLargeBlock().
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void ILargeBlockResponse.OnRequestLargeBlock(string correlationId, byte[] largeBlock)
        {
            CompleteRequest(correlationId, largeBlock);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Aysnchronous response handler for un-successful calls to RequestLargeBlock().
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void ILargeBlockResponse.OnRequestLargeBlockFailure(
            string correlationId, string errorMessage)
        {
            FaultRequest(correlationId, errorMessage);
        }        
        #endregion

        #region LargeBlockServiceClient Public Interfaces
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Asyncronously requests a large block array of bytes of size equal to blockSize.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public Task<byte[]> RequestLargeBlock(uint blockSize)
        {
            return DispatchRequest<byte[]>(
                (correlationId) => ProxyChannel.RequestLargeBlock(correlationId, blockSize));
        }
        #endregion

        #region LargeBlockServiceClient Singleton Implementation
        //----------------------------------------------------------------------------------------//
        // data members & properties
        //----------------------------------------------------------------------------------------//

        private static readonly Lazy<LargeBlockServiceClient> _instance =
            new Lazy<LargeBlockServiceClient>(() => new LargeBlockServiceClient());

        public static LargeBlockServiceClient Instance { get { return _instance.Value; } }
        
        //----------------------------------------------------------------------------------------//
        // constructor
        //----------------------------------------------------------------------------------------//

        private LargeBlockServiceClient() : base("LargeBlockServiceEP")
        {
        }        
        #endregion
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
