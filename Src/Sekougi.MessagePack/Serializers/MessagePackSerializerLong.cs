using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerLong : MessagePackSerializer<long>
    {
        public override void Serialize(IMessagePackBuffer buffer, long value)
        {
            throw new System.NotImplementedException();
        }

        public override long Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}