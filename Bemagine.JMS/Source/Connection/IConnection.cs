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
    /// A Connection object is a client's active connection to its JMS provider. It typically 
    /// allocates provider resources outside the runtime environment.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public interface IConnection
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the client ID of the connection.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        string ClientID { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Starts / restarts the connection's delivery of incoming messages.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void Start();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Stops (temporarily) the connection's delivery of incoming messages. Message delivery
        /// is resumed by calling the connection's Start() method.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void Stop();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Closes the connection with the JMS provider.
        /// </summary>
        /// <remarks>
        /// Clients should call Close() to ensure that timely release of JMS provider resources
        /// allocated outside the runtime environment.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        void Close();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a Session with the JMS provider.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        ISession CreateSession(bool transacted, AcknowledgeMode acknowledgeMode);
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
