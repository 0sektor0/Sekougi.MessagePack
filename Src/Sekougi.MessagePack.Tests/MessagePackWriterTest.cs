using System;
using System.Text;
using Xunit;



namespace Sekougi.MessagePack.Tests
{
    public class MessagePackWriterTest
    {
        [Fact]
        public void BoolTest()
        {
            using var buffer = new MessagePackBuffer(2);
            MessagePackWriter.Write(false, buffer);
            MessagePackWriter.Write(true, buffer);

            var data = buffer.GetAll();
            Assert.Equal(2, data.Length);
            Assert.Equal(MessagePackTypeCode.FALSE, data[0]);
            Assert.Equal(MessagePackTypeCode.TRUE, data[1]);
        }

        // values from -33 to 127 coverage
        [Fact]
        public void FixNumsTest()
        {
            var capacity = 39;
            using var buffer = new MessagePackBuffer(capacity);
                
            MessagePackWriter.Write((byte)0, buffer);
            MessagePackWriter.Write((sbyte)0, buffer);
            MessagePackWriter.Write((short)0, buffer);
            MessagePackWriter.Write((ushort)0, buffer);
            MessagePackWriter.Write((int)0, buffer);
            MessagePackWriter.Write((uint)0, buffer);
            MessagePackWriter.Write((long)0, buffer);
            MessagePackWriter.Write((ulong)0, buffer);
                
            MessagePackWriter.Write((byte)64, buffer);
            MessagePackWriter.Write((sbyte)64, buffer);
            MessagePackWriter.Write((short)64, buffer);
            MessagePackWriter.Write((ushort)64, buffer);
            MessagePackWriter.Write((int)64, buffer);
            MessagePackWriter.Write((uint)64, buffer);
            MessagePackWriter.Write((long)64, buffer);
            MessagePackWriter.Write((ulong)64, buffer);
                
            MessagePackWriter.Write((byte)127, buffer);
            MessagePackWriter.Write((sbyte)127, buffer);
            MessagePackWriter.Write((short)127, buffer);
            MessagePackWriter.Write((ushort)127, buffer);
            MessagePackWriter.Write((int)127, buffer);
            MessagePackWriter.Write((uint)127, buffer);
            MessagePackWriter.Write((long)127, buffer);
            MessagePackWriter.Write((ulong)127, buffer);
                
            MessagePackWriter.Write((sbyte)-1, buffer);
            MessagePackWriter.Write((short)-1, buffer);
            MessagePackWriter.Write((int)-1, buffer);
            MessagePackWriter.Write((long)-1, buffer);
                
            MessagePackWriter.Write((sbyte)-16, buffer);
            MessagePackWriter.Write((short)-16, buffer);
            MessagePackWriter.Write((int)-16, buffer);
            MessagePackWriter.Write((long)-16, buffer);
                
            MessagePackWriter.Write((sbyte)-32, buffer);
            MessagePackWriter.Write((short)-32, buffer);
            MessagePackWriter.Write((int)-32, buffer);
            MessagePackWriter.Write((long)-32, buffer);
                
            // out of bounds
            MessagePackWriter.Write((long) 1, buffer);
            MessagePackWriter.Write((long) -33, buffer);

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
            
            MessagePackWriter.Write(sbyte.MinValue, buffer);
            MessagePackWriter.Write(sbyte.MaxValue, buffer);
            
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
            
            MessagePackWriter.Write(byte.MaxValue, buffer);
            
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
            
            MessagePackWriter.Write(short.MinValue, buffer);
            MessagePackWriter.Write(short.MaxValue, buffer);
            
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
            
            MessagePackWriter.Write(ushort.MaxValue, buffer);
            
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
            
            MessagePackWriter.Write(int.MinValue, buffer);
            MessagePackWriter.Write(int.MaxValue, buffer);
            
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
            
            MessagePackWriter.Write(uint.MaxValue, buffer);
            
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
            
            MessagePackWriter.Write(long.MaxValue, buffer);
            MessagePackWriter.Write(long.MinValue, buffer);
            
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
            
            MessagePackWriter.Write(ulong.MaxValue, buffer);
            
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
        
        [Fact]
        public void FloatBoundaryTest()
        {
            var capacity = 10;
            using var buffer = new MessagePackBuffer(capacity);
            
            MessagePackWriter.Write(float.MaxValue, buffer);
            MessagePackWriter.Write(float.MinValue, buffer);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            
            var firstValue = data.Slice(0, 5);
            Assert.Equal(firstValue[0], MessagePackTypeCode.FLOAT32);
            Assert.Equal(firstValue[1], 127);
            Assert.Equal(firstValue[2], 127);
            Assert.Equal(firstValue[3], 255);
            Assert.Equal(firstValue[4], 255);
            
            var secondValue = data.Slice(5, 5);
            Assert.Equal(secondValue[0], MessagePackTypeCode.FLOAT32);
            Assert.Equal(secondValue[1], 255);
            Assert.Equal(secondValue[2], 127);
            Assert.Equal(secondValue[3], 255);
            Assert.Equal(secondValue[4], 255);
        }
                
        [Fact]
        public void DoubleBoundaryTest()
        {
            var capacity = 18;
            using var buffer = new MessagePackBuffer(capacity);
            
            MessagePackWriter.Write(double.MinValue, buffer);
            MessagePackWriter.Write(double.MaxValue, buffer);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            
            var firstValue = data.Slice(0, 9);
            Assert.Equal(firstValue[0], MessagePackTypeCode.FLOAT64);
            Assert.Equal(firstValue[1], 255);
            Assert.Equal(firstValue[2], 239);
            Assert.Equal(firstValue[3], 255);
            Assert.Equal(firstValue[4], 255);
            Assert.Equal(firstValue[5], 255);
            Assert.Equal(firstValue[6], 255);
            Assert.Equal(firstValue[7], 255);
            Assert.Equal(firstValue[8], 255);
            
            var secondValue = data.Slice(9, 9);
            Assert.Equal(secondValue[0], MessagePackTypeCode.FLOAT64);
            Assert.Equal(secondValue[1], 127);
            Assert.Equal(secondValue[2], 239);
            Assert.Equal(secondValue[3], 255);
            Assert.Equal(secondValue[4], 255);
            Assert.Equal(secondValue[5], 255);
            Assert.Equal(secondValue[6], 255);
            Assert.Equal(secondValue[7], 255);
            Assert.Equal(secondValue[8], 255);
        }

        [Fact]
        public void NullTest()
        {
            var capacity = 1;
            using var buffer = new MessagePackBuffer(capacity);
            
            MessagePackWriter.WriteNull(buffer);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            Assert.Equal(data[0], MessagePackTypeCode.NIL);
        }

        [Fact]
        public void NullStringTest()
        {
            var capacity = 1;
            using var buffer = new MessagePackBuffer(capacity);
            
            MessagePackWriter.Write(null, Encoding.UTF8, buffer);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            Assert.Equal(data[0], MessagePackTypeCode.NIL);
        }

        [Fact] 
        public void SmallStringTest()
        {
            var capacity = 2;
            using var buffer = new MessagePackBuffer(capacity);
            
            MessagePackWriter.Write("a", Encoding.UTF8, buffer);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            Assert.Equal(data[0], 161);
            Assert.Equal(data[1], 97);
        }

        [Fact]
        public void TimeStamp32Test()
        {
            var capacity = 6;
            using var buffer = new MessagePackBuffer(6);
            
            var date = new DateTime(1970,1,1);
            MessagePackWriter.Write(date, buffer);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            Assert.Equal(data[0], MessagePackTypeCode.TIMESTAMP32);
            Assert.Equal(data[1], 255);
            Assert.Equal(data[2], 0);
            Assert.Equal(data[3], 0);
            Assert.Equal(data[4], 0);
            Assert.Equal(data[5], 0);
        }

        [Fact]
        public void TimeStamp64Test()
        {
            var capacity = 10;
            using var buffer = new MessagePackBuffer(6);
            
            var date = new DateTime(1970,1,1, 0, 0, 0, 1);
            MessagePackWriter.Write(date, buffer);
            
            var data = buffer.GetAll();
            Assert.Equal(data.Length, capacity);
            Assert.Equal(data[0], MessagePackTypeCode.TIMESTAMP64);
            Assert.Equal(data[1], 255);
            Assert.Equal(data[2], 0);
            Assert.Equal(data[3], 61);
            Assert.Equal(data[4], 9);
            Assert.Equal(data[5], 0);
            Assert.Equal(data[6], 0);
            Assert.Equal(data[7], 0);
            Assert.Equal(data[8], 0);
        }
    }
}