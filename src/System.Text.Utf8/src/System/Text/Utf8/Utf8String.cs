using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Utf16;
using System.Threading.Tasks;

namespace System.Text.Utf8
{
    public struct Utf8String : IEnumerable<UnicodeCodePoint>//, IEquatable<Utf8String>, IComparable<Utf8String> // TODO:
    {
        private ByteSpan _buffer;

        private byte[] _bytes;
        private int _index;
        private int _length;

        public Utf8String(ByteSpan buffer)
        {
            _buffer = buffer;
            _bytes = null;
            _index = 0;
            _length = 0;
        }

        public Utf8String(byte[] utf8bytes) : this(utf8bytes, 0, utf8bytes.Length)
        {
        }

        public Utf8String(byte[] utf8bytes, int index, int length)
        {
            if (utf8bytes == null)
            {
                throw new ArgumentNullException("utf8bytes");
            }
            if (index + length > utf8bytes.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            _buffer = default(ByteSpan);
            _bytes = utf8bytes;
            _index = index;
            _length = length;
        }

        #region General Implementation
        private static IEnumerator<UnicodeCodePoint> GetCodePointsEnumerator(ByteSpan buffer)
        {
            while (buffer.Length > 0)
            {
                UnicodeCodePoint codePoint;
                int encodedBytes;
                if (Utf8Encoder.TryDecodeCodePoint(buffer, out codePoint, out encodedBytes))
                {
                    yield return codePoint;
                    buffer = buffer.Slice(encodedBytes);
                }
                else
                {
                    // TODO: change exception type
                    throw new Exception("Invalid character");
                }
            }
        }
        #endregion

        public IEnumerator<UnicodeCodePoint> GetEnumerator()
        {
            if (_bytes != null)
            {
                unsafe
                {
                    fixed (byte* pinnedBytes = _bytes)
                    {
                        return GetCodePointsEnumerator(new ByteSpan(pinnedBytes + _index, _length));
                    }
                }
            }
            else
            {
                return GetCodePointsEnumerator(_buffer);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //public static explicit operator Utf8String(string builtinString)
        //{
        //    return new Utf8String(builtinString);
        //}

        public override unsafe string ToString()
        {
            // get length first
            // TODO: Optimize for characters of length 1 or 2 in UTF-8 representation (no need to read anything)
            // TODO: is compiler gonna do the right thing here?
            // TODO: Should we use Linq's Count()?
            int len = 0;
            foreach (var codePoint in this)
            {
                len++;
                if (codePoint.IsSurrogate())
                    len++;
            }

            char[] characters = new char[len];
            fixed (char* pinnedCharacters = characters)
            {
                ByteSpan buffer = new ByteSpan((byte*)pinnedCharacters, len * 2);
                foreach (var codePoint in this)
                {
                    int bytesEncoded;
                    if (!Utf16LittleEndianEncoder.TryEncodeCodePoint(codePoint, buffer, out bytesEncoded))
                    {
                        // TODO: Change Exception type
                        throw new Exception("invalid character");
                    }
                    buffer = buffer.Slice(bytesEncoded);
                }
            }
            return new string(characters);
        }
    }
}
