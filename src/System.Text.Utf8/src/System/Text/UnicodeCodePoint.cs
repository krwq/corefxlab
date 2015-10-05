using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Text
{
    public struct UnicodeCodePoint
    {
        // TODO: make all methods CLSCompliant
        [CLSCompliant(false)]
        public UnicodeCodePoint(uint value) : this()
        {
            Value = value;
            if (!IsSupportedCodePoint(this))
            {
                throw new NotSupportedException("CodePoints longer than 21bits not supported.");
            }
        }

        [CLSCompliant(false)]
        public uint Value { get; private set; }

        public static bool IsSupportedCodePoint(UnicodeCodePoint codePoint)
        {
            // TODO: Check this value, written from head
            return codePoint.Value < 17 * (1 << 16);
        }

        public bool IsSurrogate()
        {
            return Value >= SpecConstants.SurrogateRangeStart && Value <= SpecConstants.SurrogateRangeEnd;
        }

        [CLSCompliant(false)]
        public static explicit operator uint(UnicodeCodePoint cp) { return cp.Value; }
        [CLSCompliant(false)]
        public static explicit operator UnicodeCodePoint(uint value) { return new UnicodeCodePoint(value); }
    }
}
