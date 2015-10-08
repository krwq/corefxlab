using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace System.Text.Utf8
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Utf16LittleEndianEncodedCodePoint
    {
        // TODO: .ctor(char)
        // TODO: .ctor(Utf8EncodedCodePoint)

        public unsafe Utf16LittleEndianEncodedCodePoint(ByteSpan buffer) : this()
        {
            fixed (byte* encodedData = &_byte0)
            {
                if (buffer.Length == 1 || buffer.Length == 3)
                    throw new ArgumentException("buffer");
                if (!buffer.TryCopyTo(encodedData, 4))
                    throw new ArgumentException("buffer");
            }
        }

        [FieldOffset(0)]
        private byte _byte0;
        [FieldOffset(1)]
        private byte _byte1;
        [FieldOffset(2)]
        private byte _byte2;
        [FieldOffset(3)]
        private byte _byte3;

        [FieldOffset(0)]
        private char _char0;
        [FieldOffset(2)]
        private char _char1;

        public int Length { get { return char.IsSurrogate(_char0) ? 2 : 1; } }
        public int LengthInBytes { get { return char.IsSurrogate(_char0) ? 4 : 2; }}

        public byte Byte0 { get { return _byte0; } set { _byte0 = value; } }
        public byte Byte1 { get { return _byte1; } set { _byte1 = value; } }
        public byte Byte2 { get { return _byte2; } set { _byte2 = value; } }
        public byte Byte3 { get { return _byte3; } set { _byte3 = value; } }
    }
}
