using System.IO;



namespace Sekougi.MessagePack.Serializers
{
    public abstract class MessagePackSerializer<T>
    {
        public abstract void Serialize(IMessagePackBuffer buffer, T value);
        public abstract T Deserialize(Stream stream);
    }
}