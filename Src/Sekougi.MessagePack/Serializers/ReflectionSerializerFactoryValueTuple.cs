namespace Sekougi.MessagePack.Serializers
{
    public class ReflectionSerializerFactoryValueTuple1<T1> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerValueTuple1<T1>();
        }
    }
    
    public class ReflectionSerializerFactoryValueTuple2<T1, T2> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerValueTuple2<T1, T2>();
        }
    }
    
    public class ReflectionSerializerFactoryValueTuple3<T1, T2, T3> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerValueTuple3<T1, T2, T3>();
        }
    }
    
    public class ReflectionSerializerFactoryValueTuple4<T1, T2, T3, T4> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerValueTuple4<T1, T2, T3, T4>();
        }
    }
    
    public class ReflectionSerializerFactoryValueTuple5<T1, T2, T3, T4, T5> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerValueTuple5<T1, T2, T3, T4, T5>();
        }
    }
    
    public class ReflectionSerializerFactoryValueTuple6<T1, T2, T3, T4, T5, T6> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerValueTuple6<T1, T2, T3, T4, T5, T6>();
        }
    }
    
    public class ReflectionSerializerFactoryValueTuple7<T1, T2, T3, T4, T5, T6, T7> : IReflectionSerializationFactory
    {
        public object Create()
        {
            return new MessagePackSerializerValueTuple7<T1, T2, T3, T4, T5, T6, T7>();
        }
    }
    
    public class ReflectionSerializerFactoryValueTuple8<T1, T2, T3, T4, T5, T6, T7, TRest> : IReflectionSerializationFactory 
        where TRest : struct
    {
        public object Create()
        {
            return new MessagePackSerializerValueTuple8<T1, T2, T3, T4, T5, T6, T7, TRest>();
        }
    }
}