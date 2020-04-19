using Sekougi.MessagePack.Exceptions;
using System;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerValueTuple2<T1, T2> : MessagePackSerializer<ValueTuple<T1, T2>>
    {
        private const int TUPLE_LENGTH = 2;
        private readonly MessagePackSerializer<T1> _item1Serializer;
        private readonly MessagePackSerializer<T2> _item2Serializer;


        public MessagePackSerializerValueTuple2()
        {
            _item1Serializer = MessagePackSerializersRepository.Get<T1>();
            _item2Serializer = MessagePackSerializersRepository.Get<T2>();
        }

        public override void Serialize(ValueTuple<T1, T2> value, MessagePackWriter writer)
        {
            writer.WriteArrayLength(TUPLE_LENGTH);
            _item1Serializer.Serialize(value.Item1, writer);
            _item2Serializer.Serialize(value.Item2, writer);
        }

        public override void SerializeUncompressed(ValueTuple<T1, T2> value, MessagePackWriter writer)
        {
            writer.WriteArrayLength(TUPLE_LENGTH);
            _item1Serializer.SerializeUncompressed(value.Item1, writer);
            _item2Serializer.SerializeUncompressed(value.Item2, writer);
        }

        public override ValueTuple<T1, T2> Deserialize(MessagePackReader reader)
        {
            var length = reader.ReadArrayLength();
            if (length != TUPLE_LENGTH) 
                throw new MessagePackException($"Tuple length is wrong expected: {TUPLE_LENGTH} got: {length}");

            var item1 = _item1Serializer.Deserialize(reader);
            var item2 = _item2Serializer.Deserialize(reader);
            
            return (item1, item2);
        }
    }
}