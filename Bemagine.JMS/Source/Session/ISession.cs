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
    /// A Session object is a single-threaded context for producing and consuming messages.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public interface ISession
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Closes the session with the JMS provider.
        /// </summary>
        /// <remarks>
        /// Clients should call Close() to ensure that timely release of JMS provider resources
        /// allocated outside the runtime environment.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        void Close();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a queue destination.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IDestination CreateQueue(string queueName);

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a topic destination
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IDestination CreateTopic(string topicName);

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a message consumer for the specified destination.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IMessageConsumer CreateConsumer(IDestination destination);

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a message consumer for the specified destination and message selector.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IMessageConsumer CreateConsumer(IDestination destination, string messageSelector);

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a message producer that sends messages to the specified destination when the
        /// non-destination specific Send variants are called.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IMessageProducer CreateProducer(IDestination destination);

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a Message.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IMessage CreateMessage();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a BytesMessage.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        IBytesMessage CreateBytesMessage();
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
