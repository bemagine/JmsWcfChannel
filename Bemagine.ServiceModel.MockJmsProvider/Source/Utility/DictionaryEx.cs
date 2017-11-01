//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.MockJmsProvider.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.MockJmsProvider
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using Bemagine.ServiceModel.Jms;

    using System;
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Defines a set of extention methods for System.Collections.Generic.Dictionary.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class DictionaryEx
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Attempts to retrieve a dictionary element corresponding to the specified key. If the
        /// value exists, it is cast to the type T. Any excpetions generated during the execution
        /// of this method are translated into a JmsException.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static T FindAs<T>(this Dictionary<string, object> dictionary, string key) 
        {
            try
            {
                return (T) dictionary[key];
            }
            catch (Exception e)
            {
                throw new JmsException(
                    string.Format(
                        "The dictionary either does not contain the object with key {0} or the " +
                        "object type does not match the conversion type {1}", key, typeof(T).Name),
                    e);
            }
        }

    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
