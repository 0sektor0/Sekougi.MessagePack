using System;
using Sekougi.MessagePack.Serializers;
using Xunit;



namespace Sekougi.MessagePack.Tests
{
    public class MessagePackSerializationTests
    {
        [Fact]
        public void BoolTest()
        {
            using var buffer = new MessagePackStreamBuffer(2);
            var serializer = MessagePackSerializersRepository.Get<bool>();
            serializer.Serialize(false, buffer);
            serializer.Serialize(true, buffer);

            var data = buffer.GetAll();
            Assert.Equal(2, data.Length);
            Assert.Equal(0xc2, data[0]);
            Assert.Equal(0xc3, data[1]);
        }

        [Fact]
        public void FixNumsTest()
        {
            var capacity = 39;
            using var buffer = new MessagePackStreamBuffer(capacity);

            var byteSerializer = MessagePackSerializersRepository.Get<byte>();
            var sbyteSerializer = MessagePackSerializersRepository.Get<sbyte>();
            var shortSerializer = MessagePackSerializersRepository.Get<short>();
            var ushortSerializer = MessagePackSerializersRepository.Get<ushort>();
            var intSerializer = MessagePackSerializersRepository.Get<int>();
            var uintSerializer = MessagePackSerializersRepository.Get<uint>();
            var longSerializer = MessagePackSerializersRepository.Get<long>();
            var ulongSerializer = MessagePackSerializersRepository.Get<ulong>();
                
            byteSerializer.Serialize(0, buffer);
            sbyteSerializer.Serialize(0, buffer);
            shortSerializer.Serialize(0, buffer);
            ushortSerializer.Serialize(0, buffer);
            intSerializer.Serialize(0, buffer);
            uintSerializer.Serialize(0, buffer);
            longSerializer.Serialize(0, buffer);
            ulongSerializer.Serialize(0, buffer);
            
            byteSerializer.Serialize(64, buffer);
            sbyteSerializer.Serialize(64, buffer);
            shortSerializer.Serialize(64, buffer);
            ushortSerializer.Serialize(64, buffer);
            intSerializer.Serialize(64, buffer);
            uintSerializer.Serialize(64, buffer);
            longSerializer.Serialize(64, buffer);
            ulongSerializer.Serialize(64, buffer);
            
            byteSerializer.Serialize(127, buffer);
            sbyteSerializer.Serialize(127, buffer);
            shortSerializer.Serialize(127, buffer);
            ushortSerializer.Serialize(127, buffer);
            intSerializer.Serialize(127, buffer);
            uintSerializer.Serialize(127, buffer);
            longSerializer.Serialize(127, buffer);
            ulongSerializer.Serialize(127, buffer);
            
            sbyteSerializer.Serialize(-1, buffer);
            shortSerializer.Serialize(-1, buffer);
            intSerializer.Serialize(-1, buffer);
            longSerializer.Serialize(-1, buffer);
            
            sbyteSerializer.Serialize(-16, buffer);
            shortSerializer.Serialize(-16, buffer);
            intSerializer.Serialize(-16, buffer);
            longSerializer.Serialize(-16, buffer);
            
            sbyteSerializer.Serialize(-32, buffer);
            shortSerializer.Serialize(-32, buffer);
            intSerializer.Serialize(-32, buffer);
            longSerializer.Serialize(-32, buffer);
            
            longSerializer.Serialize(1, buffer);
            longSerializer.Serialize(-33, buffer);

            Assert.Equal(buffer.Length, capacity);

            var data0 = buffer.GetPart(0, 8);
            foreach (var value in data0)
            {
                Assert.Equal(value, 0x00);
            }
                
            var data64 = buffer.GetPart(8, 8);
            foreach (var value in data64)
            {
                Assert.Equal(value, 0x40);
            }
                
            var data127 = buffer.GetPart(16, 8);
            foreach (var value in data127)
            {
                Assert.Equal(value, 0x7f);
            }
                
            var dataNegative1 = buffer.GetPart(24, 4);
            foreach (var value in dataNegative1)
            {
                Assert.Equal(value, 0xff);
            }
                
            var dataNegative16 = buffer.GetPart(28, 4);
            foreach (var value in dataNegative16)
            {
                Assert.Equal(value, 0xf0);
            }
                
            var dataNegative32 = buffer.GetPart(32, 4);
            foreach (var value in dataNegative32)
            {
                Assert.Equal(value, 0xe0);
            }

            var outOfBoundsValues = buffer.GetPart(36, 3);
            Assert.Equal(outOfBoundsValues[0], 1);
            Assert.Equal(outOfBoundsValues[1], 208);
            Assert.Equal(outOfBoundsValues[2], 223);
        }

        [Fact]
        public void SbyteBoundaryTest()
        {
            var capacity = 3;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var sbyteSerializer = MessagePackSerializersRepository.Get<sbyte>();
            
            sbyteSerializer.Serialize(sbyte.MinValue, buffer);
            sbyteSerializer.Serialize(sbyte.MaxValue, buffer);
            
            Assert.Equal(buffer.Length, capacity);
            
            var data = buffer.GetAll();
            Assert.Equal(data[0], 208);
            Assert.Equal(data[1], 128);
            Assert.Equal(data[2], 127);
        }
        
        [Fact]
        public void ByteBoundaryTest()
        {
            var capacity = 2;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<byte>();
            
            serializer.Serialize(byte.MaxValue, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var data = buffer.GetAll();
            Assert.Equal(data[0], 204);
            Assert.Equal(data[1], 255);
        }
        
        [Fact]
        public void ShortBoundaryTest()
        {
            var capacity = 6;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<short>();
            
            serializer.Serialize(short.MinValue, buffer);
            serializer.Serialize(short.MaxValue, buffer);
            
            Assert.Equal(buffer.Length, capacity);
            
            var firstValue = buffer.GetPart(0, 3);
            Assert.Equal(firstValue[0], 209);
            Assert.Equal(firstValue[1], 128);
            Assert.Equal(firstValue[2], 0);
            
            var secondValue = buffer.GetPart(3, 3);
            Assert.Equal(secondValue[0], 209);
            Assert.Equal(secondValue[1], 127);
            Assert.Equal(secondValue[2], 255);
        }
        
        [Fact]
        public void UshortBoundaryTest()
        {
            var capacity = 3;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<ushort>();
            
            serializer.Serialize(ushort.MaxValue, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var data = buffer.GetAll();
            Assert.Equal(data[0], 205);
            Assert.Equal(data[1], 255);
            Assert.Equal(data[2], 255);
        }
        
        [Fact]
        public void IntBoundaryTest()
        {
            var capacity = 10;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<int>();
            
            serializer.Serialize(int.MinValue, buffer);
            serializer.Serialize(int.MaxValue, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var firstValue = buffer.GetPart(0, 5);
            Assert.Equal(firstValue[0], 210);
            Assert.Equal(firstValue[1], 128);
            Assert.Equal(firstValue[2], 0);
            Assert.Equal(firstValue[3], 0);
            Assert.Equal(firstValue[4], 0);
            
            var secondValue = buffer.GetPart(5, 5);
            Assert.Equal(secondValue[0], 210);
            Assert.Equal(secondValue[1], 127);
            Assert.Equal(secondValue[2], 255);
            Assert.Equal(secondValue[3], 255);
            Assert.Equal(secondValue[4], 255);
        }
        
        [Fact]
        public void UintBoundaryTest()
        {
            var capacity = 5;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<uint>();
            
            serializer.Serialize(uint.MaxValue, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var data = buffer.GetAll();
            Assert.Equal(data[0], 206);
            Assert.Equal(data[1], 255);
            Assert.Equal(data[2], 255);
            Assert.Equal(data[3], 255);
            Assert.Equal(data[4], 255);
        }
        
        [Fact]
        public void LongBoundaryTest()
        {
            var capacity = 18;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<long>();
            
            serializer.Serialize(long.MaxValue, buffer);
            serializer.Serialize(long.MinValue, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var firstValue = buffer.GetPart(0, 9);
            Assert.Equal(firstValue[0], 211);
            Assert.Equal(firstValue[1], 127);
            Assert.Equal(firstValue[2], 255);
            Assert.Equal(firstValue[3], 255);
            Assert.Equal(firstValue[4], 255);
            Assert.Equal(firstValue[5], 255);
            Assert.Equal(firstValue[6], 255);
            Assert.Equal(firstValue[7], 255);
            Assert.Equal(firstValue[8], 255);
            
            var secondValue = buffer.GetPart(9, 9);
            Assert.Equal(secondValue[0], 211);
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
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<ulong>();
            
            serializer.Serialize(ulong.MaxValue, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var data = buffer.GetAll();
            Assert.Equal(data[0], 207);
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
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<float>();
            
            serializer.Serialize(float.MaxValue, buffer);
            serializer.Serialize(float.MinValue, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var firstValue = buffer.GetPart(0, 5);
            Assert.Equal(firstValue[0], 202);
            Assert.Equal(firstValue[1], 127);
            Assert.Equal(firstValue[2], 127);
            Assert.Equal(firstValue[3], 255);
            Assert.Equal(firstValue[4], 255);
            
            var secondValue = buffer.GetPart(5, 5);
            Assert.Equal(secondValue[0], 202);
            Assert.Equal(secondValue[1], 255);
            Assert.Equal(secondValue[2], 127);
            Assert.Equal(secondValue[3], 255);
            Assert.Equal(secondValue[4], 255);
        }
                
        [Fact]
        public void DoubleBoundaryTest()
        {
            var capacity = 18;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<double>();
            
            serializer.Serialize(double.MinValue, buffer);
            serializer.Serialize(double.MaxValue, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var firstValue = buffer.GetPart(0, 9);
            Assert.Equal(firstValue[0], 203);
            Assert.Equal(firstValue[1], 255);
            Assert.Equal(firstValue[2], 239);
            Assert.Equal(firstValue[3], 255);
            Assert.Equal(firstValue[4], 255);
            Assert.Equal(firstValue[5], 255);
            Assert.Equal(firstValue[6], 255);
            Assert.Equal(firstValue[7], 255);
            Assert.Equal(firstValue[8], 255);
            
            var secondValue = buffer.GetPart(9, 9);
            Assert.Equal(secondValue[0], 203);
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
        public void NullStringTest()
        {
            var capacity = 1;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<string>();
            
            serializer.Serialize(null, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var data = buffer.GetAll();
            Assert.Equal(data[0], 192);
        }

        [Fact] 
        public void SmallStringTest()
        {
            var capacity = 2;
            using var buffer = new MessagePackStreamBuffer(capacity);
            var serializer = MessagePackSerializersRepository.Get<string>();
            
            serializer.Serialize("a", buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var data = buffer.GetAll();
            Assert.Equal(data[0], 161);
            Assert.Equal(data[1], 97);
        }

        [Fact]
        public void TimeStamp32Test()
        {
            var capacity = 6;
            using var buffer = new MessagePackStreamBuffer(6);
            var serializer = MessagePackSerializersRepository.Get<DateTime>();
            
            var date = new DateTime(1970,1,1);
            serializer.Serialize(date, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var data = buffer.GetAll();
            Assert.Equal(data[0], 214);
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
            using var buffer = new MessagePackStreamBuffer(6);
            var serializer = MessagePackSerializersRepository.Get<DateTime>();
            
            var date = new DateTime(1970,1,1, 0, 0, 0, 1);
            serializer.Serialize(date, buffer);
            Assert.Equal(buffer.Length, capacity);
            
            var data = buffer.GetAll();
            Assert.Equal(data[0], 215);
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