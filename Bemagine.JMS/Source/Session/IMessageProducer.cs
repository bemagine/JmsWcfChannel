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
    /// <summary>
    /// A session relative producer of messages used by JMS clients.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public interface IMessageProducer
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Closes the message producer.
        /// </summary>
        /// <remarks>
        /// Clients should call Close() to ensure that timely release of JMS provider resources
        /// allocated outside the runtime environment.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        void Close();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the producer's default delivery mode.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        DeliveryMode DeliveryMode { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the producer's default priority.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        MessagePriority Priority { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the default length of time in milliseconds from its dispatch time that a 
        /// produced message should be retained by the message system.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        long TimeToLive { get; set; }                
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the destination associated with this MessageProducer.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IDestination Destination { get; }                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets an indication of whether message IDs are disabled.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        bool DisableMessageID { get; set; }              

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets an indication of whether message timestamps are disabled.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        bool DisableMessageTimestamp { get; set; }
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sends a message using the MessageProducer's default delivery mode, priority, and time 
        /// to live.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void Send(IMessage message); 

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sends a message to a destination for an unidentified message producer.
        /// </summary>
        //----------------------------------------------------------------------------------------//
        
        void Send(IDestination destination, IMessage message);                

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sends a message to a destination for an unidentified message producer, specifying 
        /// delivery mode, priority and time to live.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void Send(IDestination destination, IMessage message, DeliveryMode deliveryMode, 
            MessagePriority priority, long timeToLive); 

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Sends a message to the destination, specifying delivery mode, priority, and time to 
        /// live.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void Send(IMessage message, DeliveryMode deliveryMode, MessagePriority priority, 
            long timeToLive);                
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
