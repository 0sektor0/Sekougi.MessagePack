namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerShort : MessagePackSerializer<short>
    {
        public override void Serialize(short value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override void SerializeUncompressed(short value, MessagePackWriter writer)
        {
            writer.Write(value, false);
        }

        public override short Deserialize(MessagePackReader reader)
        {
            return reader.ReadShort();
        }
    }
}