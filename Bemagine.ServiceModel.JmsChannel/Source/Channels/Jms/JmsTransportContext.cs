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

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Encapsulates the contextual data required to configure, construct and support the
    /// functional requirements of the JMS transport infrastructure.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class JmsTransportContext : IJmsTransportContext
    {
        #region data members
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly object _thisLock = new object();
        #endregion

        #region construction & initialization
        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public JmsTransportContext(string scheme)
        {
            Scheme = scheme;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Copies the property values from the specified JmsTransportContext instance to the
        /// current instance for which this method is invoked.
        /// </summary>
        /// <param name="rhs">The source JmsTransportContext to copy property values from.</param>
        //----------------------------------------------------------------------------------------//     

        public void CopyFrom(IJmsTransportContext rhs)
        {
            Debug.Assert(
                (rhs != null), 
                "JmsTransportContext::CopyFrom expected a non-null JmsTransportContext instance.");
            
            CopyFrom(this, rhs);           
        }

        public static void CopyFrom(IJmsTransportContext lhs, IJmsTransportContext rhs)
        {            
            lhs.JmsConnectionFactoryType = rhs.JmsConnectionFactoryType;
            lhs.UserName = rhs.UserName;
            lhs.Password = rhs.Password;          

            lhs.MaxBufferPoolSize = rhs.MaxBufferPoolSize;
            lhs.MaxReceivedMessageSize = rhs.MaxReceivedMessageSize;
            lhs.MessageSizeCompressionThreshold = rhs.MessageSizeCompressionThreshold;
            lhs.PersistentDeliveryRequired = rhs.PersistentDeliveryRequired;

            lhs.ExplicitResponseDestinationName = rhs.ExplicitResponseDestinationName;            
        }
        #endregion

        #region message delivery, buffering, & compression
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the maximum size of the buffer pool in bytes managed by an instance of the
        /// BufferManager.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public long MaxBufferPoolSize { get; set; } 
        
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the maximum message size in bytes the transport channel can received. This
        /// value is specified as the maximum buffer size during the creation of a BufferManager.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public long MaxReceivedMessageSize { get; set; }

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

        public long MessageSizeCompressionThreshold 
        { 
            get 
            { 
                return (_messageSizeCompressionThreshold != null) ?
                    _messageSizeCompressionThreshold.Value : MaxReceivedMessageSize;
            }
            set
            {
                _messageSizeCompressionThreshold = value;
            }
        }
        private long? _messageSizeCompressionThreshold;

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the flag that indicates whether or not to deliver JMS messages as
        /// persistent or non-persistent. Non-persistent delivery is set by default to limit
        /// resource consumption of the JMS provider.
        /// </summary>
        //----------------------------------------------------------------------------------------//   

        public bool PersistentDeliveryRequired { get; set; }        
        #endregion

        #region JMS provider connection details
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the type string that declares the connection factory type that implements 
        /// the Bemagine.ServiceModel.Jms.IConnectionFactory interface. 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public string JmsConnectionFactoryType { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the username used to establish a connection with the JMS provider.
        /// </summary>
        //----------------------------------------------------------------------------------------//        

        public string UserName { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the password of the username used to establish a connection with the JMS 
        /// provider.
        /// </summary>
        //----------------------------------------------------------------------------------------//   

        public string Password { get; set; }
        #endregion

        #region endpoint / destination addressing
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// The URI scheme that inidicates the JMS destination type the transport should use.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public string Scheme { get; private set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets an explicit response destination to listen for responses. Explicit 
        /// response destinations server in the times where responses may be processed by any 
        /// number of resources.  
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public string ExplicitResponseDestinationName 
        { 
            get { return _responseDestinationName; }
            set { _responseDestinationName = value; }
        }
        private string _responseDestinationName;

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the name of the destination to listen for responses. If an explicit name is not
        /// specified a unique response destination is generated.
        /// </summary>
        /// <remarks>
        /// Unique response desination names are generated by default unless an explicit response
        /// destination was specified. Response destinations are specific to factory created 
        /// channels (ie client context).     
        /// </remarks>
        //----------------------------------------------------------------------------------------//
        
        public string ResponseDestinationName 
        { 
            get
            {
                if (_responseDestinationName.IsNullOrEmpty())
                {
                    lock (_thisLock)
                    {
                        if (_responseDestinationName.IsNullOrEmpty())
                        {
                            _responseDestinationName = String.Format(
                                "{0}.{1}", Environment.MachineName, Guid.NewGuid().ToString());
                        }
                    }
                }

                return _responseDestinationName;
            }
        }
        
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
