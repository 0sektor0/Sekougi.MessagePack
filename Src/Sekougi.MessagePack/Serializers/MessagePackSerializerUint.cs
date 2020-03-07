using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUint : MessagePackSerializer<uint>
    {
        public override void Serialize(IMessagePackBuffer buffer, uint value)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override uint Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadUint(stream);
        }
    }
}