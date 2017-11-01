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
    /// Exposes the TIBCO EMS Destination implementation as an IDestination.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    internal sealed class EmsDestination : IDestination
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
        
        private readonly TIBCO.EMS.Destination _innerDestination;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        internal EmsDestination(TIBCO.EMS.Destination innerDestination)
        {
            _innerDestination = innerDestination;
        }   
     
        //----------------------------------------------------------------------------------------//
        // implicit conversion operator
        //----------------------------------------------------------------------------------------//

        public static implicit operator TIBCO.EMS.Destination(EmsDestination emsDestination)
        {
            return (emsDestination != null) ? emsDestination._innerDestination : null;
        }

        //----------------------------------------------------------------------------------------//
        // IDestination interface implementation
        //----------------------------------------------------------------------------------------//

        public string Name 
        { 
            get 
            { 
                return SafeProxy.SafeCall(
                    () =>
                    {
                        return (_innerDestination is TIBCO.EMS.Queue) ?
                            ((TIBCO.EMS.Queue) _innerDestination).QueueName :
                            ((TIBCO.EMS.Topic) _innerDestination).TopicName;
                    });
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
