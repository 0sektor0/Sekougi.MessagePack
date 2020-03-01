using Xunit;



namespace Sekougi.MessagePack.Tests
{
    public class MessagePackWriterTest
    {
        [Fact]
        public void BoolTest()
        {
            using var buffer = new MessagePackBuffer(2);
            var writer = new MessagePackWriter(buffer);
            
            writer.Write(false);
            writer.Write(true);

            var data = buffer.GetAll();
            Assert.Equal(2, data.Length);
            Assert.Equal(0xc2, data[0]);
            Assert.Equal(0xc3, data[1]);
        }

        // values from -33 to 127 coverage
        [Fact]
        public void FixNumsTest()
        {
            var capacity = 39;
            using var buffer = new MessagePackBuffer(capacity);
            var writer = new MessagePackWriter(buffer);
                
            writer.Write((byte)0);
            writer.Write((sbyte)0);
            writer.Write((short)0);
            writer.Write((ushort)0);
            writer.Write((int)0);
            writer.Write((uint)0);
            writer.Write((long)0);
            writer.Write((ulong)0);
                
            writer.Write((byte)64);
            writer.Write((sbyte)64);
            writer.Write((short)64);
            writer.Write((ushort)64);
            writer.Write((int)64);
            writer.Write((uint)64);
            writer.Write((long)64);
            writer.Write((ulong)64);
                
            writer.Write((byte)127);
            writer.Write((sbyte)127);
            writer.Write((short)127);
            writer.Write((ushort)127);
            writer.Write((int)127);
            writer.Write((uint)127);
            writer.Write((long)127);
            writer.Write((ulong)127);
                
            writer.Write((sbyte)-1);
            writer.Write((short)-1);
            writer.Write((int)-1);
            writer.Write((long)-1);
                
            writer.Write((sbyte)-16);
            writer.Write((short)-16);
            writer.Write((int)-16);
            writer.Write((long)-16);
                
            writer.Write((sbyte)-32);
            writer.Write((short)-32);
            writer.Write((int)-32);
            writer.Write((long)-32);
                
            // out of bounds
            writer.Write((long) 1);
            writer.Write((long) -33);

            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);

            var data0 = data.Slice(0, 8);
            foreach (var value in data0)
            {
                Assert.Equal(value, 0x00);
            }
                
            var data64 = data.Slice(8, 8);
            foreach (var value in data64)
            {
                Assert.Equal(value, 0x40);
            }
                
            var data127 = data.Slice(16, 8);
            foreach (var value in data127)
            {
                Assert.Equal(value, 0x7f);
            }
                
            var dataNegative1 = data.Slice(24, 4);
            foreach (var value in dataNegative1)
            {
                Assert.Equal(value, 0xff);
            }
                
            var dataNegative16 = data.Slice(28, 4);
            foreach (var value in dataNegative16)
            {
                Assert.Equal(value, 0xf0);
            }
                
            var dataNegative32 = data.Slice(32, 4);
            foreach (var value in dataNegative32)
            {
                Assert.Equal(value, 0xe0);
            }

            var outOfBoundsValues = data.Slice(36, 3);
            Assert.Equal(outOfBoundsValues[0], 1);
            Assert.Equal(outOfBoundsValues[1], 208);
            Assert.Equal(outOfBoundsValues[2], 223);
        }

        [Fact]
        public void SbyteBoundaryTest()
        {
            var capacity = 3;
            using var buffer = new MessagePackBuffer(capacity);
            var writer = new MessagePackWriter(buffer);
            
            writer.Write(sbyte.MinValue);
            writer.Write(sbyte.MaxValue);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            Assert.Equal(data[0], MessagePackTypeCode.INT8);
            Assert.Equal(data[1], 128);
            Assert.Equal(data[2], 127);
        }
        
        [Fact]
        public void ByteBoundaryTest()
        {
            var capacity = 2;
            using var buffer = new MessagePackBuffer(capacity);
            var writer = new MessagePackWriter(buffer);
            
            writer.Write(byte.MaxValue);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            Assert.Equal(data[0], MessagePackTypeCode.UINT8);
            Assert.Equal(data[1], 255);
        }
        
        [Fact]
        public void ShortBoundaryTest()
        {
            var capacity = 6;
            using var buffer = new MessagePackBuffer(capacity);
            var writer = new MessagePackWriter(buffer);
            
            writer.Write(short.MinValue);
            writer.Write(short.MaxValue);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            
            var firstValue = data.Slice(0, 3);
            Assert.Equal(firstValue[0], MessagePackTypeCode.INT16);
            Assert.Equal(firstValue[1], 128);
            Assert.Equal(firstValue[2], 0);
            
            var secondValue = data.Slice(3, 3);
            Assert.Equal(secondValue[0], MessagePackTypeCode.INT16);
            Assert.Equal(secondValue[1], 127);
            Assert.Equal(secondValue[2], 255);
        }
        
        [Fact]
        public void UshortBoundaryTest()
        {
            var capacity = 3;
            using var buffer = new MessagePackBuffer(capacity);
            var writer = new MessagePackWriter(buffer);
            
            writer.Write(ushort.MaxValue);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            
            Assert.Equal(data[0], MessagePackTypeCode.UINT16);
            Assert.Equal(data[1], 255);
            Assert.Equal(data[2], 255);
        }
        
        [Fact]
        public void IntBoundaryTest()
        {
            var capacity = 10;
            using var buffer = new MessagePackBuffer(capacity);
            var writer = new MessagePackWriter(buffer);
            
            writer.Write(int.MinValue);
            writer.Write(int.MaxValue);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            
            var firstValue = data.Slice(0, 5);
            Assert.Equal(firstValue[0], MessagePackTypeCode.INT32);
            Assert.Equal(firstValue[1], 128);
            Assert.Equal(firstValue[2], 0);
            Assert.Equal(firstValue[3], 0);
            Assert.Equal(firstValue[4], 0);
            
            var secondValue = data.Slice(5, 5);
            Assert.Equal(secondValue[0], MessagePackTypeCode.INT32);
            Assert.Equal(secondValue[1], 127);
            Assert.Equal(secondValue[2], 255);
            Assert.Equal(secondValue[3], 255);
            Assert.Equal(secondValue[4], 255);
        }
        
        [Fact]
        public void UintBoundaryTest()
        {
            var capacity = 5;
            using var buffer = new MessagePackBuffer(capacity);
            var writer = new MessagePackWriter(buffer);
            
            writer.Write(uint.MaxValue);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            
            Assert.Equal(data[0], MessagePackTypeCode.UINT32);
            Assert.Equal(data[1], 255);
            Assert.Equal(data[2], 255);
            Assert.Equal(data[3], 255);
            Assert.Equal(data[4], 255);
        }
        
        [Fact]
        public void LongBoundaryTest()
        {
            var capacity = 18;
            using var buffer = new MessagePackBuffer(capacity);
            var writer = new MessagePackWriter(buffer);
            
            writer.Write(long.MaxValue);
            writer.Write(long.MinValue);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            
            var firstValue = data.Slice(0, 9);
            Assert.Equal(firstValue[0], MessagePackTypeCode.INT64);
            Assert.Equal(firstValue[1], 127);
            Assert.Equal(firstValue[2], 255);
            Assert.Equal(firstValue[3], 255);
            Assert.Equal(firstValue[4], 255);
            Assert.Equal(firstValue[5], 255);
            Assert.Equal(firstValue[6], 255);
            Assert.Equal(firstValue[7], 255);
            Assert.Equal(firstValue[8], 255);
            
            var secondValue = data.Slice(9, 9);
            Assert.Equal(secondValue[0], MessagePackTypeCode.INT64);
            Assert.Equal(secondValue[1], 128);
            Assert.Equal(secondValue[2], 0);
            Assert.Equal(secondValue[3], 0);
            Assert.Equal(secondValue[4], 0);
            Assert.Equal(secondValue[5], 0);
            Assert.Equal(secondValue[6], 0);
            Assert.Equal(secondValue[7], 0);
            Assert.Equal(secondValue[8], 0);
        }
        
        [Fact]
        public void UlongBoundaryTest()
        {
            var capacity = 9;
            using var buffer = new MessagePackBuffer(capacity);
            var writer = new MessagePackWriter(buffer);
            
            writer.Write(ulong.MaxValue);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            
            Assert.Equal(data[0], MessagePackTypeCode.UINT64);
            Assert.Equal(data[1], 255);
            Assert.Equal(data[2], 255);
            Assert.Equal(data[3], 255);
            Assert.Equal(data[4], 255);
            Assert.Equal(data[5], 255);
            Assert.Equal(data[6], 255);
            Assert.Equal(data[7], 255);
            Assert.Equal(data[8], 255);
        }
    }
}