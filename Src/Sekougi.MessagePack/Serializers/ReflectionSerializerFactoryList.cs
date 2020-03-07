namespace Sekougi.MessagePack.Serializers
{
    public class ReflectionSerializerFactoryList<T> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerList<T>();
        }
    }
}