namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerInt : MessagePackSerializer<int>
    {
        public override void Serialize(int value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override void SerializeUncompressed(int value, MessagePackWriter writer)
        {
            writer.Write(value, false);
        }

        public override int Deserialize(MessagePackReader reader)
        {
            return reader.ReadInt();
        }
    }
}