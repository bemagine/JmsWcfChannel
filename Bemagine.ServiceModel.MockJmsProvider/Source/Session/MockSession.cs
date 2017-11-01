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

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Mock implementation of a JMS Session that implements the ISession interface.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class MockSession : ISession
    {  
        #region ISession interface implementation
        //----------------------------------------------------------------------------------------//
        // ISession interface implementation
        //----------------------------------------------------------------------------------------//

        public void Close()
        {
        }

        public IDestination CreateQueue(string queueName)
        {
            return new MockDestination(queueName);
        }

        public IDestination CreateTopic(string topicName)
        {
            return new MockDestination(topicName);
        }

        public IMessageConsumer CreateConsumer(IDestination destination)
        {
            return new MockConsumer(destination);
        }

        public IMessageConsumer CreateConsumer(IDestination destination, string messageSelector)
        {
            return new MockConsumer(destination, messageSelector);
        }

        public IMessageProducer CreateProducer(IDestination destination)
        {
            return new MockProducer(destination);
        }

        public IMessage CreateMessage()
        {
            return new MockMessage();
        }

        public IBytesMessage CreateBytesMessage()
        {
            return new MockBytesMessage();
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
