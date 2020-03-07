using System;
using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerDateTime : MessagePackSerializer<DateTime>
    {
        public override void Serialize(IMessagePackBuffer buffer, DateTime value)
        {
            MessagePackPrimitivesWriter.Write(value, buffer);
        }

        public override DateTime Deserialize(Stream stream)
        {
            return MessagePackPrimitivesReader.ReadDateTime(stream);
        }
    }
}