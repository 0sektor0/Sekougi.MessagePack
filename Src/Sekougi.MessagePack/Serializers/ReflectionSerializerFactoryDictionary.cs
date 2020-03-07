namespace Sekougi.MessagePack.Serializers
{
    public class ReflectionSerializerFactoryDictionary<TKey, TValue> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerDictionary<TKey, TValue>();
        }
    }
}