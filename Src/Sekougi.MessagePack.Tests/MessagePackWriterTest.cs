using Xunit;



namespace Sekougi.MessagePack.Tests
{
    public class MessagePackWriterTest
    {
        [Fact]
        public void BoolTest()
        {
            using (var buffer = new MessagePackBuffer(2))
            {
                var writer = new MessagePackWriter(buffer);
                writer.Write(false);
                writer.Write(true);

                var data = buffer.GetAll();
                Assert.Equal(2, data.Length);
                Assert.Equal(0xc2, data[0]);
                Assert.Equal(0xc3, data[1]);
            }
        }

        [Fact]
        public void FixNumTest()
        {
            var capacity = 36;
            
            using (var buffer = new MessagePackBuffer(capacity))
            {
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
            }
        }
    }
}