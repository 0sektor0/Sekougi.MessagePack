using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializerArray<T> : MessagePackSerializer<T[]>
    {
        private readonly MessagePackSerializer<T> _elementSerializer;


        public MessagePackSerializerArray()
        {
            _elementSerializer = MessagePackSerializersReposetory.Get<T>();
        }

        public override void Serialize(T[] values, IMessagePackBuffer buffer)
        {
            MessagePackPrimitivesWriter.WriteArrayHeader(values.Length, buffer);
            foreach (var value in values)
            {
                _elementSerializer.Serialize(value, buffer);
            }
        }

        public override T[] Deserialize(Stream stream)
        {
            var length = MessagePackPrimitivesReader.ReadArrayLength(stream);
            var array = new T[length];

            for (var i = 0; i < length; i++)
            {
                array[i] = _elementSerializer.Deserialize(stream);
            }

            return array;
        }
    }
}