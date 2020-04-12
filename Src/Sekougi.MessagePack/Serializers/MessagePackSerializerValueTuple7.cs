using Sekougi.MessagePack.Exceptions;
using System;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerValueTuple7<T1, T2, T3, T4, T5, T6, T7> 
        : MessagePackSerializer<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        private const int TUPLE_LENGTH = 7;
        private readonly MessagePackSerializer<T1> _item1Serializer;
        private readonly MessagePackSerializer<T2> _item2Serializer;
        private readonly MessagePackSerializer<T3> _item3Serializer;
        private readonly MessagePackSerializer<T4> _item4Serializer;
        private readonly MessagePackSerializer<T5> _item5Serializer;
        private readonly MessagePackSerializer<T6> _item6Serializer;
        private readonly MessagePackSerializer<T7> _item7Serializer;


        public MessagePackSerializerValueTuple7()
        {
            _item1Serializer = MessagePackSerializersRepository.Get<T1>();
            _item2Serializer = MessagePackSerializersRepository.Get<T2>();
            _item3Serializer = MessagePackSerializersRepository.Get<T3>();
            _item4Serializer = MessagePackSerializersRepository.Get<T4>();
            _item5Serializer = MessagePackSerializersRepository.Get<T5>();
            _item6Serializer = MessagePackSerializersRepository.Get<T6>();
            _item7Serializer = MessagePackSerializersRepository.Get<T7>();
        }

        public override void Serialize(ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, MessagePackWriter writer)
        {
            writer.WriteArrayLength(TUPLE_LENGTH);
            _item1Serializer.Serialize(value.Item1, writer);
            _item2Serializer.Serialize(value.Item2, writer);
            _item3Serializer.Serialize(value.Item3, writer);
            _item4Serializer.Serialize(value.Item4, writer);
            _item5Serializer.Serialize(value.Item5, writer);
            _item6Serializer.Serialize(value.Item6, writer);
            _item7Serializer.Serialize(value.Item7, writer);
        }

        public override ValueTuple<T1, T2, T3, T4, T5, T6, T7> Deserialize(MessagePackReader reader)
        {
            var length = reader.ReadArrayLength();
            if (length != TUPLE_LENGTH) 
                throw new MessagePackException($"Tuple length is wrong expected: {TUPLE_LENGTH} got: {length}");

            var item1 = _item1Serializer.Deserialize(reader);
            var item2 = _item2Serializer.Deserialize(reader);
            var item3 = _item3Serializer.Deserialize(reader);
            var item4 = _item4Serializer.Deserialize(reader);
            var item5 = _item5Serializer.Deserialize(reader);
            var item6 = _item6Serializer.Deserialize(reader);
            var item7 = _item7Serializer.Deserialize(reader);
            
            return (item1, item2, item3, item4, item5, item6, item7);
        }
    }
}