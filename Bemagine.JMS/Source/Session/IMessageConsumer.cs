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
    /// A session relative consumer of messages used by JMS clients.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public interface IMessageConsumer
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Closes the message consumer.
        /// </summary>
        /// <remarks>
        /// Clients should call Close() to ensure that timely release of JMS provider resources
        /// allocated outside the runtime environment.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        void Close();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Receives the next message produced for this message consumer.
        /// </summary>
        //----------------------------------------------------------------------------------------//
        
        IMessage Receive();
          
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Receives the next message that arrives within the specified timeout interval.
        /// </summary>
        //----------------------------------------------------------------------------------------//
 
        IMessage Receive(long timeout);          

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Receives the next message if one is immediately available.
        /// </summary>
        //----------------------------------------------------------------------------------------//
 
        IMessage ReceiveNoWait(); 
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the consumer's message selector.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        string MessageSelector { get; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IMessageListener MessageListener { get; set; }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
