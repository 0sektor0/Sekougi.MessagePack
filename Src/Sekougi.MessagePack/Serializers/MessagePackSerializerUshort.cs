namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUshort : MessagePackSerializer<ushort>
    {
        public override void Serialize(ushort value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override ushort Deserialize(MessagePackReader reader)
        {
            return reader.ReadUshort();
        }
    }
}