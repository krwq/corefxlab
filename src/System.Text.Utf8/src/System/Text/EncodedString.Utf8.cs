using System;
using System.Text.Utf8;

namespace System.Text
{
    public partial struct EncodedString
    {
        public EncodedString(Utf8String utf8string) : this()
        {
            Utf8String = utf8string;
        }

        public static EncodedString Create(Utf8String utf8string) { return new EncodedString(utf8string); }

        public static implicit operator EncodedString(Utf8String utf8string) { return new EncodedString(utf8string); }
        public static explicit operator Utf8String(EncodedString text) { return text.ToUtf8String(); }

        public Utf8String Utf8String
        {
            get
            {
                if (Encoding.CodePage == Encoding.UTF8.CodePage)
                    return new Utf8String(Buffer);

                throw new InvalidOperationException();
            }
            private set
            {
                Encoding = Encoding.UTF8;
                Buffer = value.Buffer;
            }
        }

        public Utf8String ToUtf8String()
        {
            if (Encoding.CodePage == Encoding.Unicode.CodePage)
                return new Utf8String(Encoding.UTF8.GetBytes(BuiltinString));

            if (Encoding.CodePage == Encoding.UTF8.CodePage)
                return new Utf8String(Buffer);

            throw new NotSupportedException();
        }
    }
}
