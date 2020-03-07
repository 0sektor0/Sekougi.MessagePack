using System.Collections.Generic;
using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerDictionary<TKey, TValue> : MessagePackSerializer<Dictionary<TKey, TValue>>
    {
        private readonly MessagePackSerializer<TKey> _keySerializer;
        private readonly MessagePackSerializer<TValue> _valueSerializer;
        
        
        public MessagePackSerializerDictionary()
        {
            _keySerializer = MessagePackSerializersReposetory.Get<TKey>();
            _valueSerializer = MessagePackSerializersReposetory.Get<TValue>();
        }
        
        public override void Serialize(IMessagePackBuffer buffer, Dictionary<TKey, TValue> dictionary)
        {
            MessagePackPrimitivesWriter.WriteDictionaryHeader(dictionary.Count, buffer);
            foreach (var (key, value) in dictionary)
            {
                _keySerializer.Serialize(buffer, key);
                _valueSerializer.Serialize(buffer, value);
            }
        }

        public override Dictionary<TKey, TValue> Deserialize(Stream stream)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            var length = MessagePackPrimitivesReader.ReadDictionaryLength(stream);
            
            for (var i = 0; i < length; i++)
            {
                var key = _keySerializer.Deserialize(stream);
                var value = _valueSerializer.Deserialize(stream);
                
                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }
}