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
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System.Configuration;

    //--------------------------------------------------------------------------------------------//
    // using aliases
    //--------------------------------------------------------------------------------------------//

    using BPN = EmsConnectionFactoryPropertyNames;
    using BPD = EmsConnectionFactoryPropertyDefaults;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public class EmsConnectionFactoryConfigurationSection : ConfigurationSection
    {
        //----------------------------------------------------------------------------------------//
        // ConnectionAttempts
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.ConnectionAttempts, DefaultValue=BPD.ConnectionAttempts)]
        [IntegerValidator(MinValue=BPD.ConnectionAttemptsMinimum)]
        public int ConnectionAttempts 
        { 
            get { return (int) base[BPN.ConnectionAttempts];  }
            set { base[BPN.ConnectionAttempts] = value;  }
        }

        //----------------------------------------------------------------------------------------//
        // ConnectionAttemptDelay
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.ConnectionAttemptDelay, DefaultValue=BPD.ConnectionAttemptDelay)]
        [IntegerValidator(MinValue=BPD.ConnectionAttemptDelayMinimum)]
        public int ConnectionAttemptDelay 
        { 
            get { return (int) base[BPN.ConnectionAttemptDelay];  }
            set { base[BPN.ConnectionAttemptDelay] = value;  }
        }

        //----------------------------------------------------------------------------------------//
        // ConnectionAttemptTimeout
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.ConnectionAttemptTimeout, DefaultValue=BPD.ConnectionAttemptTimeout)]
        [IntegerValidator(MinValue=BPD.ConnectionAttemptTimeoutMinimum)]
        public int ConnectionAttemptTimeout 
        { 
            get { return (int) base[BPN.ConnectionAttemptTimeout];  }
            set { base[BPN.ConnectionAttemptTimeout] = value;  }
        }

        //----------------------------------------------------------------------------------------//
        // ReconnectionAttempts
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.ReconnectionAttempts, DefaultValue=BPD.ReconnectionAttempts)]
        [IntegerValidator(MinValue=BPD.ReconnectionAttemptsMinimum)]
        public int ReconnectionAttempts 
        { 
            get { return (int) base[BPN.ReconnectionAttempts];  }
            set { base[BPN.ReconnectionAttempts] = value;  }
        }

        //----------------------------------------------------------------------------------------//
        // ReconnectionAttemptDelay
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.ReconnectionAttemptDelay, DefaultValue=BPD.ReconnectionAttemptDelay)]
        [IntegerValidator(MinValue=BPD.ReconnectionAttemptDelayMinimum)]
        public int ReconnectionAttemptDelay 
        { 
            get { return (int) base[BPN.ReconnectionAttemptDelay];  }
            set { base[BPN.ReconnectionAttemptDelay] = value;  }
        }

        //----------------------------------------------------------------------------------------//
        // ReconnectionAttemptTimeout
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.ReconnectionAttemptTimeout, DefaultValue=BPD.ReconnectionAttemptTimeout)]
        [IntegerValidator(MinValue=BPD.ReconnectionAttemptTimeout)]
        public int ReconnectionAttemptTimeout 
        { 
            get { return (int) base[BPN.ReconnectionAttemptTimeout];  }
            set { base[BPN.ReconnectionAttemptTimeout] = value;  }
        }            
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//

