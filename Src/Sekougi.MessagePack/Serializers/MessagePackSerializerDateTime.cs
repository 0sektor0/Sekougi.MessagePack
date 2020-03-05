using System;
using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerDateTime : MessagePackSerializer<DateTime>
    {
        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override DateTime Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}