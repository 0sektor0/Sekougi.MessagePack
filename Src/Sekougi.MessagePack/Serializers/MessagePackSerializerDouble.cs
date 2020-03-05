using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerDouble : MessagePackSerializer<double>
    {
        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override double Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}