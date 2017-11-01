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
    /// JmsQueueChannelBase provides the ChannelBase override implementation for managing 
    /// CommunicationObject state transitions. Additionally, provides the override for channel
    /// property interogation.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal class JmsChannelBase : ChannelBase
    {
        //----------------------------------------------------------------------------------------//
        // data members and member properties
        //----------------------------------------------------------------------------------------//
       
        public IJmsChannelManager QueueChannelManager { get; private set; }

        //----------------------------------------------------------------------------------------//
        // utility properties
        //----------------------------------------------------------------------------------------//

        public bool IsClient
        {
            get { return QueueChannelManager.IsClient; }
        }

        public TimeSpan InfiniteTimeout { get; private set; }        

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        protected JmsChannelBase(ChannelManagerBase channelManagerBase) 
            : base(channelManagerBase)
        {
            QueueChannelManager = channelManagerBase as IJmsChannelManager;

            if (QueueChannelManager == null)
            {
                throw new ArgumentException("Expecting IJmsQueueChannelManager", "channelManagerBase");
            }
            
            InfiniteTimeout = new TimeSpan(System.Threading.Timeout.Infinite);
        }

        #region ChannelBase overrides
        //----------------------------------------------------------------------------------------//
        // ChannelBase overrides
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The standard IChannel object query interface. This override intercepts queries
        /// to the encoder, and marshalls these queries to the embedded MessageEncoderFactory.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override T GetProperty<T>()
        {
            var messageEncoderProperty = 
                QueueChannelManager.MessageEncoderFactory.Encoder.GetProperty<T>();

            return messageEncoderProperty ?? base.GetProperty<T>();
        }
        #endregion

        #region CommunicationObject overrides
        //----------------------------------------------------------------------------------------//
        // CommunicationObject overrides
        //----------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------//
        // channel construction methods
        //----------------------------------------------------------------------------------------//

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, 
            Object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        //----------------------------------------------------------------------------------------//
        // channel deconstruction methods
        //----------------------------------------------------------------------------------------//

        protected override void OnAbort()
        {
            Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        { 
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, 
            Object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
