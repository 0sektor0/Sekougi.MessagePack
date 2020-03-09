using System;
using System.Collections.Generic;



namespace Sekougi.MessagePack.Serializers
{
    public static class MessagePackSerializersReposetory
    {
        private static readonly Dictionary<Type, object> _serializers = new Dictionary<Type, object>();
        private static readonly object _lock = new Object();


        public static MessagePackSerializer<T> Get<T>()
        {
            var serializingType = typeof(T);
            MessagePackSerializer<T> serializer;

            lock (_lock)
            {
                serializer = Get(serializingType) as MessagePackSerializer<T>;
            }

            return serializer;
        }

        private static object Get(Type serializingType)
        {
            if (_serializers.ContainsKey(serializingType))
                return _serializers[serializingType];

            var serializer = Create(serializingType);
            _serializers.Add(serializingType, serializer);

            return serializer;
        }

        private static object Create(Type serializingType)
        {
            if (serializingType == typeof(bool))
            {
                return new MessagePackSerializerBool();
            }
            
            if (serializingType == typeof(sbyte))
            {
                return new MessagePackSerializerSbyte();
            }
            
            if (serializingType == typeof(byte))
            {
                return new MessagePackSerializerByte();
            }
            
            if (serializingType == typeof(ushort))
            {
                return new MessagePackSerializerUshort();
            }
            
            if (serializingType == typeof(short))
            {
                return new MessagePackSerializerShort();
            }
            
            if (serializingType == typeof(uint))
            {
                return new MessagePackSerializerUint();
            }
            
            if (serializingType == typeof(int))
            {
                return new MessagePackSerializerInt();
            }
            
            if (serializingType == typeof(ulong))
            {
                return new MessagePackSerializerUlong();
            }
            
            if (serializingType == typeof(long))
            {
                return new MessagePackSerializerLong();
            }
            
            if (serializingType == typeof(float))
            {
                return new MessagePackSerializerFloat();
            }
            
            if (serializingType == typeof(double))
            {
                return new MessagePackSerializerDouble();
            }
            
            if (serializingType == typeof(DateTime))
            {
                return new MessagePackSerializerDateTime();
            }
            
            if (serializingType == typeof(string))
            {
                return new MessagePackSerializerString();
            }
            
            if (serializingType == typeof(byte[]))
            {
                return new MessagePackSerializerBinary();
            }

            if (IsTypeDictionary(serializingType))
            {
                return CreateDictionarySerializer(serializingType);
            }

            if (IsTypeList(serializingType))
            {
                return CreateListSerializer(serializingType);
            }

            if (serializingType.IsArray)
            {
                return CreateArraySerializer(serializingType);
            }

            return CreateReflectionSerializer(serializingType);
        }

        private static bool IsTypeDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        private static bool IsTypeList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        private static object CreateDictionarySerializer(Type serializingType)
        {
            var keyType = serializingType.GetGenericArguments()[0];
            var valueType = serializingType.GetGenericArguments()[1];
            
            var factoryType = typeof(ReflectionSerializerFactoryDictionary<,>).MakeGenericType(keyType, valueType);
            var factory = (IReflectionSerializationFactory) Activator.CreateInstance(factoryType, Array.Empty<object>());
            
            return factory.Create();
        }

        // TODO: refactoring code duplicate
        private static object CreateListSerializer(Type serializingType)
        {
            var elementType = serializingType.GetGenericArguments()[0];
            
            var factoryType = typeof(ReflectionSerializerFactoryList<>).MakeGenericType(elementType);
            var factory = (IReflectionSerializationFactory) Activator.CreateInstance(factoryType, Array.Empty<object>());
            
            return factory.Create();
        }

        private static object CreateArraySerializer(Type serializingType)
        {
            var elementType = serializingType.GetElementType();
            
            var factoryType = typeof(ReflectionSerializerFactoryArray<>).MakeGenericType(elementType);
            var factory = (IReflectionSerializationFactory) Activator.CreateInstance(factoryType, Array.Empty<object>());
            
            return factory.Create();
        }

        private static object CreateReflectionSerializer(Type serializingType)
        {
            throw new NotImplementedException();
        }
    }
}