using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerFloat : MessagePackSerializer<float>
    {
        public override void Serialize(float value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override float Deserialize(MessagePackReader reader)
        {
            return reader.ReadFloat();
        }
    }
}