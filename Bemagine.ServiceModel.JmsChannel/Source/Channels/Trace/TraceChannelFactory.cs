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
    /// Constructs instances of the TraceChannel and marshalls state transition calls to the inner 
    /// channel factory.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class TraceChannelFactory : ChannelFactoryBase<IDuplexChannel>
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
    
        private readonly IChannelFactory<IDuplexChannel> _innerChannelFactory;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public TraceChannelFactory(IChannelFactory<IDuplexChannel> innerChannelFactory)
        {
            LogUtility.Tracer();            
            _innerChannelFactory = innerChannelFactory;
        }

        //----------------------------------------------------------------------------------------//
        // ChannelFactoryBase overrides
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        // properties
        //----------------------------------------------------------------------------------------//

        public override T GetProperty<T>()
        {
            return _innerChannelFactory.GetProperty<T>();
        }

        //----------------------------------------------------------------------------------------//
        // channel shutdown
        //----------------------------------------------------------------------------------------//

        protected override void OnAbort()
        {
            LogUtility.Tracer();
            _innerChannelFactory.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            LogUtility.Tracer();
            _innerChannelFactory.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, 
            object state)
        {
            LogUtility.Tracer();
            return _innerChannelFactory.BeginClose(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            LogUtility.Tracer();
            _innerChannelFactory.EndClose(result);
        }

        //----------------------------------------------------------------------------------------//
        // channel open
        //----------------------------------------------------------------------------------------//

        protected override void OnOpen(TimeSpan timeout)
        {
            LogUtility.Tracer();
            _innerChannelFactory.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, 
            object state)
        {
            LogUtility.Tracer();
            return _innerChannelFactory.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            LogUtility.Tracer();
            _innerChannelFactory.EndOpen(result);
        }

        //----------------------------------------------------------------------------------------//
        // channel instance construction
        //----------------------------------------------------------------------------------------//

        protected override IDuplexChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            LogUtility.Tracer();

            return new TraceChannel(this, _innerChannelFactory.CreateChannel(address, via), true);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
