using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Text.Utf16
{
    public struct StringCodePointEnumerator : IEnumerator<UnicodeCodePoint>, IEnumerator
    {
        readonly string _s;
        int _index;

        public StringCodePointEnumerator(string s)
        {
            _s = s;
            _index = -1;
        }

        public UnicodeCodePoint Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (UnicodeCodePoint)(unchecked((uint)char.ConvertToUtf32(_s, _index)));
            }
        }

        public void Reset()
        {
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            ++_index;
            if (_index < _s.Length)
            {
                if (!char.IsSurrogate(_s[_index]))
                {
                    return true;
                }
                else
                {
                    ++_index;
                    if (_index < _s.Length)
                    {
                        return true;
                    }
                    // TODO: Throw exception here? (Invalid Utf16 character)
                }
            }
            return false;
        }

        object IEnumerator.Current { get { return Current; } }

        void IDisposable.Dispose() { }
    }
}
