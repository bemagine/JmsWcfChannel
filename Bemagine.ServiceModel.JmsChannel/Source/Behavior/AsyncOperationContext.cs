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

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The AsyncOperationContext is an OperationContext extention that captures the ReplyTo
    /// URI during an request call for subsequent use out side of the initial / request operation
    /// context (i.e. an asynchronous callback).
    /// </summary>
    /// <remarks>
    /// The use case that this context addresses is described as follows. consider an operation 
    /// that is async that invokes another async operation and waits for a response. Upon arrival 
    /// of that response it may callback to the initial caller in a different thread than the 
    /// initial call occurred. The original request message has been disposed. Therefore, an 
    /// OperationContext extension is required to maintain the context of the original call. 
    /// AsyncEmsOperationContext provides that mechanism. 
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    public sealed class AsyncOperationContext : IExtension<OperationContext>
    {    
    //--------------------------------------------------------------------------------------------//
    // data memebers and member properties
    //--------------------------------------------------------------------------------------------//

        public Uri ReplyToUri { get; private set; }

    //--------------------------------------------------------------------------------------------//
    // public interface
    //--------------------------------------------------------------------------------------------//

        public static AsyncOperationContext Current
        {
            get { return OperationContext.Current.Extensions.Find<AsyncOperationContext>(); }
        }

        public static void AddToOperationContext()
        {
            OperationContext.Current.Extensions.Add(new AsyncOperationContext());
        }

        public static void RemoveFromOperationContext()
        {
            OperationContext.Current.Extensions.Remove(AsyncOperationContext.Current);
        }

    //--------------------------------------------------------------------------------------------//
    // IExtension<OperationContext> interface implementation
    //--------------------------------------------------------------------------------------------//

        public void Attach(OperationContext context)
        {
            MessageHeaders headers = context.IncomingMessageHeaders;

            if ((headers != null) && (headers.ReplyTo != null))
                ReplyToUri = headers.ReplyTo.Uri;
        }

        public void Detach(OperationContext context)
        {            
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
