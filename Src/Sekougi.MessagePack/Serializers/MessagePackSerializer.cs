using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public abstract class MessagePackSerializer<T>
    {
        public abstract void Serialize(T value, IMessagePackBuffer buffer);
        public abstract T Deserialize(Stream stream);
    }
}