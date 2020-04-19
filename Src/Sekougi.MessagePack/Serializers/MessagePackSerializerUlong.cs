namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUlong : MessagePackSerializer<ulong>
    {
        public override void Serialize(ulong value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override void SerializeUncompressed(ulong value, MessagePackWriter writer)
        {
            writer.Write(value, false);
        }

        public override ulong Deserialize(MessagePackReader reader)
        {
            return reader.ReadUlong();
        }
    }
}