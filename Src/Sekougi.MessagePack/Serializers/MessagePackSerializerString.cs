using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerString : MessagePackSerializer<string>
    {
        public override void Serialize(IMessagePackBuffer buffer, string value)
        {
            throw new System.NotImplementedException();
        }

        public override string Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}