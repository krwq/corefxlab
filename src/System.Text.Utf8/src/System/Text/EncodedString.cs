using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Text
{
    public partial struct EncodedString
    {
        public Encoding Encoding { get; private set; }
        // TODO: Should this be union?
        public TextBuffer Buffer { get; private set; }
        private string _builtinString;

        public EncodedString(Encoding encoding, TextBuffer buffer)
        {
            Encoding = encoding;
            Buffer = buffer;
            _builtinString = null;
        }
    }
}
