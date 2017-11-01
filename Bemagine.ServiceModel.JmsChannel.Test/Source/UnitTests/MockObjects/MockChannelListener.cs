//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.Test.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.JmsChannel.Test
{
    //---------------------------------------------------------------------------------------------//
    // using directives
    //---------------------------------------------------------------------------------------------//

    using System;
    using System.ServiceModel.Channels;

    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Mock System.ServiceModel.Channels.ChannelListenerBase to streamline unit testing of the
    /// internal APIs of Bemagine.ServiceModel.JmsChannel assembly.
    /// </summary>
    //---------------------------------------------------------------------------------------------//

    internal sealed class MockChannelListener : ChannelListenerBase
    {
        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(ExistentChannelProperty))
                return new ExistentChannelProperty() as T;

            return base.GetProperty<T>();
        }

        protected override void OnAbort()
        {
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override void OnEndClose(IAsyncResult result)
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            var emptyAction = new Action(() => {});
            return emptyAction.BeginInvoke(OnEndClose, null);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            var emptyAction = new Action(() => { });
            return emptyAction.BeginInvoke(OnEndOpen, null);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return true;
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            var emptyAction = new Action(() => { });
            return emptyAction.BeginInvoke(OnEndClose, null);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return true;
        }

        public override Uri Uri { get { return _uri; } }
        private readonly Uri _uri = new Uri("net.jmsq://someHost/someQueue");
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
