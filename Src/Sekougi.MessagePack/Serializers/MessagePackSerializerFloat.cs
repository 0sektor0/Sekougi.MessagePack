using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerFloat : MessagePackSerializer<float>
    {
        public override void Serialize(IMessagePackBuffer buffer, float value)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override float Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadFloat(stream);
        }
    }
}