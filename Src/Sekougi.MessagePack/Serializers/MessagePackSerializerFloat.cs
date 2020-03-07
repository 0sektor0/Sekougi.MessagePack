using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerFloat : MessagePackSerializer<float>
    {
        public override void Serialize(IMessagePackBuffer buffer, float value)
        {
            throw new System.NotImplementedException();
        }

        public override float Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}