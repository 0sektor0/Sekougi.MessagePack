using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerByte : MessagePackSerializer<byte>
    {
        public override void Serialize(IMessagePackBuffer buffer, byte value)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override byte Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadByte(stream);
        }
    }
}