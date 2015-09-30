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

using System;
using System.Collections.Generic;
using System.IO;

namespace sharpBencode
{
    public class BDict : 
        Dictionary<string, IBencodingType>, 
        IEquatable<BDict>, 
        IEquatable<Dictionary<string, IBencodingType>>, 
        IBencodingType
    {
        public IBencodingType Decode(BinaryReader inputStream, ref int bytesConsumed)
        {
            // Get past 'd'
            inputStream.ReadByte();

            bytesConsumed++;

            // Read elements till an 'e'
            while (inputStream.PeekChar() != 'e')
            {
                // Key
                var key = (new BString()).Decode(inputStream, ref bytesConsumed);

                // Value
                var value = Utils.Decode(inputStream, ref bytesConsumed);

                this[((BString)key).Value] = value;
            }

            // Get past 'e'
            inputStream.Read();
            bytesConsumed++;

            return this;
        }

        public void Encode(BinaryWriter writer)
        {
            // Write header
            writer.Write('d');

            // Write elements
            foreach (var item in this)
            {
                // Write key
                var key = new BString
                {
                    Value = item.Key
                };

                key.Encode(writer);

                // Write value
                item.Value.Encode(writer);
            }

            // Write footer
            writer.Write('e');
        }

        public bool Equals(BDict obj)
        {
            var other = obj;

            return Equals(other);
        }

        public bool Equals(Dictionary<string, IBencodingType> other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Count != Count)
            {
                return false;
            }

            foreach (string key in Keys)
            {
                if (!other.ContainsKey(key))
                {
                    return false;
                }

                // Dictionaries cannot have nulls
                if (!other[key].Equals(this[key]))
                {
                    // Not ok
                    return false;
                }
            }

            return true;
        }
        public override bool Equals(object obj)
        {
            BDict other = obj as BDict;

            return Equals(other);
        }

        public new void Add(string key, IBencodingType value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            base.Add(key, value);
        }

        public new IBencodingType this[string index]
        {
            get { return base[index]; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                base[index] = value;
            }
        }

        public override int GetHashCode()
        {
            int r = 1;
            foreach (var pair in this)
            {
                r ^= pair.GetHashCode();
            }

            return r;
        }
    }
}
