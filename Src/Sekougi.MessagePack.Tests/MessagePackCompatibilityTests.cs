using MsgPack;
using MsgPack.Serialization;
using Sekougi.MessagePack.Serializers;
using Xunit;



namespace Sekougi.MessagePack.Tests
{
    public class MessagePackCompatibilityTests
    {
        [Fact]
        public void BoolCompatibilityTest()
        {
            var values = new[]
            {
                true,
                false,
            };
            
            CompatibilityTest(values);
        }
        
        [Fact]
        public void IntegerNumbersCompatibilityTest()
        {
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
            
            CompatibilityTest(values);
        }

        [Fact]
        public void FloatCompabilityTest()
        {
            var values = new[]
            {
                0,
                -1f,
                1f,
                -1.1f,
                1.1f,
                sbyte.MaxValue,
                sbyte.MinValue,
                short.MaxValue,
                short.MinValue,
                int.MaxValue,
                int.MinValue,
                long.MaxValue,
                long.MinValue,
            };
            
            CompatibilityTest(values);
        }

        private void CompatibilityTest<T>(T[] values)
        {
            using var buffer = new MessagePackStreamBuffer();
            using var packer = Packer.Create(buffer);
            
            var cliSerializer = MessagePackSerializer.Get<T>();
            var sekougiSerializer = MessagePackSerializersReposetory.Get<T>();
            
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