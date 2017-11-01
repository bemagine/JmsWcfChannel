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
    /// Exposes the TIBCO EMS BytesMessage implementation as an IBytesMessage.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public sealed class EmsBytesMessage : EmsMessage, IBytesMessage
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//
        
        private readonly TIBCO.EMS.BytesMessage _innerMessage;

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        internal EmsBytesMessage(TIBCO.EMS.BytesMessage innerMessage) : base(innerMessage)
        {
            _innerMessage = innerMessage;
        }

        #region IBytesMessage interface implementation
        //----------------------------------------------------------------------------------------//
        // IBytesMessage interface implementation
        //----------------------------------------------------------------------------------------//

        public long BodyLength 
        { 
            get { return _innerMessage.BodyLength; } 
        }

        public int ReadBytes(byte[] bytes)
        {
            return SafeProxy.SafeCall( () => { return _innerMessage.ReadBytes(bytes); } );
        }

        public void WriteBytes(byte[] bytes)
        {
            SafeProxy.SafeCall( () => _innerMessage.WriteBytes(bytes) );
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
