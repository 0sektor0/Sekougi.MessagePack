using Sekougi.MessagePack.Serializers;
using Xunit;



namespace Sekougi.MessagePack.Tests
{
    [MessagePackSerialized(typeof(TestSerializer))]
    public class TestSerializable
    {
        public byte Value { get; }

        public TestSerializable(byte value)
        {
            Value = value;
        }
    }

    
    public class TestSerializer : MessagePackSerializer<TestSerializable>
    {
        public override void Serialize(TestSerializable value, MessagePackWriter writer)
        {
            writer.Write(value.Value);
        }

        public override TestSerializable Deserialize(MessagePackReader reader)
        {
            var value = reader.ReadByte();
            var testSerializable = new TestSerializable(value);
            
            return testSerializable;
        }
    }
    
    
    public class MessagePackSerializedAttributeTests
    {
        [Fact]
        public void TestCustomSerialization()
        {
            using var buffer = new MessagePackStreamBuffer();
            var writer = new MessagePackWriter(buffer);
            
            var value = new TestSerializable(0x1);
            var serializer = MessagePackSerializersRepository.Get<TestSerializable>();
            serializer.Serialize(value, writer);
            
            buffer.Drop();

            var reader = new MessagePackReader(buffer);
            var deserializedValue = serializer.Deserialize(reader);
            
            Assert.Equal(value.Value, deserializedValue.Value);
        }
    }
}