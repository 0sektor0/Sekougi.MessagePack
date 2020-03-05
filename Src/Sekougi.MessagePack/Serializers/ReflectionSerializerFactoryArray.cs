namespace Sekougi.MessagePack.Serializers
{
    public class ReflectionSerializerFactoryArray<T> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerArray<T>();
        }
    }
}