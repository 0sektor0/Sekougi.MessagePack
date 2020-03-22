using System.Collections.Generic;



namespace Sekougi.MessagePack.Serializers
{
    // TODO: remove array code duplicate
    public class MessagePackSerializerList<T> : MessagePackSerializer<List<T>>
    {
        private readonly MessagePackSerializer<T> _elementSerializer;


        public MessagePackSerializerList()
        {
            _elementSerializer = MessagePackSerializersRepository.Get<T>();
        }
        
        public override void Serialize(List<T> values, MessagePackWriter writer)
        {
            writer.WriteArrayHeader(values.Count);
            foreach (var value in values)
            {
                _elementSerializer.Serialize(value, writer);
            }
        }

        public override List<T> Deserialize(MessagePackReader reader)
        {
            var length = reader.ReadArrayLength();
            var list = new List<T>(length);

            for (var i = 0; i < length; i++)
            {
                list[i] = _elementSerializer.Deserialize(reader);
            }

            return list;
        }
    }
}