//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.Jms.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Jms
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Utility that wraps calls and converts any exceptions into Jms.JmsExceptions.
    /// </summary>
    /// <remarks>
    /// This is strictly not required as implementers call allow JMS Provider specific exceptions
    /// to bubble up to the caller. At that point the caller can simply handle the generic 
    /// Exception case without knowledge of the underlying type. This solution ensures a level
    /// of consistency that permits callers to identify exceptions generated from the JMS
    /// provider / wrapper impelementation.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    public static class SafeProxy
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Converts exceptions generated via a void return method call (i.e. Action) into
        /// JmsExceptions.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static void SafeCall(Action action)
        {
            try { action(); }
            catch (Exception e)
            {
                throw new Jms.JmsException(e.Message, e.InnerException);
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Converts exceptions generated via a T return method call (i.e. Func) into JmsExceptions.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static T SafeCall<T>(Func<T> action)
        {
            try { return action(); }
            catch (Exception e)
            {
                throw new Jms.JmsException(e.Message, e.InnerException);
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
