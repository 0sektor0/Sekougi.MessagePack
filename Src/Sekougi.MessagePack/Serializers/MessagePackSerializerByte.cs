using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerByte : MessagePackSerializer<byte>
    {
        public override void Serialize(IMessagePackBuffer buffer, byte value)
        {
            throw new System.NotImplementedException();
        }

        public override byte Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}