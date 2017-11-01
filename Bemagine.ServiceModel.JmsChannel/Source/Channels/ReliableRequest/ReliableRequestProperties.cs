﻿//------------------------------------------------------------------------------------------------//
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

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The common set of reliable request protocol properties specified in each JMS message
    /// sent via the ReliableRequestChannel.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class ReliableRequestProperties
    {
        //----------------------------------------------------------------------------------------//
        // member properties
        //----------------------------------------------------------------------------------------//

        public static string ServiceHost { get; private set; }
        public static string ServiceInstance { get; private set; }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        static ReliableRequestProperties()
        {
            ServiceHost = Environment.MachineName;
            ServiceInstance = Guid.NewGuid().ToString();
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
