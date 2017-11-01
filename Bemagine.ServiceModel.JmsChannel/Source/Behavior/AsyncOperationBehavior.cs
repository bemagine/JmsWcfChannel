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
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Description;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// An IEndpointBehavior that registers itself as an IDispatchMessageInspector that adds
    /// the AsyncEmsOperationContext extension context to the current OperationContext extentions
    /// collection.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class AsyncEmsOperationBehavior : IEndpointBehavior, IDispatchMessageInspector
    {
        //----------------------------------------------------------------------------------------//
        /// <remarks>
        /// Provides the ability to pass custom data to binding elements to support the endpoint 
        /// configuration.
        /// </remarks>
        //----------------------------------------------------------------------------------------// 

        public void AddBindingParameters(ServiceEndpoint endpoint, 
            BindingParameterCollection bindingParameters)
        {            
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Throws an InvalidOperationException as queue throttling is no a behavior supported
        /// by client endpoints.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void ApplyClientBehavior(ServiceEndpoint endpoint, 
            ClientRuntime clientRuntime)
        {
            throw new InvalidOperationException(
                "AsyncEmsOperationBehavior behavior cannot be applied to client endpoints.");
        }
    
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Adds a new instance of the QueueThrottle dispatch message inspector to the endpoint's
        /// dispatch runtime message inspectors list.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, 
            EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(
                new AsyncEmsOperationBehavior());
        }        

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Inspects the service endpoint to verify if the behavior is appropriate for the 
        /// specified endpoint scheme.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public void Validate(ServiceEndpoint endpoint)
        {       
            //if (endpoint.ListenUri.Scheme != BindingPropertyDefaults.Scheme)
            //{
            //    throw new InvalidOperationException( 
            //        string.Format(
            //            "The AsyncEmsOperationBehavior behavior is not valid for endpoint the "+
            //            "scheme {0}. Verify the endpoint scheme is intended. If not set the scheme "+
            //            "to {1} to apply this behavior.", endpoint.ListenUri.Scheme, 
            //            BindingPropertyDefaults.Scheme));
            //}
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Adds a new instance of the AsyncEmsOperationContext extension context to the current 
        /// OperationContext extentions collection.
        /// </summary>
        //----------------------------------------------------------------------------------------//        

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, 
            InstanceContext instanceContext)
        {
            AsyncOperationContext.AddToOperationContext();
            return null;
        }

        //----------------------------------------------------------------------------------------//
        /// <remarks>
        /// Logically, we may consider removing the extension operation context after sending the
        /// reply, but that would be premature in the use case this context extension addtesses.
        /// When the operation returns this method is invoked regardless if there's a response
        /// to send or not (in one-way communication the reply message is always null). There's
        /// the case where the operation simply invokes a subsequent asynchronous operation and
        /// returns. On some response event some subsequent callback may be invoked as a response
        /// to the initial request. The AsyncEmsOperationContext instance must be available at
        /// that point for the communication choreography to work. Therefore, we do not remove
        /// the custom context in this method.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            // AsyncEmsOperationContext.RemoveFromOperationContext();   
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
