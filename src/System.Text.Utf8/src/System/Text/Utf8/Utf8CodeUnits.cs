using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace System.Text.Utf8
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Utf8CodeUnits
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

        public Utf8CodeUnits(CodePoint cp) : this()
        {
            if (!IsSupportedCodePoint(cp))
                throw new NotSupportedException("CodePoints longer than 21bits not supported.");

            _length = GetNumberOfCodeUnits(cp);

            unsafe
            {
                fixed (byte* buffer = &_byte0)
                {
                    EncodeCodePointUncheckedInternal(cp, _length, buffer);
                }
            }
        }

        [FieldOffset(0)]
        private int _length;

        [FieldOffset(4)]
        private byte _byte0;
        [FieldOffset(5)]
        private byte _byte1;
        [FieldOffset(6)]
        private byte _byte2;
        [FieldOffset(7)]
        private byte _byte3;

        public int Length { get { return _length; } set { _length = value; } }

        public byte Byte0 { get { return _byte0; } set { _byte0 = value; } }
        public byte Byte1 { get { return _byte1; } set { _byte1 = value; } }
        public byte Byte2 { get { return _byte2; } set { _byte2 = value; } }
        public byte Byte3 { get { return _byte3; } set { _byte3 = value; } }

        #region Decoder

        public static unsafe int GetNumberOfEncodedCodeUnitsFromFirstCodeUnit(byte first)
        {
            if ((first & mask_1000_0000) == 0)
                return 1;
            if ((first & mask_1110_0000) == mask_1100_0000)
                return 2;
            if ((first & mask_1111_0000) == mask_1110_0000)
                return 3;
            if ((first & mask_1111_1000) == mask_1111_0000)
                return 4;

            return -1;
        }
        //TODO: FINISH IT!

        #endregion


        #region Encoder
        public static bool IsSupportedCodePoint(CodePoint cp)
        {
            return (cp.Value >> 21) == 0;
        }

        public static int GetNumberOfCodeUnits(CodePoint cp)
        {
            if (cp.Value <= 0x7F)
                return 1;

            if (cp.Value <= 0x7FF)
                return 2;

            if (cp.Value <= 0xFFFF)
                return 3;

            if (cp.Value <= 0x1FFFFF)
                return 4;

            return -1;
        }

        public unsafe int EncodeCodePoint(byte[] buffer, long index)
        {
            if (index < 0 || index + Length > buffer.Length)
                throw new ArgumentOutOfRangeException("index");

            fixed (byte* pinnedBuffer = buffer)
            {
                switch (Length)
                {
                    case 1:
                        buffer[index] = Byte0;
                        return Length;
                    case 2:
                        buffer[index++] = Byte0;
                        buffer[index++] = Byte1;
                        return Length;
                    case 3:
                        buffer[index++] = Byte0;
                        buffer[index++] = Byte1;
                        buffer[index++] = Byte2;
                        return Length;
                    case 4:
                        buffer[index++] = Byte0;
                        buffer[index++] = Byte1;
                        buffer[index++] = Byte2;
                        buffer[index++] = Byte3;
                        return Length;
                }
            }

            throw new Exception("InternalError");
        }

        [CLSCompliant(false)]
        public static unsafe byte* EncodeCodePoint(CodePoint cp, byte* buffer, byte* end)
        {
            int numberOfCodeUnits = GetNumberOfCodeUnits(cp);
            if (buffer + numberOfCodeUnits > end)
                return null;

            return EncodeCodePointUncheckedInternal(cp, numberOfCodeUnits, buffer);
        }

        private static unsafe byte* EncodeCodePointUncheckedInternal(CodePoint cp, int numberOfCodeUnits, byte* buffer)
        {
            switch (numberOfCodeUnits)
            {
                case 1:
                    *buffer++ = (byte)(mask_0111_1111 & cp.Value);
                    return buffer;
                case 2:
                    *buffer++ = (byte)(((cp.Value >> 6) & mask_0001_1111) | mask_1100_0000);
                    *buffer++ = (byte)(((cp.Value >> 0) & mask_0011_1111) | mask_1000_0000);
                    return buffer;
                case 3:
                    *buffer++ = (byte)(((cp.Value >> 12) & mask_0000_1111) | mask_1110_0000);
                    *buffer++ = (byte)(((cp.Value >> 6) & mask_0011_1111) | mask_1000_0000);
                    *buffer++ = (byte)(((cp.Value >> 0) & mask_0011_1111) | mask_1000_0000);
                    return buffer;
                case 4:
                    *buffer++ = (byte)(((cp.Value >> 18) & mask_0000_0111) | mask_1111_0000);
                    *buffer++ = (byte)(((cp.Value >> 12) & mask_0011_1111) | mask_1000_0000);
                    *buffer++ = (byte)(((cp.Value >> 6) & mask_0011_1111) | mask_1000_0000);
                    *buffer++ = (byte)(((cp.Value >> 0) & mask_0011_1111) | mask_1000_0000);
                    return buffer;
                default:
                    throw new Exception("InternalError");
            }
        }
        #endregion
    }
}
