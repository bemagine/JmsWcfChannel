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
    /// Exposes the TIBCO EMS Connection implementation as an IConnection.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public sealed class EmsConnection : IConnection
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
        
        private readonly TIBCO.EMS.Connection _innerConnection;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        internal EmsConnection(TIBCO.EMS.Connection innerConnection)
        {
            _innerConnection = innerConnection;
        }

        //----------------------------------------------------------------------------------------//
        // implicit conversion operator
        //----------------------------------------------------------------------------------------//

        public static implicit operator TIBCO.EMS.Connection(EmsConnection emsConnection)
        {
            return emsConnection._innerConnection;
        }

         #region IConnection interface implementation
        //----------------------------------------------------------------------------------------//
        /// IConnection interface implementation
        //----------------------------------------------------------------------------------------//

        public string ClientID 
        { 
            get { return SafeProxy.SafeCall(() => { return _innerConnection.ClientID; }); }
            set { _innerConnection.ClientID = value; }
        }

        public void Start()
        {
            SafeProxy.SafeCall( () => _innerConnection.Start() );
        }

        public void Stop()
        {
            SafeProxy.SafeCall( () => _innerConnection.Stop() );
        }

        public void Close()
        {
            SafeProxy.SafeCall( () => _innerConnection.Close() );
        }

        public ISession CreateSession(bool transacted, AcknowledgeMode acknowledgeMode)
        {
            return SafeProxy.SafeCall( 
                () =>
                { 
                    return new EmsSession(
                        _innerConnection.CreateSession(
                            transacted,
                            acknowledgeMode.ToEms()
                        ));
                });
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
