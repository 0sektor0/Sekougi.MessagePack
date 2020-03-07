using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerLong : MessagePackSerializer<long>
    {
        public override void Serialize(IMessagePackBuffer buffer, long value)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override long Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadLong(stream);
        }
    }
}