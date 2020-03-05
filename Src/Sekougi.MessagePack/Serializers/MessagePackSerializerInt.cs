using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerInt : MessagePackSerializer<int>
    {
        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override int Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}