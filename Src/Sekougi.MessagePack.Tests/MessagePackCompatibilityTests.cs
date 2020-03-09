using MsgPack;
using MsgPack.Serialization;
using Sekougi.MessagePack.Serializers;
using Xunit;



namespace Sekougi.MessagePack.Tests
{
    public class MessagePackCompatibilityTests
    {
        [Fact]
        public void IntegerNumbersCompatibilityTest()
        {
            using var buffer = new MessagePackStreamBuffer();
            using var packer = Packer.Create(buffer);
            
            var cliSerializer = MessagePackSerializer.Get<long>();
            var sekougiSerializer = MessagePackSerializersReposetory.Get<long>();

            var values = new[]
            {
                0,
                -1,
                32,
                sbyte.MaxValue,
                sbyte.MinValue,
                short.MaxValue,
                short.MinValue,
                int.MaxValue,
                int.MinValue,
                long.MaxValue,
                long.MinValue,
            };

            foreach (var value in values)
            {
                cliSerializer.PackTo(packer, value);
            }

            buffer.Drop();
            foreach (var value in values)
            {
                Assert.Equal(value, sekougiSerializer.Deserialize(buffer));
            }
        }
    }
}