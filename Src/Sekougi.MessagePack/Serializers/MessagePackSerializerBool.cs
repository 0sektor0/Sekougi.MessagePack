using System.IO;

namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerBool : MessagePackSerializer<bool>
    {
        public override void Serialize(bool value, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override bool Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadBool(stream);
        }
    }
}