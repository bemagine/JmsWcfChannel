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
    /// Marshalls calls to the inner channel listener.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class ReliableRequestChannelListener 
        : ChannelListenerBase<IDuplexChannel>
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly IChannelListener<IDuplexChannel> _innerChannelListener;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public ReliableRequestChannelListener (
            IChannelListener<IDuplexChannel> innerChannelListener)
        {
            _innerChannelListener = innerChannelListener;
        }

        //----------------------------------------------------------------------------------------//
        // ChannelListenerBase overrides
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        // properties
        //----------------------------------------------------------------------------------------//

        public override Uri Uri
        {
            get { return _innerChannelListener.Uri; }
        }

        public override T GetProperty<T>()
        {
            return _innerChannelListener.GetProperty<T>();
        }

        //----------------------------------------------------------------------------------------//
        // channel shutdown
        //----------------------------------------------------------------------------------------//

        protected override void OnAbort()
        {
            _innerChannelListener.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _innerChannelListener.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, 
            object state)
        {
            return _innerChannelListener.BeginClose(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            _innerChannelListener.EndClose(result);
        }

        //----------------------------------------------------------------------------------------//
        // channel open
        //----------------------------------------------------------------------------------------//

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerChannelListener.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, 
            object state)
        {
            return _innerChannelListener.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _innerChannelListener.EndOpen(result);
        }
        
        //----------------------------------------------------------------------------------------//
        // channel listening
        //----------------------------------------------------------------------------------------//        

        protected override IDuplexChannel OnAcceptChannel(TimeSpan timeout)
        {
            IDuplexChannel innerChannel = _innerChannelListener.AcceptChannel(timeout);

            return (innerChannel == null) ? 
                null : new ReliableRequestChannel(this, innerChannel, false);
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, 
            AsyncCallback callback, object state)
        {
            return _innerChannelListener.BeginAcceptChannel(timeout, callback, state);
        }

        protected override IDuplexChannel OnEndAcceptChannel(IAsyncResult result)
        {
            IDuplexChannel innerChannel = _innerChannelListener.EndAcceptChannel(result);

            return (innerChannel == null) ? 
                null : new ReliableRequestChannel(this, innerChannel, false);
        }

        //----------------------------------------------------------------------------------------//
        // channel waiting
        //----------------------------------------------------------------------------------------//

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return _innerChannelListener.WaitForChannel(timeout);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, 
            AsyncCallback callback, object state)
        {
            return _innerChannelListener.BeginWaitForChannel(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return _innerChannelListener.EndWaitForChannel(result);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
