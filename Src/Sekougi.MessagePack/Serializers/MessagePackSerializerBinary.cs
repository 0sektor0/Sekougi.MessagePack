using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerBinary : MessagePackSerializer<byte[]>
    {
        public override void Serialize(byte[] value, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.WriteBinary(value, buffer);
        }

        public override byte[] Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadBinary(stream);
        }
    }
}