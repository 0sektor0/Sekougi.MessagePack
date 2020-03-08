using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerInt : MessagePackSerializer<int>
    {
        public override void Serialize(int value, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override int Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadInt(stream);
        }
    }
}