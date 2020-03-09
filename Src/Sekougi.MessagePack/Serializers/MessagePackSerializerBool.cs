namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerBool : MessagePackSerializer<bool>
    {
        public override void Serialize(bool value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override bool Deserialize(MessagePackReader reader)
        {
            return reader.ReadBool();
        }
    }
}