using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerSbyte : MessagePackSerializer<sbyte>
    {
        public override void Serialize(IMessagePackBuffer buffer, sbyte value)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override sbyte Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadSbyte(stream);
        }
    }
}