namespace Sekougi.MessagePack.Serializers
{
    public abstract class MessagePackSerializer<T>
    {
        public abstract void Serialize(T value, MessagePackWriter writer);

        public virtual void SerializeUncompressed(T value, MessagePackWriter writer)
        {
            Serialize(value, writer);
        }

        public void Serialize(T value, IMessagePackBuffer buffer)
        {
            var writer = new MessagePackWriter(buffer);
            Serialize(value, writer);
        }

        public void SerializeUncompressed(T value, IMessagePackBuffer buffer)
        {
            var writer = new MessagePackWriter(buffer);
            SerializeUncompressed(value, writer);
        }
        
        public abstract T Deserialize(MessagePackReader reader);

        public T Deserialize(IMessagePackBuffer buffer)
        {
            var reader = new MessagePackReader(buffer);
            return Deserialize(reader);
        }
    }
}