using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Utf16
{
    public static class Utf16LittleEndianEncoder
    {
        const uint MaskLow10Bits = 0x3FF;

        public static bool TryDecodeCodePoint(ByteSpan buffer, out UnicodeCodePoint codePoint, out int encodedBytes)
        {
            if (buffer.Length < 2)
            {
                codePoint = default(UnicodeCodePoint);
                encodedBytes = default(int);
                // buffer too small
                return false;
            }

            uint codePointValue = (uint)buffer[0] | ((uint)buffer[1] << 8);
            encodedBytes = 2;
            // Notice: This is any surrogate, not only high surrogate
            bool isSurrogate = codePointValue >= SpecConstants.HighSurrogateFirstCodePoint && codePointValue <= SpecConstants.LowSurrogateLastCodePoint;
            if (isSurrogate)
            {
                isSurrogate = codePointValue <= SpecConstants.HighSurrogateLastCodePoint;
                if (!isSurrogate || buffer.Length < 4)
                {
                    codePoint = default(UnicodeCodePoint);
                    encodedBytes = default(int);
                    // invalid high surrogate or buffer too small
                    return false;
                }
                unchecked
                {
                    codePointValue -= SpecConstants.HighSurrogateFirstCodePoint;
                    encodedBytes += 2;
                }
                // high surrogate contains 10 first bits of the code point
                codePointValue <<= 10;

                uint lowSurrogate = (uint)buffer[2] | ((uint)buffer[3] << 8);
                if (lowSurrogate < SpecConstants.LowSurrogateFirstCodePoint || lowSurrogate > SpecConstants.LowSurrogateLastCodePoint)
                {
                    codePoint = default(UnicodeCodePoint);
                    encodedBytes = default(int);
                    // invalid low surrogate character
                    return false;
                }

                unchecked
                {
                    lowSurrogate -= SpecConstants.LowSurrogateFirstCodePoint;
                }
                codePointValue |= lowSurrogate;
            }

            codePoint = (UnicodeCodePoint)codePointValue;

            return true;
        }

        public static bool TryEncodeCodePoint(UnicodeCodePoint codePoint, ByteSpan buffer, out int encodedBytes)
        {
            if ((uint)codePoint > 0x10FFFF)
            {
                encodedBytes = default(int);
                return false;
            }

            // is Surrogate?
            encodedBytes = ((uint)codePoint >= 0x10000) ? 4 : 2;

            if (buffer.Length < encodedBytes)
            {
                codePoint = default(UnicodeCodePoint);
                encodedBytes = default(int);
                // buffer too small
                return false;
            }

            if (encodedBytes == 2)
            {
                unchecked
                {
                    buffer[0] = (byte)((uint)codePoint);
                    buffer[1] = (byte)((uint)codePoint >> 8);
                }
            }
            else
            {
                unchecked
                {
                    uint highSurrogate = ((uint)codePoint >> 10) + 0xD800;
                    uint lowSurrogate = ((uint)codePoint & MaskLow10Bits) + 0xDC00;
                    buffer[0] = (byte)highSurrogate;
                    buffer[1] = (byte)(highSurrogate >> 8);

                    buffer[2] = (byte)lowSurrogate;
                    buffer[3] = (byte)(lowSurrogate >> 8);
                }
            }
            return true;
        }
    }
}
