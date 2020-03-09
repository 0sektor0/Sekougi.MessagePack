using System.Text;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerString : MessagePackSerializer<string>
    {
        public override void Serialize(string value, MessagePackWriter writer)
        {
            writer.Write(value, Encoding.UTF8);
        }

        public override string Deserialize(MessagePackReader reader)
        {
            return reader.ReadString(Encoding.UTF8);
        }
    }
}