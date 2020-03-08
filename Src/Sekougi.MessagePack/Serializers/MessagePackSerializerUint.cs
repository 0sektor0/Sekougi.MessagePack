namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUint : MessagePackSerializer<uint>
    {
        public override void Serialize(uint value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override uint Deserialize(MessagePackReader reader)
        {
            return reader.ReadUint();
        }
    }
}