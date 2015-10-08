using System.Buffers;
using System.Collections.Generic;
using Xunit;

namespace System.Text.Utf8.Tests
{
    public class Utf8StringTests
    {
        [Fact]
        public void NativePoolBasics()
        {
            using (var pool = new NativeBufferPool(256, 10))
            {
                List<ByteSpan> buffers = new List<ByteSpan>();
                for (byte i = 0; i < 10; i++)
                {
                    var buffer = pool.Rent();
                    buffers.Add(buffer);
                    for (int bi = 0; bi < buffer.Length; bi++)
                    {
                        buffer[bi] = i;
                    }
                }
                for (byte i = 0; i < 10; i++)
                {
                    var buffer = buffers[i];
                    for (int bi = 0; bi < buffer.Length; bi++)
                    {
                        Assert.Equal(i, buffer[bi]);
                    }
                    pool.Return(buffer);
                }
            }
        }

        [Fact]
        public void ByteSpanEmptyCreateArrayTest()
        {
            var empty = ByteSpan.Empty;
            var array = empty.CreateArray();
            Assert.Equal(0, array.Length);
        }
        
        [Fact]
        public unsafe void ByteSpanEqualsTestsTwoDifferentBuffersSameValues()
        {
            byte[] buffer1 = new byte[128];
            byte[] buffer2 = new byte[128];
            
            for (int i = 0; i < 128; i++)
            {
                buffer1[i] = (byte)(129 - i);
                buffer2[i] = (byte)(129 - i);
            }
            
            fixed (byte* buffer1pinned = buffer1) fixed (byte* buffer2pinned = buffer2)
            {
                ByteSpan b1 = new ByteSpan(buffer1pinned, 128);
                ByteSpan b2 = new ByteSpan(buffer2pinned, 128);
                
                for (int i = 0; i < 128; i++)
                {
                    Assert.True(b1.Slice(i).Equals(b2.Slice(i)));
                }
            }
        }
        
        [Fact]
        public unsafe void ByteSpanEqualsTestsTwoDifferentBuffersOneValueDifferent()
        {
            byte[] buffer1 = new byte[128];
            byte[] buffer2 = new byte[128];
            
            for (int i = 0; i < 128; i++)
            {
                buffer1[i] = (byte)(129 - i);
                buffer2[i] = (byte)(129 - i);
            }
            
            fixed (byte* buffer1pinned = buffer1) fixed (byte* buffer2pinned = buffer2)
            {
                ByteSpan b1 = new ByteSpan(buffer1pinned, 128);
                ByteSpan b2 = new ByteSpan(buffer2pinned, 128);
                
                for (int i = 0; i < 128; i++)
                {
                    for (int diffPosition = i; diffPosition < 128; diffPosition++)
                    {
                        buffer1[diffPosition] = 130;
                        Assert.False(b1.Slice(i).Equals(b2.Slice(i)));
                    }
                }
            }
        }
    }
}
