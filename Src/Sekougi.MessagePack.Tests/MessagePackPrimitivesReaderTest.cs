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
    }
}