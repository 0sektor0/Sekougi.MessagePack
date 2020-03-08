using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerDouble : MessagePackSerializer<double>
    {
        public override void Serialize(double value, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override double Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadDouble(stream);
        }
    }
}