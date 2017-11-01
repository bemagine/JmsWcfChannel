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

    using System.ServiceModel;

    //--------------------------------------------------------------------------------------------//
    // contracts
    //--------------------------------------------------------------------------------------------//

    [ServiceContract(CallbackContract = typeof(ILargeBlockResponse))]
    public interface ILargeBlock
    {
        [OperationContract]
        byte[] GetLargeBlock(uint blockSize);

        [OperationContract(IsOneWay = true)]
        void RequestLargeBlock(string correlationId, uint blockSize);
    }

    [ServiceContract(Name = "ILargeBlockResponse")]
    public interface ILargeBlockResponse
    {
        [OperationContract(IsOneWay = true)]
        void OnRequestLargeBlock(string correlationId, byte[] largeBlock);

        [OperationContract(IsOneWay = true)]
        void OnRequestLargeBlockFailure(string correlationId, string errorMessage);
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
