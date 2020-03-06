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
            Assert.Equal(MessagePackPrimitivesReader.ReadInt(buffer), -1);
            Assert.Equal(MessagePackPrimitivesReader.ReadInt(buffer), 1);
            Assert.Equal(MessagePackPrimitivesReader.ReadSbyte(buffer), sbyte.MinValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadSbyte(buffer), sbyte.MaxValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadShort(buffer), short.MinValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadShort(buffer), short.MaxValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadInt(buffer), int.MinValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadInt(buffer), int.MaxValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadLong(buffer), long.MinValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadLong(buffer), long.MaxValue);
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
            Assert.Equal(MessagePackPrimitivesReader.ReadByte(buffer), byte.MaxValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadUshort(buffer), ushort.MaxValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadUint(buffer), uint.MaxValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadUlong(buffer), ulong.MaxValue);
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
            Assert.Equal(MessagePackPrimitivesReader.ReadFloat(buffer), float.MaxValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadFloat(buffer), float.MinValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadDouble(buffer), double.MaxValue);
            Assert.Equal(MessagePackPrimitivesReader.ReadDouble(buffer), double.MinValue);
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
            Assert.Equal(MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8), null);
            Assert.Equal(MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8), "");
            Assert.Equal(MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8), shortStr);
            Assert.Equal(MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8), str8);
            Assert.Equal(MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8), str16);
            Assert.Equal(MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8), str32);
        }
    }
}