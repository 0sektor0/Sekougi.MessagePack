namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerLong : MessagePackSerializer<long>
    {
        public override void Serialize(long value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override void SerializeUncompressed(long value, MessagePackWriter writer)
        {
            writer.Write(value, false);
        }

        public override long Deserialize(MessagePackReader reader)
        {
            return reader.ReadLong();
        }
    }
}