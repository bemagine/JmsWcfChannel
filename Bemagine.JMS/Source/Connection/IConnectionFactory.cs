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
    /// An administered object, the IConnectionFactory object encapsulates a set of connection
    /// configuration parameters that has been defined by an administrator. A client uses it to 
    /// create a connection with a JMS provider. 
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public interface IConnectionFactory
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a connection to the JMS provider with the default user credentials. 
        /// </summary>
        /// <returns>
        /// A newly created connection in stopped mode.
        /// </returns>
        /// <remarks>
        /// No messages will be delivered until the Connection.Start() method is explicitly called. 
        /// </remarks>
        //----------------------------------------------------------------------------------------//  

        IConnection CreateConnection();

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a connection to the JMS provider with the specified user credentials.
        /// </summary>
        /// <param name="username">
        /// User name to use to connect to the JMS provider.
        /// </param>
        /// <param name="password">
        /// Password of the user name touse to connect to the JMS provider.
        /// </param>
        /// <returns>
        /// A newly created connection in stopped mode. 
        /// </returns>
        /// <remarks>
        /// No messages will be delivered until the Connection.Start() method is explicitly called. 
        /// </remarks>
        //----------------------------------------------------------------------------------------//  

        IConnection CreateConnection(string username, string password);
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
