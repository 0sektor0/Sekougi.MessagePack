using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerBinary : MessagePackSerializer<byte[]>
    {
        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}