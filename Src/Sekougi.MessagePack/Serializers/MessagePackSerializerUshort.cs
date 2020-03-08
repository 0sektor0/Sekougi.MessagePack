using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerUshort : MessagePackSerializer<ushort>
    {
        public override void Serialize(ushort value, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override ushort Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadUshort(stream);
        }
    }
}