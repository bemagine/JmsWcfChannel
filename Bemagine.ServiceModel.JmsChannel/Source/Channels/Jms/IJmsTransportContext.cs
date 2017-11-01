//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla License Version 1.1 
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
    /// <summary>
    /// An interface defining the contextual data required to configure, construct and support
    /// the functional requirements of the JMS transport infrastructure.
    /// </summary>
    /// <remarks>
    /// The interface merely exists to support simplification of binding logic.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    public interface IJmsTransportContext
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Copies the members of the specified instance to the current instance.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        void CopyFrom(IJmsTransportContext rhs);

        #region message delivery, buffering, & compression
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the maximum size of the buffer pool in bytes managed by an instance of the
        /// BufferManager.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        long MaxBufferPoolSize { get; set; } 
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the maximum message size in bytes the transport channel can received. This
        /// value is specified as the maximum buffer size during the creation of a BufferManager.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        long MaxReceivedMessageSize { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the message size threshold where messages should be compressed.
        /// </summary>
        /// <remarks>
        /// Compression can be resource intensive and there are diminishing returns as the message
        /// size threshold decreases. This feature exists to support compression of large messages
        /// and should be set accordingly.
        /// </remarks>
        //----------------------------------------------------------------------------------------//

        long MessageSizeCompressionThreshold { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the flag that indicates whether or not to deliver JMS messages as
        /// persistent or non-persistent. Non-persistent delivery is set by default to limit
        /// resource consumption of the JMS provider.
        /// </summary>
        //----------------------------------------------------------------------------------------//   

        bool PersistentDeliveryRequired { get; set; }        
        #endregion

        #region JMS provider connection details
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the type string that declares the connection factory type that implements 
        /// the Bemagine.ServiceModel.Jms.IConnectionFactory interface. 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        string JmsConnectionFactoryType { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the username used to establish a connection with the JMS provider.
        /// </summary>
        //----------------------------------------------------------------------------------------//        

        string UserName { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the password of the username used to establish a connection with the JMS 
        /// provider.
        /// </summary>
        //----------------------------------------------------------------------------------------//   

        string Password { get; set; }
        #endregion

        #region endpoint / destination addressing
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets an explicit response destination to listen for responses. Explicit 
        /// response destinations server in the times where responses may be processed by any 
        /// number of resources.  
        /// </summary>
        //----------------------------------------------------------------------------------------//

        string ExplicitResponseDestinationName { get; set; }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
