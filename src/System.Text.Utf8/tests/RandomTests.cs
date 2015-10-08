using Xunit;
using System.Linq;

namespace System.Text.Utf8.Tests
{
    public class Utf8StringTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("1258")]
        [InlineData("\uABCD")]
        [InlineData("\uABEE")]
        [InlineData("a\uABEE")]
        [InlineData("a\uABEEa")]
        [InlineData("a\uABEE\uABCDa")]
        public unsafe void Length(string s)
        {
            Assert.Equal(s.Length, (new Utf8String(Encoding.UTF8.GetBytes(s))).Count());
        }

        [Theory]
        [InlineData("")]
        [InlineData("1258")]
        [InlineData("1258Hello")]
        [InlineData("\uABCD")]
        [InlineData("\uABEE")]
        [InlineData("a\uABEE")]
        [InlineData("a\uABEEa")]
        [InlineData("a\uABEE\uABCDa")]
        public unsafe void ToStringTest(string s)
        {
            Assert.Equal(s, (new Utf8String(Encoding.UTF8.GetBytes(s))).ToString());
        }

        [Fact]
        public unsafe void Utf8EncodedCodePointFromChar0()
        {
            Utf8EncodedCodePoint ecp = new Utf8EncodedCodePoint('\u0000');
            Assert.Equal(1, ecp.Length);
            Assert.Equal(0x00, ecp.Byte0);
        }

        [Fact]
        public unsafe void Utf8EncodedCodePointFromChar1()
        {
            Utf8EncodedCodePoint ecp = new Utf8EncodedCodePoint('\u007F');
            Assert.Equal(1, ecp.Length);
            Assert.Equal(0x7F, ecp.Byte0);
        }

        [Fact]
        public unsafe void Utf8EncodedCodePointFromChar2()
        {
            Utf8EncodedCodePoint ecp = new Utf8EncodedCodePoint('\u0080');
            Assert.Equal(2, ecp.Length);
            Assert.Equal(0xC2, ecp.Byte0);
            Assert.Equal(0x80, ecp.Byte1);
        }

        [Fact]
        public unsafe void Utf8EncodedCodePointFromChar3()
        {
            Utf8EncodedCodePoint ecp = new Utf8EncodedCodePoint('\u01ED');
            Assert.Equal(2, ecp.Length);
            Assert.Equal(0xC7, ecp.Byte0);
            Assert.Equal(0xAD, ecp.Byte1);
        }

        [Fact]
        public unsafe void Utf8EncodedCodePointFromChar4()
        {
            Utf8EncodedCodePoint ecp = new Utf8EncodedCodePoint('\u07FF');
            Assert.Equal(2, ecp.Length);
            Assert.Equal(0xDF, ecp.Byte0);
            Assert.Equal(0xBF, ecp.Byte1);
        }

        [Fact]
        public unsafe void Utf8EncodedCodePointFromChar5()
        {
            Utf8EncodedCodePoint ecp = new Utf8EncodedCodePoint('\u0800');
            Assert.Equal(3, ecp.Length);
            Assert.Equal(0xE0, ecp.Byte0);
            Assert.Equal(0xA0, ecp.Byte1);
            Assert.Equal(0x80, ecp.Byte2);
        }

        [Fact]
        public unsafe void Utf8EncodedCodePointFromChar6()
        {
            Utf8EncodedCodePoint ecp = new Utf8EncodedCodePoint('\u1FA9');
            Assert.Equal(3, ecp.Length);
            Assert.Equal(0xE1, ecp.Byte0);
            Assert.Equal(0xBE, ecp.Byte1);
            Assert.Equal(0xA9, ecp.Byte2);
        }

        [Fact]
        public unsafe void Utf8EncodedCodePointFromChar7()
        {
            Utf8EncodedCodePoint ecp = new Utf8EncodedCodePoint('\uFFFF');
            Assert.Equal(3, ecp.Length);
            Assert.Equal(0xEF, ecp.Byte0);
            Assert.Equal(0xBF, ecp.Byte1);
            Assert.Equal(0xBF, ecp.Byte2);
        }
    }
}
