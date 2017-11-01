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
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A ReliableRequest represents the a tracked request dispatch to a service. State associated 
    /// with the original request such as the number of attempts to service the request along with 
    /// the message are maintained by this class.
    /// </summary>
    /// <remarks>
    /// The ReliableRequestManager provide details regarding the state transitions for reliable
    /// requests.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal sealed class ReliableRequest
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        public Message Message { get; private set; }
        public string CorrelationId { get; private set; }

        public uint Attempts { get; private set; }
        public uint MaxAttempts { get; private set; }

        public IDuplexChannel Channel { get; private set; }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public ReliableRequest(Message message, uint maxAttempts, IDuplexChannel channel)
        {
            Debug.Assert(message != null);
            Debug.Assert(channel != null);

            Message = message;
            CorrelationId = message.Headers.MessageId.ToString();
            MaxAttempts = maxAttempts;
            Channel = channel;
        }

        //----------------------------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns true if the number of send/resend attempts exceeds the max allowable attempts.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public bool IsFaulted() 
        {
            return (Attempts >= MaxAttempts);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// <para>
        /// Sends the message to the destination via the channel specified at the time of 
        /// construction. Each call to send() increments the attempts counter. False is 
        /// returned when the number of send attempts exceeds the max allowed attempts. This 
        /// indicates a fault where services died while processing messages. If this message is 
        /// faulted it may be the poison pill bringing services down. In any regard, we must give
        /// up and report an error.
        /// </para>
        /// <para>
        /// It is assumed, that the send method is called at the appropriate times when the first 
        /// requested and on server down events where the message is not faulted.
        /// </para>
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public static bool Send(ReliableRequest request)
        {
            if (request.IsFaulted())
            {
                if (request.Message != null)
                {
                    //---------------------------------------------------------------------------//
                    // The maximum number of retry attempts has been exceeded. Dispatch an error 
                    // response indicating the error.
                    //---------------------------------------------------------------------------//

                    ReliableRequestManager.Instance.Dispatched(request.CorrelationId);

                    string faultMessage = 
                        String.Format(
                            "Failed to process the request [CORRELATION ID : {0}] after {1} attempts",
                            request.CorrelationId, request.MaxAttempts);
                    
                    LogUtility.Error(faultMessage);
                    throw new FaultException(faultMessage);
                }

                return false;
            }

            ++request.Attempts;

            //------------------------------------------------------------------------------------//
            // Transition to the queued state after sending the message. 
            //------------------------------------------------------------------------------------//

            MessageBuffer messageBuffer = request.Message.CreateBufferedCopy(int.MaxValue);

            request.Channel.Send( messageBuffer.CreateMessage() );
            ReliableRequestManager.Instance.Queued(request);

            return true;
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
