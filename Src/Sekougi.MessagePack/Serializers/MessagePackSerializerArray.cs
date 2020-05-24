namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerArray<T> : MessagePackSerializer<T[]>
    {
        private readonly MessagePackSerializer<T> _elementSerializer;


        public MessagePackSerializerArray()
        {
            _elementSerializer = MessagePackSerializersRepository.Get<T>();
        }

        public override void Serialize(T[] values, MessagePackWriter writer)
        {
            writer.WriteArrayLength(values.Length);
            var length = values.Length;

            for (var i = 0; i < length; i++)
            {
                var value = values[i];
                _elementSerializer.Serialize(value, writer);
            }
        }
        
        public override void SerializeUncompressed(T[] values, MessagePackWriter writer)
        {
            writer.WriteArrayLength(values.Length);
            var length = values.Length;

            for (var i = 0; i < length; i++)
            {
                var value = values[i];
                _elementSerializer.SerializeUncompressed(value, writer);
            }
        }

        public override T[] Deserialize(MessagePackReader reader)
        {
            var length = reader.ReadArrayLength();
            var array = new T[length];

            for (var i = 0; i < length; i++)
            {
                array[i] = _elementSerializer.Deserialize(reader);
            }

            return array;
        }
    }
}