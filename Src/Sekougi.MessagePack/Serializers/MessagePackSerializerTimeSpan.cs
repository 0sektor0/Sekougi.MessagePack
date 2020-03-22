using System;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerTimeSpan : MessagePackSerializer<TimeSpan>
    {
        public override void Serialize(TimeSpan value, MessagePackWriter writer)
        {
            writer.Write(value);
        }

        public override TimeSpan Deserialize(MessagePackReader reader)
        {
            return reader.ReadTimeSpan();
        }
    }
}