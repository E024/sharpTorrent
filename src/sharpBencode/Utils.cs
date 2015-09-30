/*
    Copyright (c) 2013, Darren Horrocks. All rights reserved.

    Redistribution and use in source and binary forms, with or without modification,
    are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this
      list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice, this
      list of conditions and the following disclaimer in the documentation and/or
      other materials provided with the distribution.

    * Neither the name of Darren Horrocks nor the names of its
      contributors may be used to endorse or promote products derived from
      this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
    WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
    ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
    (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
    LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
    ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/

using System.IO;

namespace sharpBencode
{
    public class Utils
    {
        public static IBencodingType DecodeFile(string fileName)
        {
            using (var fileStream = File.OpenRead(fileName))
            {
                return Decode(fileStream);
            }
        }

        /// <summary>
        /// Parse a bencoded object from a string. 
        /// Warning: Beware of encodings.
        /// </summary>
        /// <seealso cref="ExtendedASCIIEncoding"/>
        /// <param name="inputString">The bencoded string to parse.</param>
        /// <returns>A bencoded object.</returns>
        public static IBencodingType Decode(string inputString)
        {
            byte[] byteArray = BString.ExtendedASCIIEncoding.GetBytes(inputString);

            return Decode(new MemoryStream(byteArray));
        }

        public static IBencodingType Decode(byte[] byteArray)
        {
            return Decode(new MemoryStream(byteArray));
        }

        public static IBencodingType Decode(byte[] byteArray, ref int bytesConsumed)
        {
            return Decode(new MemoryStream(byteArray), ref bytesConsumed);
        }

        /// <summary>
        /// Parse a bencoded stream (for example a file).
        /// </summary>
        /// <param name="inputStream">The bencoded stream to parse.</param>
        /// <returns>A bencoded object.</returns>
        public static IBencodingType Decode(Stream inputStream)
        {
            using (BinaryReader sr = new BinaryReader(inputStream, BString.ExtendedASCIIEncoding))
            {
                int bytesConsumed = 0;
                return Decode(sr, ref bytesConsumed);
            }
        }

        public static IBencodingType Decode(Stream inputStream, ref int bytesConsumed)
        {
            using (BinaryReader sr = new BinaryReader(inputStream, BString.ExtendedASCIIEncoding))
            {
                return Decode(sr, ref bytesConsumed);
            }
        }

        public static IBencodingType Decode(BinaryReader inputStream, ref int bytesConsumed)
        {
            IBencodingType returnValue = null;

            char next = (char)inputStream.PeekChar();

            switch (next)
            {
                case 'i':
                    // Integer
                    returnValue = new BInt(0);
                    break;

                case 'l':
                    // List
                    returnValue = new BList();
                    break;

                case 'd':
                    // Dictionary
                    returnValue = new BDict();
                    break;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    // String
                    returnValue = new BString();
                    break;
            }

            return returnValue.Decode(inputStream, ref bytesConsumed);
        }
    }
}
