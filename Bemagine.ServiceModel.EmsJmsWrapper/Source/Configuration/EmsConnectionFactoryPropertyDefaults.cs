//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.EmsJmsWrapper.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.EmsJmsWrapper
{
    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// EmsConnectionFactoryPropertyNames defines the set of property names specific to the
    /// configuration of the EmsConnectionFactory.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class EmsConnectionFactoryPropertyDefaults
    {
        public const int ConnectionAttempts          = 2;
        public const int ConnectionAttemptDelay      = 1000;
        public const int ConnectionAttemptTimeout    = 1000;
        public const int ReconnectionAttempts        = 2;
        public const int ReconnectionAttemptDelay    = 1000;
        public const int ReconnectionAttemptTimeout  = 1000;

        public const int ConnectionAttemptsMinimum          = 1;
        public const int ConnectionAttemptDelayMinimum      = 100;
        public const int ConnectionAttemptTimeoutMinimum    = 100;
        public const int ReconnectionAttemptsMinimum        = 1;
        public const int ReconnectionAttemptDelayMinimum    = 100;
        public const int ReconnectionAttemptTimeoutMinimum  = 100;
        public const long MaxBufferPoolSizeMinimum          = 0L;
        public const long MaxReceivedMessageSizeMinimum     = 1L;
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
