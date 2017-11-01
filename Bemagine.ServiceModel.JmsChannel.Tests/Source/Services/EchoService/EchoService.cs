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
    /// The logical (WCF) service that implements the IEcho interface.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public sealed class EchoService : IEcho
    {
        #region IEcho interface implmentation
        //----------------------------------------------------------------------------------------//
        // IEcho interface implmentation
        //----------------------------------------------------------------------------------------//

        public string SyncGreet(string message)
        {
            return "[SyncGreet(Echo)] " + message;
        }

        public void AsyncGreet(string correlationId, string message)
        {
            var responder = OperationContext.Current.GetCallbackChannel<IEchoResponse>();

            try 
            {
                responder.OnAsyncGreet(correlationId, "[AsyncGreet(Echo)] " + message);
            }
            catch (Exception e)
            {
                responder.OnAsyncGreetFailure(correlationId,
                    String.Format("[AsyncGreet(Echo)] error -- {0}", e.Message));
            }
        } 
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
