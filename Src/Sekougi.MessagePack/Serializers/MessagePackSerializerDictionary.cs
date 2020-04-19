using System.Collections.Generic;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerDictionary<TKey, TValue> : MessagePackSerializer<Dictionary<TKey, TValue>>
    {
        private readonly MessagePackSerializer<TKey> _keySerializer;
        private readonly MessagePackSerializer<TValue> _valueSerializer;
        
        
        public MessagePackSerializerDictionary()
        {
            _keySerializer = MessagePackSerializersRepository.Get<TKey>();
            _valueSerializer = MessagePackSerializersRepository.Get<TValue>();
        }
        
        public override void Serialize(Dictionary<TKey, TValue> dictionary, MessagePackWriter writer)
        {
            writer.WriteDictionaryLength(dictionary.Count);
            foreach (var (key, value) in dictionary)
            {
                _keySerializer.Serialize(key, writer);
                _valueSerializer.Serialize(value, writer);
            }
        }
        
        public override void SerializeUncompressed(Dictionary<TKey, TValue> dictionary, MessagePackWriter writer)
        {
            writer.WriteDictionaryLength(dictionary.Count);
            foreach (var (key, value) in dictionary)
            {
                _keySerializer.SerializeUncompressed(key, writer);
                _valueSerializer.SerializeUncompressed(value, writer);
            }
        }

        public override Dictionary<TKey, TValue> Deserialize(MessagePackReader reader)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            var length = reader.ReadDictionaryLength();
            
            for (var i = 0; i < length; i++)
            {
                var key = _keySerializer.Deserialize(reader);
                var value = _valueSerializer.Deserialize(reader);
                
                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }
}