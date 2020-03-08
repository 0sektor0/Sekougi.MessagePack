using System.Collections.Generic;
using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    // TODO: remove array code duplicate
    public class MessagePackSerializerList<T> : MessagePackSerializer<List<T>>
    {
        private readonly MessagePackSerializer<T> _elementSerializer;


        public MessagePackSerializerList()
        {
            _elementSerializer = MessagePackSerializersReposetory.Get<T>();
        }
        
        public override void Serialize(List<T> values, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.WriteArrayHeader(values.Count, buffer);
            foreach (var value in values)
            {
                _elementSerializer.Serialize(value, buffer);
            }
        }

        public override List<T> Deserialize(Stream stream)
        {
            var length = MessagePackPrimitivesReader.ReadArrayLength(stream);
            var list = new List<T>(length);

            for (var i = 0; i < length; i++)
            {
                list[i] = _elementSerializer.Deserialize(stream);
            }

            return list;
        }
    }
}