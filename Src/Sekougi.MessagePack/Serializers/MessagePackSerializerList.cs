using System.Collections.Generic;
using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerList<T> : MessagePackSerializer<List<T>>
    {
        private MessagePackSerializer<T> _elementSerializer;


        public MessagePackSerializerList()
        {
            _elementSerializer = MessagePackSerializersReposetory.Get<T>();
        }
        
        public override void Serialize(IMessagePackBuffer buffer, List<T> value)
        {
            throw new System.NotImplementedException();
        }

        public override List<T> Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}