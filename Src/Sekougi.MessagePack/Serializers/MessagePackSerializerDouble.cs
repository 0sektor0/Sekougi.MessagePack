namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerDouble : MessagePackSerializer<double>
    {
        public override void Serialize(double value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override double Deserialize(MessagePackReader reader)
        {
            return reader.ReadDouble();
        }
    }
}