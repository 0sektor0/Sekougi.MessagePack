using Sekougi.MessagePack.Exceptions;
using System;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerValueTuple5<T1, T2, T3, T4, T5> 
        : MessagePackSerializer<ValueTuple<T1, T2, T3, T4, T5>>
    {
        private const int TUPLE_LENGTH = 5;
        private readonly MessagePackSerializer<T1> _item1Serializer;
        private readonly MessagePackSerializer<T2> _item2Serializer;
        private readonly MessagePackSerializer<T3> _item3Serializer;
        private readonly MessagePackSerializer<T4> _item4Serializer;
        private readonly MessagePackSerializer<T5> _item5Serializer;


        public MessagePackSerializerValueTuple5()
        {
            _item1Serializer = MessagePackSerializersRepository.Get<T1>();
            _item2Serializer = MessagePackSerializersRepository.Get<T2>();
            _item3Serializer = MessagePackSerializersRepository.Get<T3>();
            _item4Serializer = MessagePackSerializersRepository.Get<T4>();
            _item5Serializer = MessagePackSerializersRepository.Get<T5>();
        }

        public override void Serialize(ValueTuple<T1, T2, T3, T4, T5> value, MessagePackWriter writer)
        {
            writer.WriteArrayLength(TUPLE_LENGTH);
            _item1Serializer.Serialize(value.Item1, writer);
            _item2Serializer.Serialize(value.Item2, writer);
            _item3Serializer.Serialize(value.Item3, writer);
            _item4Serializer.Serialize(value.Item4, writer);
            _item5Serializer.Serialize(value.Item5, writer);
        }

        public override void SerializeUncompressed(ValueTuple<T1, T2, T3, T4, T5> value, MessagePackWriter writer)
        {
            writer.WriteArrayLength(TUPLE_LENGTH);
            _item1Serializer.SerializeUncompressed(value.Item1, writer);
            _item2Serializer.SerializeUncompressed(value.Item2, writer);
            _item3Serializer.SerializeUncompressed(value.Item3, writer);
            _item4Serializer.SerializeUncompressed(value.Item4, writer);
            _item5Serializer.SerializeUncompressed(value.Item5, writer);
        }

        public override ValueTuple<T1, T2, T3, T4, T5> Deserialize(MessagePackReader reader)
        {
            var length = reader.ReadArrayLength();
            if (length != TUPLE_LENGTH) 
                throw new MessagePackException($"Tuple length is wrong expected: {TUPLE_LENGTH} got: {length}");

            var item1 = _item1Serializer.Deserialize(reader);
            var item2 = _item2Serializer.Deserialize(reader);
            var item3 = _item3Serializer.Deserialize(reader);
            var item4 = _item4Serializer.Deserialize(reader);
            var item5 = _item5Serializer.Deserialize(reader);
            
            return (item1, item2, item3, item4, item5);
        }
    }
}