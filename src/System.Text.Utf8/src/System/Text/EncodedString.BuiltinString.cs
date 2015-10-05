using System;

namespace System.Text
{
    public partial struct EncodedString
    {
        public EncodedString(string builtinString) : this()
        {
            BuiltinString = builtinString;
        }

        public static EncodedString Create(string builtinString) { return new EncodedString(builtinString); }

        public static implicit operator EncodedString(string builtinString) { return new EncodedString(builtinString); }
        public static explicit operator string(EncodedString text) { return text.ToString(); }

        public string BuiltinString
        {
            get
            {
                if (Encoding.CodePage == Encoding.Unicode.CodePage)
                    return _builtinString;

                throw new InvalidOperationException();
            }
            private set
            {
                if (value == null)
                    // Encoding = null;
                    return;

                Encoding = Encoding.Unicode;
                _builtinString = value;
            }
        }

        public override string ToString()
        {
            if (Encoding.CodePage == Encoding.Unicode.CodePage)
                return BuiltinString;

            if (Encoding.CodePage == Encoding.UTF8.CodePage)
                return Utf8String.ToString();

            throw new NotSupportedException();
        }
    }
}
