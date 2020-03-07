using System.IO;
using System.Text;


namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerString : MessagePackSerializer<string>
    {
        public override void Serialize(IMessagePackBuffer buffer, string value)
        {
            MessagePackPrimitivesWriter.Write(value, Encoding.UTF8, buffer);
        }

        public override string Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadString(stream, Encoding.UTF8);
        }
    }
}