using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUlong : MessagePackSerializer<ulong>
    {
        public override void Serialize(IMessagePackBuffer buffer, ulong value)
        {
            throw new System.NotImplementedException();
        }

        public override ulong Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}