using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Text
{
    public struct TextBuffer : IEquatable<TextBuffer>
    {
        public long Index { get; private set; }
        public long Length { get; private set; }
        public byte[] Buffer { get; private set; }

        public TextBuffer(byte[] buffer, long index, long length)
        {
            Buffer = buffer;
            Index = index;
            Length = length;
        }

        public TextBuffer(byte[] buffer) : this(buffer, 0, buffer.Length) { }

        private static unsafe bool MemoryCompare(byte* s1, long len1, byte* s2, long len2)
        {
            if (s1 == s2)
                return true;

            if (len1 != len2)
                return false;

            if (s1 == null || s2 == null)
                return false;

            if (len1 == 0)
                return true;

            if (len1 == 1)
                return *s1 == *s2;

            byte* end1 = s1 + len1;
            byte* end2 = s2 + len2;
            for (; s1 != end1 && s2 != end2; s1++, s2++)
            {
                if (*s1 != *s2)
                {
                    return false;
                }
            }

            return true;
        }

        public unsafe bool Equals(TextBuffer other)
        {
            fixed (byte* pinnedBuffer = Buffer)
            {
                byte* s1 = pinnedBuffer + Index;
                fixed (byte* pinnedOtherBuffer = other.Buffer)
                {
                    byte* s2 = pinnedOtherBuffer + other.Index;
                    // TODO: replace with better implementation or optimize
                    return MemoryCompare(s1, Length, s2, other.Length);
                }
            }
        }

        // operators
        public static bool operator ==(TextBuffer left, TextBuffer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextBuffer left, TextBuffer right)
        {
            return !left.Equals(right);
        }

        public EncodedString ToEncodedString(Encoding encoding)
        {
            return new EncodedString(encoding, this);
        }

        public string ToString(Encoding encoding)
        {
            return encoding.GetString(Buffer, (int)Index, (int)Length);
        }

        public override string ToString()
        {
            // TODO: Auto-detection of encoding? UTF8? UTF16?
            throw new InvalidOperationException();
        }

        public override bool Equals(object obj)
        {
            if (obj is TextBuffer)
            {
                return Equals((TextBuffer)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (new Tuple<byte[], long, long>(Buffer, Index, Length)).GetHashCode();
        }
    }
}
