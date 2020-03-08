using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUint : MessagePackSerializer<uint>
    {
        public override void Serialize(uint value, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override uint Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadUint(stream);
        }
    }
}