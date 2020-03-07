using System.Collections.Generic;
using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerDictionary<TKey, TValue> : MessagePackSerializer<Dictionary<TKey, TValue>>
    {
        private MessagePackSerializer<TKey> _keySerializer;
        private MessagePackSerializer<TValue> _valueSerializer;
        
        
        public MessagePackSerializerDictionary()
        {
            _keySerializer = MessagePackSerializersReposetory.Get<TKey>();
            _valueSerializer = MessagePackSerializersReposetory.Get<TValue>();
        }
        
        public override void Serialize(IMessagePackBuffer buffer, Dictionary<TKey, TValue> value)
        {
            throw new System.NotImplementedException();
        }

        public override Dictionary<TKey, TValue> Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}