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

    internal static class EmsConnectionFactoryPropertyNames
    {
        public const string ConnectionAttempts          = "connectionAttempts";
        public const string ConnectionAttemptDelay      = "connectionAttemptDelay";
        public const string ConnectionAttemptTimeout    = "connectionAttemptTimeout";
        public const string ReconnectionAttempts        = "reconnectionAttempts";
        public const string ReconnectionAttemptDelay    = "reconnectionAttemptDelay";
        public const string ReconnectionAttemptTimeout  = "reconnectionAttemptTimeout";
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
