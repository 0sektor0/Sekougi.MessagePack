using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerArray<T> : MessagePackSerializer<T[]>
    {
        private MessagePackSerializer<T> _elementSerializer;


        public MessagePackSerializerArray()
        {
            _elementSerializer = MessagePackSerializersReposetory.Get<T>();
        }

        public override void Serialize(IMessagePackBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override T[] Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}