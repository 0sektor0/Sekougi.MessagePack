using Sekougi.MessagePack.Exceptions;
using System;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerValueTuple3<T1, T2, T3> : MessagePackSerializer<ValueTuple<T1, T2, T3>>
    {
        private const int TUPLE_LENGTH = 3;
        private readonly MessagePackSerializer<T1> _item1Serializer;
        private readonly MessagePackSerializer<T2> _item2Serializer;
        private readonly MessagePackSerializer<T3> _item3Serializer;


        public MessagePackSerializerValueTuple3()
        {
            _item1Serializer = MessagePackSerializersRepository.Get<T1>();
            _item2Serializer = MessagePackSerializersRepository.Get<T2>();
            _item3Serializer = MessagePackSerializersRepository.Get<T3>();
        }

        public override void Serialize(ValueTuple<T1, T2, T3> value, MessagePackWriter writer)
        {
            writer.WriteArrayLength(TUPLE_LENGTH);
            _item1Serializer.Serialize(value.Item1, writer);
            _item2Serializer.Serialize(value.Item2, writer);
            _item3Serializer.Serialize(value.Item3, writer);
        }

        public override void SerializeUncompressed(ValueTuple<T1, T2, T3> value, MessagePackWriter writer)
        {
            writer.WriteArrayLength(TUPLE_LENGTH);
            _item1Serializer.SerializeUncompressed(value.Item1, writer);
            _item2Serializer.SerializeUncompressed(value.Item2, writer);
            _item3Serializer.SerializeUncompressed(value.Item3, writer);
        }

        public override ValueTuple<T1, T2, T3> Deserialize(MessagePackReader reader)
        {
            var length = reader.ReadArrayLength();
            if (length != TUPLE_LENGTH) 
                throw new MessagePackException($"Tuple length is wrong expected: {TUPLE_LENGTH} got: {length}");

            var item1 = _item1Serializer.Deserialize(reader);
            var item2 = _item2Serializer.Deserialize(reader);
            var item3 = _item3Serializer.Deserialize(reader);
            
            return (item1, item2, item3);
        }
    }
}