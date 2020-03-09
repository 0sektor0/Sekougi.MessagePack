namespace Sekougi.MessagePack.Serializers
{
    public abstract class MessagePackSerializer<T>
    {
        public abstract void Serialize(T value, MessagePackWriter writer);

        public void Serialize(T value, IMessagePackBuffer buffer)
        {
            var writer = new MessagePackWriter(buffer);
            Serialize(value, writer);
        }
        
        public abstract T Deserialize(MessagePackReader reader);

        public T Deserialize(IMessagePackBuffer buffer)
        {
            var reader = new MessagePackReader(buffer);
            return Deserialize(reader);
        }
    }
}