namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUlong : MessagePackSerializer<ulong>
    {
        public override void Serialize(ulong value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override ulong Deserialize(MessagePackReader reader)
        {
            return reader.ReadUlong();
        }
    }
}