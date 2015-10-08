﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace System.Text.Utf8
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Utf8EncodedCodePoint
    {
        // TODO: .ctor(Utf16LittleEndianEncodedCodePoint)

        //public unsafe Utf8EncodedCodePoint(ByteSpan buffer) : this()
        //{
        //    fixed (byte* encodedData = &_byte0)
        //    {
        //        if (!buffer.TryCopyTo(encodedData, 4))
        //            throw new ArgumentException("buffer");
        //        _length = buffer.Length;
        //    }
        //}

        public unsafe Utf8EncodedCodePoint(char character) : this()
        {
            if (char.IsSurrogate(character))
                throw new ArgumentOutOfRangeException("character", "Surrogate characters are not allowed");

            UnicodeCodePoint codePoint = (UnicodeCodePoint)(uint)character;

            fixed (byte* encodedData = &_byte0)
            {
                ByteSpan buffer = new ByteSpan(encodedData, 4);
                if (!Utf8Encoder.TryEncodeCodePoint(codePoint, buffer, out _length))
                {
                    // TODO: Change exception type
                    throw new Exception("Internal error: this should never happen as codePoint is within acceptable range and is not surrogate");
                }
            }
        }

        public unsafe Utf8EncodedCodePoint(char highSurrogate, char lowSurrogate) : this()
        {
            UnicodeCodePoint codePoint = (UnicodeCodePoint)(uint)char.ConvertToUtf32(highSurrogate, lowSurrogate);

            fixed (byte* encodedData = &_byte0)
            {
                ByteSpan buffer = new ByteSpan(encodedData, 4);
                if (!Utf8Encoder.TryEncodeCodePoint(codePoint, buffer, out _length))
                {
                    // TODO: Change exception type
                    throw new Exception("Internal error: this should never happen as codePoint should be within acceptable range");
                }
            }
        }

        public static unsafe explicit operator Utf8EncodedCodePoint(char character) { return new Utf8EncodedCodePoint(character); }


        // TODO: Make it a property, read the length from the first byte
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
    }
}
