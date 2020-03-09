using System;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerDateTime : MessagePackSerializer<DateTime>
    {
        public override void Serialize(DateTime value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override DateTime Deserialize(MessagePackReader reader)
        {
            return reader.ReadDateTime();
        }
    }
}