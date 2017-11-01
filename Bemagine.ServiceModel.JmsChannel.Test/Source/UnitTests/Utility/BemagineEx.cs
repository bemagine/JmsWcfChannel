//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.Test.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.JmsChannel.Test
{
    //---------------------------------------------------------------------------------------------//
    // using directives
    //---------------------------------------------------------------------------------------------//

    using System;
    using System.Linq;
    using System.ServiceModel.Channels;

    using Bemagine.ServiceModel.Channels;

    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// An ubiquitous set of extension methods for classes in the Bemagine namespace (excludes this
    /// test suite).
    /// </summary>
    //---------------------------------------------------------------------------------------------//

    internal static class BemagineEx
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Generalized extention method that determines the null-ness of the specified "this"
        /// parameter.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static bool IsNull<T>(this T t)
        {
            return (null == t);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static U IfNotNull<T, U>(this T t, Func<T, U> fn)
        {
            return !t.IsNull() ? fn(t) : default(U);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a message with the default message version and a default action.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static Message CreateDefaultMessage()
        {
            return Message.CreateMessage(MessageVersion.Default, "DefaultMessageAction");
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Simulates message reception by a service endpoint that increments the QueueThrottle's
        /// concurrently executing WCF calls counter by the number of simulations specified.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static void SimulateMessageReception(this QueueThrottle queueThrottle, 
            int nSimulations)
        {
            Message message = CreateDefaultMessage();

            foreach (int i in Enumerable.Range(1, nSimulations))
                queueThrottle.AfterReceiveRequest(ref message, null, null);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Simulates a message reply by a service endpoint that decrements the QueueThrottle's
        /// concurrently executing WCF calls counter by the number of simulations specified.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static void SimulateMessageReply(this QueueThrottle queueThrottle,
            int nSimulations)
        {
            Message message = CreateDefaultMessage();

            foreach (int i in Enumerable.Range(1, nSimulations))
                queueThrottle.BeforeSendReply(ref message, null);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Calculates the sum of the sequence of natural numbers from 1 to n.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static int SequenceSum(this int n)
        {
            return (n * (n + 1)) / 2;
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
