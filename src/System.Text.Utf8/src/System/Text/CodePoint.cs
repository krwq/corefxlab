using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Text
{
    public struct CodePoint
    {
        [CLSCompliant(false)]
        public CodePoint(uint value)
        {
            Value = value;
        }

        [CLSCompliant(false)]
        public uint Value { get; private set; }

        [CLSCompliant(false)]
        public static explicit operator uint(CodePoint cp) { return cp.Value; }
        [CLSCompliant(false)]
        public static explicit operator CodePoint(uint value) { return new CodePoint(value); }

        // TODO: Add helper methods: Something like: GetChar, GetHighSurrogate, GetLowSurrogate, IsSurrogate, GetUtf8CodeUnits
    }
}
