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
    /// Mock implementation of a JMS producer that implements the IMessageProducer interface.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class MockProducer : IMessageProducer
    {
        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public MockProducer(IDestination destination)
        {
            Destination = destination;
        }

        #region IMessageProducer interface implementation
        //----------------------------------------------------------------------------------------//
        // IMessageProducer interface implementation
        //----------------------------------------------------------------------------------------// 
      
        public IDestination Destination { get; set; }

        public MessagePriority Priority { get; set; }
        public DeliveryMode DeliveryMode { get; set; }
        public long TimeToLive { get; set; }
        public bool DisableMessageID { get; set; }
        public bool DisableMessageTimestamp { get; set; }

        public void Close()
        {
        }

        public void Send(IMessage message)
        {
            MessageDispatcher.Instance.DispatchMessage(Destination, message);
        }

        public void Send(IDestination destination, IMessage message)
        {
            MessageDispatcher.Instance.DispatchMessage(destination, message);
        }

        public void Send(IDestination destination, IMessage message, DeliveryMode deliveryMode, 
            MessagePriority priority, long timeToLive)
        {
            MessageDispatcher.Instance.DispatchMessage(destination, message);
        }

        public void Send(IMessage message, DeliveryMode deliveryMode, MessagePriority priority, 
            long timeToLive)
        {
            MessageDispatcher.Instance.DispatchMessage(Destination, message);
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
