using Sekougi.MessagePack.Exceptions;
using System;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerValueTuple1<T> : MessagePackSerializer<ValueTuple<T>>
    {
        private const int TUPLE_LENGTH = 1;
        private readonly MessagePackSerializer<T> _itemSerializer;


        public MessagePackSerializerValueTuple1()
        {
            _itemSerializer = MessagePackSerializersRepository.Get<T>();
        }

        public override void Serialize(ValueTuple<T> value, MessagePackWriter writer)
        {
            writer.WriteArrayLength(TUPLE_LENGTH);
            _itemSerializer.Serialize(value.Item1, writer);
        }

        public override ValueTuple<T> Deserialize(MessagePackReader reader)
        {
            var length = reader.ReadArrayLength();
            if (length != TUPLE_LENGTH) 
                throw new MessagePackException($"Tuple length is wrong expected: {TUPLE_LENGTH} got: {length}");

            var item = _itemSerializer.Deserialize(reader);
            var tuple = new ValueTuple<T>(item);
            
            return tuple;
        }
    }
}