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
    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Provides the base CommunicationObject overrides for handling state transitions. Calls to
    /// these methods are marshalled to the InnerChannel. Additionally provides the override for 
    /// channel property interogation.
    /// </summary>
    /// <typeparam name="TShape">
    /// The expected channel shape in this case is IDuplexChannel.
    /// </typeparam>
    //--------------------------------------------------------------------------------------------//

    internal class TraceChannelBase<TShape> : ChannelBase where TShape : class, IDuplexChannel
    {
        //----------------------------------------------------------------------------------------//
        // data members & member properties
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The reference to the next channel in the stack.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected TShape InnerChannel { get; private set; }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        protected TraceChannelBase(
            ChannelManagerBase channelManagerBase, TShape innerChannel)
                : base(channelManagerBase)
        {
            LogUtility.Tracer();

            if (innerChannel == null)
            {
                throw new ArgumentNullException(
                     "innerChannel", "TraceChannelBase requires a non-null channel.");
            }

            InnerChannel = innerChannel;
        }

        //----------------------------------------------------------------------------------------//
        // ChannelBase overrides
        //----------------------------------------------------------------------------------------//

        public override T GetProperty<T>()
        {
            return InnerChannel.GetProperty<T>();
        }

        //----------------------------------------------------------------------------------------//
        // CommunicationObjet overrides
        //----------------------------------------------------------------------------------------//

        protected override void OnOpen(TimeSpan timeout)
        {
            LogUtility.Tracer();
            InnerChannel.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback,
            Object state)
        {
            LogUtility.Tracer();
            return InnerChannel.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            LogUtility.Tracer();
            InnerChannel.EndClose(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            LogUtility.Tracer();
            InnerChannel.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback,
            Object state)
        {
            LogUtility.Tracer();
            return InnerChannel.BeginClose(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            LogUtility.Tracer();
            InnerChannel.EndOpen(result);
        }

        protected override void OnAbort()
        {
            LogUtility.Tracer();
            InnerChannel.Abort();
        }

        protected override void OnFaulted()
        {
            LogUtility.Tracer();
        }        
    }
}

//--------------------------------------------------------------------------------------------------//
// end of file
//--------------------------------------------------------------------------------------------------//
