namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerByte : MessagePackSerializer<byte>
    {
        public override void Serialize(byte value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override byte Deserialize(MessagePackReader reader)
        {
            return reader.ReadByte();
        }
    }
}