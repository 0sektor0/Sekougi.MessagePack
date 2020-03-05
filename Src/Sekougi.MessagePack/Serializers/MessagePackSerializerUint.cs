using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUint : MessagePackSerializer<uint>
    {
        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override uint Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}