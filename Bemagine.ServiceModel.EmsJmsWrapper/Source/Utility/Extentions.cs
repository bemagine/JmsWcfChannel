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

    using Bemagine.ServiceModel.Jms;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Static utility that implements various extention methods
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class Extentions
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Converts Bemagine.Jms.AcknowledgeMode enum to TIBCO.EMS.SessionMode enum.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static TIBCO.EMS.SessionMode ToEms(this AcknowledgeMode acknowledgeMode)
        {
            switch (acknowledgeMode)
            {
                case AcknowledgeMode.ClientAcknowledge:
                    return TIBCO.EMS.SessionMode.ClientAcknowledge;

                case AcknowledgeMode.DupsOkAcknowledge:
                    return TIBCO.EMS.SessionMode.DupsOkAcknowledge;

                case AcknowledgeMode.SessionTransacted:
                    return TIBCO.EMS.SessionMode.SessionTransacted;

                case AcknowledgeMode.AutoAcknowledge: 
                default:
                    return TIBCO.EMS.SessionMode.AutoAcknowledge;
            }
        }  
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
