using System;
using Sekougi.MessagePack.Buffers;
using Xunit;



namespace Sekougi.MessagePack.Tests
{
    public class MessagePackDeserializationTests
    {
        [Fact]
        public void SignedNumbersTest()
        {
            using var buffer = new MessagePackMemoryStreamBuffer();

            var intSerializer = MessagePackSerializersRepository.Get<int>();
            var sbyteSerializer = MessagePackSerializersRepository.Get<sbyte>();
            var shortSerializer = MessagePackSerializersRepository.Get<short>();
            var longSerializer = MessagePackSerializersRepository.Get<long>();
            
            intSerializer.Serialize(-1, buffer);
            intSerializer.Serialize(1, buffer);
            sbyteSerializer.Serialize(sbyte.MinValue, buffer);
            sbyteSerializer.Serialize(sbyte.MaxValue, buffer);
            shortSerializer.Serialize(short.MinValue, buffer);
            shortSerializer.Serialize(short.MaxValue, buffer);
            intSerializer.Serialize(int.MinValue, buffer);
            intSerializer.Serialize(int.MaxValue, buffer);
            longSerializer.Serialize(long.MinValue, buffer);
            longSerializer.Serialize(long.MaxValue, buffer);

            buffer.Drop();
            Assert.Equal(-1, intSerializer.Deserialize(buffer));
            Assert.Equal(1, intSerializer.Deserialize(buffer));
            Assert.Equal(sbyte.MinValue, sbyteSerializer.Deserialize(buffer));
            Assert.Equal(sbyte.MaxValue, sbyteSerializer.Deserialize(buffer));
            Assert.Equal(short.MinValue, shortSerializer.Deserialize(buffer));
            Assert.Equal(short.MaxValue, shortSerializer.Deserialize(buffer));
            Assert.Equal(int.MinValue, intSerializer.Deserialize(buffer));
            Assert.Equal(int.MaxValue, intSerializer.Deserialize(buffer));
            Assert.Equal(long.MinValue, longSerializer.Deserialize(buffer));
            Assert.Equal(long.MaxValue, longSerializer.Deserialize(buffer));
        }

        [Fact]
        public void UnsignedNumbersTest()
        {
            using var buffer = new MessagePackMemoryStreamBuffer();
            
            var uintSerializer = MessagePackSerializersRepository.Get<uint>();
            var byteSerializer = MessagePackSerializersRepository.Get<byte>();
            var ushortSerializer = MessagePackSerializersRepository.Get<ushort>();
            var ulongSerializer = MessagePackSerializersRepository.Get<ulong>();
            
            byteSerializer.Serialize(byte.MaxValue, buffer);
            ushortSerializer.Serialize(ushort.MaxValue, buffer);
            uintSerializer.Serialize(uint.MaxValue, buffer);
            ulongSerializer.Serialize(ulong.MaxValue, buffer);

            buffer.Drop();
            Assert.Equal(byte.MaxValue, byteSerializer.Deserialize(buffer));
            Assert.Equal(ushort.MaxValue, ushortSerializer.Deserialize(buffer));
            Assert.Equal(uint.MaxValue, uintSerializer.Deserialize(buffer));
            Assert.Equal(ulong.MaxValue, ulongSerializer.Deserialize(buffer));
        }

        [Fact]
        public void FloatingPointNumbersTest()
        {
            using var buffer = new MessagePackMemoryStreamBuffer();
            
            var floatSerializer = MessagePackSerializersRepository.Get<float>();
            var doubleSerializer = MessagePackSerializersRepository.Get<double>();
            
            floatSerializer.Serialize(float.MaxValue, buffer);
            floatSerializer.Serialize(float.MinValue, buffer);
            doubleSerializer.Serialize(double.MaxValue, buffer);
            doubleSerializer.Serialize(double.MinValue, buffer);

            buffer.Drop();
            Assert.Equal(float.MaxValue, floatSerializer.Deserialize(buffer));
            Assert.Equal(float.MinValue, floatSerializer.Deserialize(buffer));
            Assert.Equal(double.MaxValue, doubleSerializer.Deserialize(buffer));
            Assert.Equal(double.MinValue, doubleSerializer.Deserialize(buffer));
        }

        [Fact]
        public void StringTest()
        {
            var shortStr = new string(new char[31]);
            var str8 = new string(new char[byte.MaxValue - 10]);
            var str16 = new string(new char[ushort.MaxValue - 10]);
            var str32 = new string(new char[ushort.MaxValue + 10]);
            
            using var buffer = new MessagePackMemoryStreamBuffer();
            var serializer = MessagePackSerializersRepository.Get<string>();
            
            serializer.Serialize(null, buffer);
            serializer.Serialize("", buffer);
            serializer.Serialize(shortStr, buffer);
            serializer.Serialize(str8, buffer);
            serializer.Serialize(str16, buffer);
            serializer.Serialize(str32, buffer);

            buffer.Drop();
            Assert.Equal(null,serializer.Deserialize(buffer));
            Assert.Equal("", serializer.Deserialize(buffer));
            
            var str = serializer.Deserialize(buffer);
            var count = str.Length;
            var count2 = shortStr.Length;
            Assert.Equal(shortStr, str);
            
            Assert.Equal(str8, serializer.Deserialize(buffer));
            Assert.Equal(str16, serializer.Deserialize(buffer));
            Assert.Equal(str32, serializer.Deserialize(buffer));
        }

        [Fact]
        public void DateTimeTest()
        {
            var dateTimeZero = new DateTime(1970,1,1);
            var dateTime = new DateTime(2020, 1, 1, 1, 1, 1, 1);
            
            using var buffer = new MessagePackMemoryStreamBuffer();
            var serializer = MessagePackSerializersRepository.Get<DateTime>();
            
            serializer.Serialize(dateTimeZero, buffer);
            serializer.Serialize(dateTime, buffer);

            buffer.Drop();
            Assert.Equal(dateTimeZero, serializer.Deserialize(buffer));
            Assert.Equal(dateTime, serializer.Deserialize(buffer));
        }

        [Fact]
        public void BinaryTest()
        {
            var binaryData8 = new byte[byte.MaxValue - 1];
            var binaryData16 = new byte[ushort.MaxValue - 1];
            var binaryData32= new byte[ushort.MaxValue + 1];
            
            using var buffer = new MessagePackMemoryStreamBuffer();
            var serializer = MessagePackSerializersRepository.Get<byte[]>();
            
            serializer.Serialize(binaryData8, buffer);
            serializer.Serialize(binaryData16, buffer);
            serializer.Serialize(binaryData32, buffer);

            buffer.Drop();
            Assert.Equal(binaryData8, serializer.Deserialize(buffer));
            Assert.Equal(binaryData16, serializer.Deserialize(buffer));
            Assert.Equal(binaryData32, serializer.Deserialize(buffer));
        }

        [Fact]
        public void ArrayTest()
        {
            var arrayEmpty = new string[0];
            var array1 = new string[1];
            var array16 = new string[ushort.MaxValue - 1];
            var array32 = new string[ushort.MaxValue + 1];
            
            using var buffer = new MessagePackMemoryStreamBuffer();
            var serializer = MessagePackSerializersRepository.Get<string[]>();
            
            serializer.Serialize(arrayEmpty, buffer);
            serializer.Serialize(array1, buffer);
            serializer.Serialize(array16, buffer);
            serializer.Serialize(array32, buffer);

            buffer.Drop();
            Assert.Equal(arrayEmpty, serializer.Deserialize(buffer));
            Assert.Equal(array1, serializer.Deserialize(buffer));
            Assert.Equal(array16, serializer.Deserialize(buffer));
            Assert.Equal(array32, serializer.Deserialize(buffer));
        }
    }
}