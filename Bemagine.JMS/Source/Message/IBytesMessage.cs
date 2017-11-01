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
    /// A BytesMessage object is used to send a message containing a stream of uninterpreted bytes.
    /// </summary>
    //--------------------------------------------------------------------------------------------//
    
    public interface IBytesMessage : IMessage
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the number of bytes of the message body.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        long BodyLength { get; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Reads a byte array from the bytes message stream.
        /// </summary>
        /// <param name="bytes"></param>
        //----------------------------------------------------------------------------------------//

        int ReadBytes(byte[] bytes);

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Writes a byte array from the bytes message stream.
        /// </summary>
        /// <param name="bytes"></param>
        //----------------------------------------------------------------------------------------//

        void WriteBytes(byte[] bytes);
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
