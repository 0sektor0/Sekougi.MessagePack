using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUshort : MessagePackSerializer<ushort>
    {
        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override ushort Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}