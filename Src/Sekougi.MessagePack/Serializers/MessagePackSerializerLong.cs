using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerLong : MessagePackSerializer<long>
    {
        public override void Serialize(long value, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override long Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadLong(stream);
        }
    }
}