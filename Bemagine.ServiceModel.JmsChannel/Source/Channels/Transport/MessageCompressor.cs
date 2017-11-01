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
    using System.IO;
    using System.IO.Compression;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A static utility class that provide compression and decompression methods. The deflate
    /// compression algorithm is used by these methods because they perform better than the
    /// GZip compression algorithm. The difference in performance is attributatble to the
    /// CRC checksum that the GZip algorithm generates. If compressed archives are being 
    /// corrupted, the GZip algorithm may help.
    /// </summary>
    /// <remarks>
    /// Given that this is a low level utility class there are some assumptions that can be
    /// made concerning the nullness of the method parameters. If this is ported to a more
    /// general context these checks will be required.
    /// </remarks>
    //--------------------------------------------------------------------------------------------//

    internal static class MessageCompressor
    {
        //----------------------------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Compresses the specified byte array using the deflate compression algorithn.
        /// </summary>
        /// <param name="buffer">The byte array to be compressed.</param>
        /// <returns>An array representing the compressed input array.</returns>
        //----------------------------------------------------------------------------------------//

        public static byte[] CompressBuffer(ArraySegment<byte> buffer)
        {
            var memoryStream = new MemoryStream();

            using (var compressor = new DeflateStream(memoryStream, CompressionMode.Compress, true))
            {
                compressor.Write(buffer.Array, 0, buffer.Count);
            }
            
            byte[] compressedBytes = memoryStream.ToArray();
            memoryStream.Close();

            return compressedBytes;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Decompresses the an array of bytes compressed using the deflate compression algorithm.
        /// </summary>
        /// <param name="compressedBytes"></param>
        /// <param name="decompressedBytes">
        /// The decompressedBytes array is specified to reduce the number of allocations of
        /// large arrays performed.
        /// </param>
        /// <param name="decompressedByteCount"></param>
        //----------------------------------------------------------------------------------------//        

        public static void DecompressBuffer(
            byte[] compressedBytes, 
            byte[] decompressedBytes,
            int decompressedByteCount)
        {
            using (var stream = new MemoryStream(compressedBytes))
            {
                stream.Position = 0;

                using (var compressor = new DeflateStream(stream, CompressionMode.Decompress))
                {
                    int offset = 0;
                    int bytesRead = 0;
                    
                    while (true)
                    {
                        bytesRead += 
                            compressor.Read(decompressedBytes, offset, decompressedByteCount);

                        if (bytesRead == decompressedByteCount)
                            break;

                        offset += bytesRead;
                    }
                }
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
