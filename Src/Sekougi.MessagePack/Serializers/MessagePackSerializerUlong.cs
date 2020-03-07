using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUlong : MessagePackSerializer<ulong>
    {
        public override void Serialize(IMessagePackBuffer buffer, ulong value)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override ulong Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadUlong(stream);
        }
    }
}