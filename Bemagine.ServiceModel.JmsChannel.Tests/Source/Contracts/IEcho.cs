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

    [ServiceContract(Name = "IEcho", CallbackContract = typeof(IEchoResponse))]
    public interface IEcho
    {
        [OperationContract]
        string SyncGreet(string message);

        [OperationContract(IsOneWay = true)]
        void AsyncGreet(string correlationId, string message);
    }

    [ServiceContract(Name = "IEchoResponse")]
    public interface IEchoResponse
    {
        [OperationContract(IsOneWay = true)]
        void OnAsyncGreet(string correlationId, string message);

        [OperationContract(IsOneWay = true)]
        void OnAsyncGreetFailure(string correlationId, string message);
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
