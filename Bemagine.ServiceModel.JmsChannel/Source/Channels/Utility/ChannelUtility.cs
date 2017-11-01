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
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Provides a set of static channel utility methods.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class ChannelUtility
    {
        //----------------------------------------------------------------------------------------//
        // channel helper interfaces
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Ensures that that the TimeSpan value is not less than zero. An exception is generated
        /// if the TimeSpan value is not valid.
        /// </summary>
        //----------------------------------------------------------------------------------------//        

        public static void ValidateTimeout(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout", timeout, 
                    @"Timeout must be greater than or equal to TimeSpan.Zero. To disable timeout,"+
                    @"specify TimeSpan.MaxValue.");
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a timeout exception in response to an IInputChannel receive timeout.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static TimeoutException CreateReceiveTimedOutException(IInputChannel channel, 
            TimeSpan timeout)
        {
            if (channel.LocalAddress != null)
            {
                return new TimeoutException(
                    string.Format("Receive on local address {0} timed out after {1}.",
                    channel.LocalAddress.Uri.AbsoluteUri, timeout));
            }

            return new TimeoutException(
                string.Format("Receive timed out after {0}.",
                timeout));
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a timeout exception in response to a listener accept timeout.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static TimeoutException CreateAcceptTimeoutException(Uri uri, TimeSpan timeout)
        {
            if (uri != null)
            {
                return new TimeoutException(
                    string.Format("Accept on listener at address {0} timed out after {1}.",
                    uri.AbsoluteUri, timeout));
            }

            return new TimeoutException(
                string.Format("Accept on listener timed out after {0}.", timeout));
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Logs the exception as an error and throws a communication exception with the initial
        /// exception specified as the inner exception.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static void LogErrorAndThrowCommunicationException(
            string errorMessageFormat, Exception innerException, params object[] details)
        {
            var errorMessage = String.Format(errorMessageFormat, details);
            LogUtility.Error(errorMessage);
            
            throw new CommunicationException(errorMessage, innerException); 
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Logs the exception as a warning and throws a communication exception with the initial
        /// exception specified as the inner exception.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static void LogWarningAndThrowCommunicationException(
            string errorMessageFormat, Exception innerException, params object[] details)
        {
            var errorMessage = String.Format(errorMessageFormat, details);
            LogUtility.Warning(errorMessage);

            throw new CommunicationException(errorMessage, innerException); 
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
