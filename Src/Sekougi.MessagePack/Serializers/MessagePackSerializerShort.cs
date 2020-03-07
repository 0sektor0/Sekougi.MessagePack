using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerShort : MessagePackSerializer<short>
    {
        public override void Serialize(IMessagePackBuffer buffer, short value)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override short Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadShort(stream);
        }
    }
}