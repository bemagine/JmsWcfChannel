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
    /// ChannelManagerBase extension methods.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class ChannelManagerBaseEx
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Proxies a call to GetProperty of the ChannelManagerBase where upcasts to either
        /// a ChannelListenerBase or ChannelFactoryBase.
        /// </summary>
        /// <remarks>
        /// Not really sure why this method was not specified in the ChannelManagerBase class.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        public static T GetProperty<T>(this ChannelManagerBase channelManagerBase) where T : class
        {
            var listenerBase = channelManagerBase as ChannelListenerBase;
            if (listenerBase != null)
            {
                return listenerBase.GetProperty<T>();
            }

            var factoryBase = channelManagerBase as ChannelFactoryBase;
            if (factoryBase != null)
            {
                return factoryBase.GetProperty<T>();
            }

            return null;
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
