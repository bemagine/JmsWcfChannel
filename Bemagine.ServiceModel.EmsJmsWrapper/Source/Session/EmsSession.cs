//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.EmsJmsWrapper.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.EmsJmsWrapper
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using Bemagine.ServiceModel.Jms;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Exposes the TIBCO EMS Session implementation as an ISession.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    internal sealed class EmsSession : ISession
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
        
        private readonly TIBCO.EMS.Session _innerSession;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        internal EmsSession(TIBCO.EMS.Session innerSession)
        {
            _innerSession = innerSession;
        }

        #region ISession interface implementation
        //----------------------------------------------------------------------------------------//
        // ISession interface implementation
        //----------------------------------------------------------------------------------------//

        public void Close()
        {
            SafeProxy.SafeCall( () => _innerSession.Close() );
        }

        public IDestination CreateQueue(string queueName)
        {
            return SafeProxy.SafeCall( 
                () => { return new EmsDestination(_innerSession.CreateQueue(queueName)); }
            );
        }

        public IDestination CreateTopic(string topicName)
        {
            return SafeProxy.SafeCall( 
                () => { return new EmsDestination(_innerSession.CreateTopic(topicName)); }
            );
        }

        public IMessageConsumer CreateConsumer(IDestination destination)
        {
            return SafeProxy.SafeCall( 
                () => 
                { 
                    return new EmsConsumer(
                        _innerSession.CreateConsumer((EmsDestination) destination)
                    ); 
                }
            );
        }

        public IMessageConsumer CreateConsumer(IDestination destination, string messageSelector)
        {
            return SafeProxy.SafeCall( 
                () => 
                { 
                    return new EmsConsumer(
                        _innerSession.CreateConsumer((EmsDestination) destination, messageSelector)
                    ); 
                }
            );
        }

        public IMessageProducer CreateProducer(IDestination destination)
        {
            return SafeProxy.SafeCall( 
                () => 
                { 
                    return new EmsProducer(
                        _innerSession.CreateProducer((EmsDestination) destination)
                    ); 
                }
            );
        }

        public IMessage CreateMessage()
        {
            return SafeProxy.SafeCall( 
                () => { return new EmsMessage(_innerSession.CreateMessage()); }
            );
        }

        public IBytesMessage CreateBytesMessage()
        {
            return SafeProxy.SafeCall( 
                () => { return new EmsBytesMessage(_innerSession.CreateBytesMessage()); }
            );
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
