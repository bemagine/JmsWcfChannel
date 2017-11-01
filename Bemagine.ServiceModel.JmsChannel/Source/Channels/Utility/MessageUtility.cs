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

    using System;
    using System.ServiceModel.Channels;
    using System.Runtime.Serialization;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Provides a set of static WCF message utility methods.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class MessageUtility
    {
        //----------------------------------------------------------------------------------------//
        // channel helper interfaces
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Adds the specified messageHeader as a header to the specified message. It is expected
        /// that the messageHeader provided is properly decorated with the DataContract attribute.
        /// Additionally, the Name and Namespace should be explicitly set for the attribute.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static bool AddHeader<T>(this Message message, T messageHeader)
        {   
            var dataContract = 
                Attribute.GetCustomAttribute(typeof(T), typeof(DataContractAttribute)) 
                    as DataContractAttribute;

            //------------------------------------------------------------------------------------//
            // Resharper will complain about the expression (messageHeader != null) because of a 
            // possible comparison of null with a value type. According to the C# specification 
            // the comparison of unbounded parameters (i.e. parameters without any constraints) to  
            // null when the type parameter is a value type always returns false. So, in this case
            // Resharper has got it wrong.
            //------------------------------------------------------------------------------------//

            if ((message != null) && (messageHeader != null) && (dataContract != null))
            {
                var header = 
                    MessageHeader.CreateHeader(
                        dataContract.Name, 
                        dataContract.Namespace, 
                        messageHeader,
                        false,
                        "",
                        true);

                message.Headers.Add(header);
                return true;
            }

            return false;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Tries to extract a message header of a given type from the message. If the header 
        /// exists the out parameter is set to that instance and true is returned to indicate
        /// operation success; otherwise, the out paramerter is set to null and false returned
        /// to indicate operation failure. It is expected that the messageHeader provided is 
        /// properly decorated with the DataContract attribute. Additionally, the Name and 
        /// Namespace should be explicitly set for the attribute.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static bool TryGetHeader<T>(this Message message, out T messageHeader)
        {
            bool headerFound = false;
            messageHeader = default(T);

            var dataContract = 
                Attribute.GetCustomAttribute(typeof(T), typeof(DataContractAttribute)) 
                    as DataContractAttribute;

            if ((message != null) && (dataContract != null))
            {
                MessageHeaders headers = message.Headers;
                int headerIndex = headers.FindHeader(dataContract.Name, dataContract.Namespace);

                if (headerIndex > 0)
                {
                    messageHeader = headers.GetHeader<T>(headerIndex);
                    headerFound = true;
                }
            }

            return headerFound;
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
