//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Reflection;
    using System.ServiceModel;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Represents the exceptional case when a service contract operation is not specified as a
    /// one-way operation.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class InvalidPubSubContractException : Exception
    {
        public InvalidPubSubContractException()
        {
        }

        public InvalidPubSubContractException(string message) : base(message)
        {
        }

        public InvalidPubSubContractException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Provides a set of static utility methods for validating PubSub contract implementations.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class PubSubContractValidator
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The publish-subsribe message exchange pattern is inherently one-way. This utility 
        /// ensures that all operations of a given contract are specified as one-way.
        /// </summary>
        /// <remarks>
        /// This is a reasonable policy that ensures the publisher does not require a response
        /// from a client. This is important for a couple of reasons. First, a publisher generally
        /// broadcasts information to a number of subscribers. In this case, which subscriber 
        /// should respond. Finally, if the publisher was required to wait for a response from a 
        /// subscriber this could interupt the flow of data should none respond.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        public static void AllOperationsAreOneWay<IServiceContractT>()
        {
            foreach (MemberInfo memberInfo in typeof(IServiceContractT).GetMembers())
            {
                var attribute = 
                    Attribute.GetCustomAttribute(memberInfo, typeof(OperationContractAttribute)) 
                        as OperationContractAttribute;

                if ((attribute == null) || (!attribute.IsOneWay))
                {
                    throw new InvalidPubSubContractException(
                        String.Format(
                            "The PubSub message exchange pattern as implemented by this library "+
                            "requires all operations to be implemented as one-way. The operation "+
                            "{0} of the {1} contract violates this requirement.", 
                            attribute != null ? attribute.Name : "UNKOWN", 
                            typeof(IServiceContractT).Name));
                }                    
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
