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
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class TraceChannel : TraceChannelBase<IDuplexChannel>, IDuplexChannel
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly bool _isClient;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public TraceChannel(ChannelManagerBase channelManagerBase, IDuplexChannel innerChannel, 
            bool isClient)
                : base(channelManagerBase, innerChannel)
        {
            LogUtility.Tracer();
            _isClient = isClient;
        }

        //----------------------------------------------------------------------------------------//
        // member properties
        //----------------------------------------------------------------------------------------//

        public EndpointAddress LocalAddress 
        {
            get { return InnerChannel.LocalAddress; }
        }

        public EndpointAddress RemoteAddress 
        {
            get { return InnerChannel.RemoteAddress; }
        }

        public Uri Via 
        {
            get { return InnerChannel.Via; }
        }

        //private Uri ServiceUri
        //{
        //    get
        //    {
        //        if (_isClient) return Via;
        //        return LocalAddress.Uri;
        //    }
        //}  

        //----------------------------------------------------------------------------------------//
        // IInputChannel implementation
        //----------------------------------------------------------------------------------------//

        public Message Receive() 
        {
            LogUtility.Tracer();
            return InnerChannel.Receive();
        }

        public Message Receive(TimeSpan timeout)
        {
            LogUtility.Tracer();
            return InnerChannel.Receive(timeout);
        }        

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            LogUtility.Tracer();
            return InnerChannel.BeginReceive(callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            LogUtility.Tracer();
            return InnerChannel.BeginReceive(timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            LogUtility.Tracer();
            return InnerChannel.EndReceive(result);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            LogUtility.Tracer();
            return InnerChannel.TryReceive(timeout, out message);
        }        

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, 
            object state)
        {
            LogUtility.Tracer();
            return InnerChannel.BeginTryReceive(timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            LogUtility.Tracer();
            return InnerChannel.EndTryReceive(result, out message); 
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            LogUtility.Tracer();
            return InnerChannel.WaitForMessage(timeout);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback,
            object state)
        {
            LogUtility.Tracer();
            return InnerChannel.BeginWaitForMessage(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            LogUtility.Tracer();
            return InnerChannel.EndWaitForMessage(result);
        }

        //----------------------------------------------------------------------------------------//
        // IOuptutChannel implementation
        //----------------------------------------------------------------------------------------//

        public void Send(Message message)
        {
            LogUtility.Tracer();
            InnerChannel.Send(message);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            LogUtility.Tracer();
            InnerChannel.Send(message, timeout);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            LogUtility.Tracer();
            return InnerChannel.BeginSend(message, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, 
            object state) 
        {
            LogUtility.Tracer();
            return InnerChannel.BeginSend(message, timeout, callback, state);
        }        

        public void EndSend(IAsyncResult result)
        {
            LogUtility.Tracer();
            InnerChannel.EndSend(result);
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
