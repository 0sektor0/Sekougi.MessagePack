using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUlong : MessagePackSerializer<ulong>
    {
        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override ulong Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}