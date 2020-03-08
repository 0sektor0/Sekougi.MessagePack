namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerBinary : MessagePackSerializer<byte[]>
    {
        public override void Serialize(byte[] value, MessagePackWriter writer)
        {
            writer.WriteBinary(value);
        }

        public override byte[] Deserialize(MessagePackReader reader)
        {
            return reader.ReadBinary();
        }
    }
}