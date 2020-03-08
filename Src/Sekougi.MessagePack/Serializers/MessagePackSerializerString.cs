using System.IO;
using System.Text;


namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerString : MessagePackSerializer<string>
    {
        public override void Serialize(string value, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.Write(value, Encoding.UTF8, buffer);
        }

        public override string Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadString(stream, Encoding.UTF8);
        }
    }
}