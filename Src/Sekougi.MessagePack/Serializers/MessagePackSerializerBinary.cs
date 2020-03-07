using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerBinary : MessagePackSerializer<byte[]>
    {
        public override void Serialize(IMessagePackBuffer buffer, byte[] value)
        {
            MessagePackPrimitivesWriter.WriteBinary(value, buffer);
        }

        public override byte[] Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadBinary(stream);
        }
    }
}