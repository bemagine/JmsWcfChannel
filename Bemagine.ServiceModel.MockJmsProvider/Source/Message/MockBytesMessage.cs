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
    using System.Collections.Generic;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Mock implementation of a JMS bytes message that implements the IBytesMessage interface.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class MockBytesMessage : MockMessage, IBytesMessage
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly List<byte> _body = new List<byte>();

        #region IMessage interface implementation
        //----------------------------------------------------------------------------------------//
        // IBytesMessage interface implementation
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the number of bytes of the message body.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public long BodyLength { get { return _body.Count; } }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Reads a byte array from the bytes message stream.
        /// </summary>
        /// <param name="bytes"></param>
        //----------------------------------------------------------------------------------------//

        public int ReadBytes(byte[] bytes)
        {
            int bytesRead = 0;

            if ((bytes != null) && (bytes.Length > 0))
            {
                bytesRead = (bytes.Length > _body.Count) ? _body.Count : bytes.Length;
                _body.GetRange(0, bytesRead).CopyTo(bytes);
            }

            return bytesRead;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Writes a byte array from the bytes message stream. Successive calls appends the bytes
        /// to the end of the body of bytes already written.
        /// </summary>
        /// <param name="bytes"></param>
        //----------------------------------------------------------------------------------------//

        public void WriteBytes(byte[] bytes)
        {
            if (bytes != null)
            {
                _body.AddRange(bytes);
            }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Clears the bytes message stream body.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override void ClearBody()
        {
            _body.Clear();
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
