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

    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Interface to be implemented by JMS queue channel managers.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    interface IJmsChannelManager
    {
    //--------------------------------------------------------------------------------------------//
    // data memebers and member properties
    //--------------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//    
        /// <summary>
        /// The buffer manager used by channels to transform messages.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        BufferManager BufferManager { get; }
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The encoder factory that creates encoders to encode and decode messages. The default
        /// encoder is the BinaryMessageEncoder
        /// </summary>
        //----------------------------------------------------------------------------------------//

        MessageEncoderFactory MessageEncoderFactory { get; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The set of JMS queue channel properties required to construct the channels.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        JmsTransportContext TransportProperties { get; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the channel stack was instantiated in the client context.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        bool IsClient { get; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the JmsTransport initialized by the channel manager.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        JmsTransport Transport { get; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        QueueThrottle Throttle { get; set; }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
