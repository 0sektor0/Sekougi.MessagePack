using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerSbyte : MessagePackSerializer<sbyte>
    {
        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override sbyte Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}