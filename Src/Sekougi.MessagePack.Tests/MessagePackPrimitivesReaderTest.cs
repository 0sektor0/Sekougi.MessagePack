using System;
using System.Text;
using Xunit;

namespace Sekougi.MessagePack.Tests
{
    public class MessagePackPrimitivesReaderTest
    {
        [Fact]
        public void SignedNumbersTest()
        {
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.Write(-1, buffer);
            MessagePackPrimitivesWriter.Write(1, buffer);
            MessagePackPrimitivesWriter.Write(sbyte.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(sbyte.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(short.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(short.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(int.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(int.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(long.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(long.MaxValue, buffer);

            buffer.Position = 0;
            Assert.Equal(-1, MessagePackPrimitivesReader.ReadInt(buffer));
            Assert.Equal(1, MessagePackPrimitivesReader.ReadInt(buffer));
            Assert.Equal(sbyte.MinValue, MessagePackPrimitivesReader.ReadSbyte(buffer));
            Assert.Equal(sbyte.MaxValue, MessagePackPrimitivesReader.ReadSbyte(buffer));
            Assert.Equal(short.MinValue, MessagePackPrimitivesReader.ReadShort(buffer));
            Assert.Equal(short.MaxValue, MessagePackPrimitivesReader.ReadShort(buffer));
            Assert.Equal(int.MinValue, MessagePackPrimitivesReader.ReadInt(buffer));
            Assert.Equal(int.MaxValue, MessagePackPrimitivesReader.ReadInt(buffer));
            Assert.Equal(long.MinValue, MessagePackPrimitivesReader.ReadLong(buffer));
            Assert.Equal(long.MaxValue, MessagePackPrimitivesReader.ReadLong(buffer));
        }

        [Fact]
        public void UnsignedNumbersTest()
        {
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.Write(byte.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(ushort.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(uint.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(ulong.MaxValue, buffer);

            buffer.Position = 0;
            Assert.Equal(byte.MaxValue, MessagePackPrimitivesReader.ReadByte(buffer));
            Assert.Equal(ushort.MaxValue, MessagePackPrimitivesReader.ReadUshort(buffer));
            Assert.Equal(uint.MaxValue, MessagePackPrimitivesReader.ReadUint(buffer));
            Assert.Equal(ulong.MaxValue, MessagePackPrimitivesReader.ReadUlong(buffer));
        }

        [Fact]
        public void FloatingPointNumbersTest()
        {
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.Write(float.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(float.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(double.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(double.MinValue, buffer);

            buffer.Position = 0;
            Assert.Equal(float.MaxValue, MessagePackPrimitivesReader.ReadFloat(buffer));
            Assert.Equal(float.MinValue, MessagePackPrimitivesReader.ReadFloat(buffer));
            Assert.Equal(double.MaxValue, MessagePackPrimitivesReader.ReadDouble(buffer));
            Assert.Equal(double.MinValue, MessagePackPrimitivesReader.ReadDouble(buffer));
        }

        [Fact]
        public void StringTest()
        {
            var shortStr = new string(new char[31]);
            var str8 = new string(new char[byte.MaxValue - 10]);
            var str16 = new string(new char[ushort.MaxValue - 10]);
            var str32 = new string(new char[ushort.MaxValue + 10]);
            
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.Write(null, Encoding.UTF8, buffer);
            MessagePackPrimitivesWriter.Write("", Encoding.UTF8, buffer);
            MessagePackPrimitivesWriter.Write(shortStr, Encoding.UTF8, buffer);
            MessagePackPrimitivesWriter.Write(str8, Encoding.UTF8, buffer);
            MessagePackPrimitivesWriter.Write(str16, Encoding.UTF8, buffer);
            MessagePackPrimitivesWriter.Write(str32, Encoding.UTF8, buffer);

            buffer.Position = 0;
            Assert.Equal(null,MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8));
            Assert.Equal("", MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8));
            Assert.Equal(shortStr, MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8));
            Assert.Equal(str8, MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8));
            Assert.Equal(str16, MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8));
            Assert.Equal(str32, MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8));
        }

        [Fact]
        public void DateTimeTest()
        {
            var dateTimeZero = new DateTime(1970,1,1);
            var dateTime = new DateTime(2020, 1, 1, 1, 1, 1, 1);
            
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.Write(dateTimeZero, buffer);
            MessagePackPrimitivesWriter.Write(dateTime, buffer);

            buffer.Position = 0;
            Assert.Equal(dateTimeZero, MessagePackPrimitivesReader.ReadDateTime(buffer));
            Assert.Equal(dateTime, MessagePackPrimitivesReader.ReadDateTime(buffer));
        }

        [Fact]
        public void BinaryTest()
        {
            var binaryData8 = new byte[byte.MaxValue - 1];
            var binaryData16 = new byte[ushort.MaxValue - 1];
            var binaryData32= new byte[ushort.MaxValue + 1];
            
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.WriteBinary(binaryData8, buffer);
            MessagePackPrimitivesWriter.WriteBinary(binaryData16, buffer);
            MessagePackPrimitivesWriter.WriteBinary(binaryData32, buffer);

            buffer.Position = 0;
            Assert.Equal(binaryData8, MessagePackPrimitivesReader.ReadBinary(buffer));
            Assert.Equal(binaryData16, MessagePackPrimitivesReader.ReadBinary(buffer));
            Assert.Equal(binaryData32, MessagePackPrimitivesReader.ReadBinary(buffer));
        }

        [Fact]
        public void ArrayLengthTest()
        {
            var arrayLen16 = ushort.MaxValue - 1;
            var arrayLen32 = ushort.MaxValue + 1;
            
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.WriteArrayHeader(0, buffer);
            MessagePackPrimitivesWriter.WriteArrayHeader(1, buffer);
            MessagePackPrimitivesWriter.WriteArrayHeader(arrayLen16, buffer);
            MessagePackPrimitivesWriter.WriteArrayHeader(arrayLen32, buffer);

            buffer.Position = 0;
            Assert.Equal(0, MessagePackPrimitivesReader.ReadArrayLength(buffer));
            Assert.Equal(1, MessagePackPrimitivesReader.ReadArrayLength(buffer));
            Assert.Equal(arrayLen16, MessagePackPrimitivesReader.ReadArrayLength(buffer));
            Assert.Equal(arrayLen32, MessagePackPrimitivesReader.ReadArrayLength(buffer));
        }

        [Fact]
        public void MapLengthTest()
        {
            var mapLen16 = ushort.MaxValue - 1;
            var mapLen32 = ushort.MaxValue + 1;
            
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.WriteDictionaryHeader(0, buffer);
            MessagePackPrimitivesWriter.WriteDictionaryHeader(1, buffer);
            MessagePackPrimitivesWriter.WriteDictionaryHeader(mapLen16, buffer);
            MessagePackPrimitivesWriter.WriteDictionaryHeader(mapLen32, buffer);

            buffer.Position = 0;
            Assert.Equal(0, MessagePackPrimitivesReader.ReadDictionaryLength(buffer));
            Assert.Equal(1, MessagePackPrimitivesReader.ReadDictionaryLength(buffer));
            Assert.Equal(mapLen16, MessagePackPrimitivesReader.ReadDictionaryLength(buffer));
            Assert.Equal(mapLen32, MessagePackPrimitivesReader.ReadDictionaryLength(buffer));
        }
    }
}