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
using System.Globalization;
using System.IO;
using System.Text;

namespace sharpBencode
{
    public class BString : 
        IEquatable<string>, 
        IEquatable<BString>, 
        IComparable<string>, 
        IComparable<BString>, 
        IBencodingType
    {
        public static Encoding ExtendedASCIIEncoding { get { return Encoding.GetEncoding("Windows-1252"); } }

        public BString()
        {
            Value = string.Empty;
        }

        public BString(string value)
        {
            Value = value;
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (value == null)
                {
                    _value = string.Empty;
                }
                else
                {
                    _value = value;
                }
            }
        }

        public byte[] ByteValue { get; set; }
        
        public IBencodingType Decode(BinaryReader inputStream, ref int bytesConsumed)
        {
            string numberLength = "";
            char ch;

            while ((ch = inputStream.ReadChar()) != ':')
            {
                numberLength += ch;
                bytesConsumed++;
            }

            bytesConsumed++;

            byte[] stringData = inputStream.ReadBytes(int.Parse(numberLength));

            bytesConsumed += int.Parse(numberLength);

            Value = ExtendedASCIIEncoding.GetString(stringData, 0, stringData.Length);
            ByteValue = stringData;

            return this;
        }

        public void Encode(BinaryWriter writer)
        {
            byte[] ascii = ByteValue ?? ExtendedASCIIEncoding.GetBytes(Value);

            // Write length
            writer.Write(ExtendedASCIIEncoding.GetBytes(ascii.Length.ToString(CultureInfo.InvariantCulture)));

            // Write seperator
            writer.Write(':');

            // Write ASCII representation
            writer.Write(ascii);
        }

        public int CompareTo(string other)
        {
            return string.Compare(Value, other);
        }

        public int CompareTo(BString other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return CompareTo(other.Value);
        }

        public override bool Equals(object obj)
        {
            BString other = obj as BString;

            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(BString other)
        {
            if (other == null)
            {
                return false;
            }

            if (Equals(other, this))
            {
                return true;
            }

            return Equals(other.Value, Value);
        }

        public bool Equals(string other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(Value, other);
        }

        public override int GetHashCode()
        {
            // Value should never be null
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator BString(string x)
        {
            return new BString(x);
        }

        public static implicit operator string (BString x)
        {
            return x.Value;
        }

    }
}
