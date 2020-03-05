using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerShort : MessagePackSerializer<short>
    {
        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override short Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}