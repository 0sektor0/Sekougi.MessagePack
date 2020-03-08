namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerSbyte : MessagePackSerializer<sbyte>
    {
        public override void Serialize(sbyte value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override sbyte Deserialize(MessagePackReader reader)
        {
            return reader.ReadSbyte();
        }
    }
}