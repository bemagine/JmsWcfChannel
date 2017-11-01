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
    using System.ServiceModel;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The logical (WCF) service that implements the ILargeBlock interface.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public sealed class LargeBlockService : ILargeBlock
    {
        #region ILargeBlock interface implmentation
        //----------------------------------------------------------------------------------------//
        // ILargeBlock interface implmentation
        //----------------------------------------------------------------------------------------//

        public byte[] GetLargeBlock(uint blockSize)
        {
            return AllocateLargeBlock(blockSize);
        }

        public void RequestLargeBlock(string correlationId, uint blockSize)
        {
            var responder = OperationContext.Current.GetCallbackChannel<ILargeBlockResponse>();

            try 
            {
                responder.OnRequestLargeBlock(correlationId, AllocateLargeBlock(blockSize));
            }
            catch (Exception e)
            {
                responder.OnRequestLargeBlockFailure(correlationId,
                    String.Format("OnRequestLargeBlock request error -- {0}", e.Message));
            }
        } 
        #endregion

        #region utility methods
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Allocates a array of bytes given a blockSize.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        private byte[] AllocateLargeBlock(uint blockSize)
        {
            var block = new byte[blockSize];

            if (blockSize <= 256)
            {
                for (int i=0; i<blockSize; ++i)
                    block[i] = 25;
            }

            return block;
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
