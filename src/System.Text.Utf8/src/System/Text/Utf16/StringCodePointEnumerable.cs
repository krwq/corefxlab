using System.Collections;
using System.Collections.Generic;

namespace System.Text.Utf16
{
    // TODO: Should this and Utf8 code point enumerators/enumerable be subclasses of Utf8/16Encoder?
    public struct StringCodePointEnumerable : IEnumerable<UnicodeCodePoint>, IEnumerable
    {
        private string _s;

        public StringCodePointEnumerable(string s)
        {
            _s = s;
        }

        public StringCodePointEnumerator GetEnumerator()
        {
            return new StringCodePointEnumerator(_s);
        }

        IEnumerator<UnicodeCodePoint> IEnumerable<UnicodeCodePoint>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
