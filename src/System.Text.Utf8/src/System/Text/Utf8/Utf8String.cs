using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Utf8
{
    public struct Utf8String // TODO: : IEnumerable<CodePoint>
    {
        const uint mask_0111_1111 = 0x7F;
        const uint mask_0011_1111 = 0x3F;
        const uint mask_0001_1111 = 0x1F;
        const uint mask_0000_1111 = 0x0F;
        const uint mask_0000_0111 = 0x07;
        const uint mask_1000_0000 = 0x80;
        const uint mask_1100_0000 = 0xC0;
        const uint mask_1110_0000 = 0xE0;
        const uint mask_1111_0000 = 0xF0;
        const uint mask_1111_1000 = 0xF8;

        public TextBuffer Buffer { get; set; }

        public Utf8String(TextBuffer text)
        {
            Buffer = text;
        }

        public Utf8String(byte[] buffer)
        {
            Buffer = new TextBuffer(buffer);
        }

        public static explicit operator Utf8String(string builtinString)
        {
            return new Utf8String(System.Text.Encoding.UTF8.GetBytes(builtinString));
        }

        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(Buffer.Buffer, (int)Buffer.Index, (int)Buffer.Length);
        }

        [CLSCompliant(false)]
        public static unsafe byte* ReadCodePointByte(byte* index, ref uint codePoint)
        {
            codePoint <<= 6;
            uint current = *index++;
            if ((current & mask_1100_0000) != mask_1000_0000)
                throw new ArgumentOutOfRangeException("index");

            codePoint |= mask_0011_1111 & current;
            return index;
        }

        [CLSCompliant(false)]
        public static unsafe byte* ReadCodePointFromBuffer(byte* index, out uint codePoint)
        {
            uint first = *index++;

            if ((first & mask_1111_1000) == mask_1111_0000)
            {
                codePoint = first & mask_0000_0111;
                index = ReadCodePointByte(index, ref codePoint);
                index = ReadCodePointByte(index, ref codePoint);
                index = ReadCodePointByte(index, ref codePoint);

                return index;
            }

            if ((first & mask_1111_0000) == mask_1110_0000)
            {
                codePoint = first & mask_0000_1111;
                index = ReadCodePointByte(index, ref codePoint);
                index = ReadCodePointByte(index, ref codePoint);

                return index;
            }

            if ((first & mask_1110_0000) == mask_1100_0000)
            {
                codePoint = first & mask_0001_1111;
                index = ReadCodePointByte(index, ref codePoint);

                return index;
            }

            if ((first & mask_1000_0000) == 0)
            {
                codePoint = first & mask_0111_1111;

                return index;
            }

            throw new Exception("InternalError");
        }

        private static unsafe byte* ReadNextCharacterChecked(byte* start, byte* end, out char characterOrHighSurrogate, out char lowSurrogate)
        {
            byte* index = start;
            if (index < end)
            {
                if ((end - index) < 4)
                {
                    int size = Utf8CodeUnits.GetNumberOfEncodedCodeUnitsFromFirstCodeUnit(*index);
                    if (size == -1)
                        throw new Exception("Illegal UTF8 byte.");
                    if (index + size >= end)
                        throw new Exception("InternalError");
                }

                uint codePoint;
                index = ReadCodePointFromBuffer(index, out codePoint);

                if (codePoint < 0x010000)
                {
                    characterOrHighSurrogate = (char)codePoint;
                    lowSurrogate = (char)0;
                    return index;
                }
                else 
                {
                    // code point needs to be represented as surrogate
                    uint val = (codePoint - 0x010000) & 0x0FFFFF;

                    characterOrHighSurrogate = (char)(0xD800 + (val >> 10));
                    lowSurrogate = (char)(0xDC00 + (val & 0x3FF));
                }
            }

            characterOrHighSurrogate = (char)0;
            lowSurrogate = (char)0;
            return null;
        }

        public unsafe long CalculateNumberOfCodePointsUnchecked()
        {
            long ret = 0;
            fixed (byte* pinnedBuffer = Buffer.Buffer)
            {
                byte* index = pinnedBuffer + Buffer.Index;
                byte* end = index + Buffer.Length;

                while (index < end)
                {
                    ret++;
                    index += Utf8CodeUnits.GetNumberOfEncodedCodeUnitsFromFirstCodeUnit(*index);
                }
            }

            return ret;
        }

        public unsafe long CalculateNumberOfCharacters()
        {
            long ret = 0;
            fixed (byte* pinnedBuffer = Buffer.Buffer)
            {
                byte* index = pinnedBuffer + Buffer.Index;
                byte* end = index + Buffer.Length;

                while (index < end)
                {
                    ret++;
                    throw new NotImplementedException();
                //    CodePoint cp;
                //    index = Utf8CodeUnits.DecodeCodePoint(index, end, out cp);

                //    // TODO: Utf16Encoder.DoesCodePointNeedSurrogateCharactersForItsRepresentation?
                //    if (cp.Value >= 10000)
                //        ret++;
                }
            }

            return ret;
        }

        public unsafe char[] ConvertToUtf16Characters()
        {
            long len = CalculateNumberOfCharacters();
            char[] ret = new char[len];

            fixed (byte* pinnedBuffer = Buffer.Buffer)
            {
                byte* index = pinnedBuffer + Buffer.Index;
                byte* end = index + Buffer.Length;

                long idx = 0;
                while (true)
                {
                    char high, low;
                    index = ReadNextCharacterChecked(index, end, out high, out low);

                    if (index == null)
                        return ret;

                    if (low != 0)
                    {
                        ret[idx++] = high;
                        ret[idx++] = low;
                    }
                    else
                    {
                        ret[idx++] = high;
                    }
                }
            }
        }
    }
}
