namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUshort : MessagePackSerializer<ushort>
    {
        public override void Serialize(ushort value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override void SerializeUncompressed(ushort value, MessagePackWriter writer)
        {
            writer.Write(value, false);
        }

        public override ushort Deserialize(MessagePackReader reader)
        {
            return reader.ReadUshort();
        }
    }
}