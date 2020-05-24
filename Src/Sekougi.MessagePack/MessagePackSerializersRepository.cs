using System.Runtime.CompilerServices;
using Sekougi.MessagePack.Exceptions;
using Sekougi.MessagePack.Serializers;
using System.Collections.Generic;
using System;



namespace Sekougi.MessagePack
{
    public static class MessagePackSerializersRepository
    {
        private static readonly Dictionary<Type, object> _serializers = new Dictionary<Type, object>();
        private static readonly object _lock = new Object();

        private static readonly Dictionary<Type, Type> _serializersByTypes = new Dictionary<Type, Type>
            {
                {typeof(bool), typeof(MessagePackSerializerBool)},
                {typeof(sbyte), typeof(MessagePackSerializerSbyte)},
                {typeof(byte), typeof(MessagePackSerializerByte)},
                {typeof(ushort), typeof(MessagePackSerializerUshort)},
                {typeof(short), typeof(MessagePackSerializerShort)},
                {typeof(uint), typeof(MessagePackSerializerUint)},
                {typeof(int), typeof(MessagePackSerializerInt)},
                {typeof(ulong), typeof(MessagePackSerializerUlong)},
                {typeof(long), typeof(MessagePackSerializerLong)},
                {typeof(float), typeof(MessagePackSerializerFloat)},
                {typeof(double), typeof(MessagePackSerializerDouble)},
                {typeof(DateTime), typeof(MessagePackSerializerDateTime)},
                {typeof(string), typeof(MessagePackSerializerString)},
                {typeof(byte[]), typeof(MessagePackSerializerBinary)},
                {typeof(TimeSpan), typeof(MessagePackSerializerTimeSpan)},
            };


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
            if (_serializersByTypes.TryGetValue(serializingType, out var serializerType))
            {
                return Activator.CreateInstance(serializerType);
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

            if (IsValueTuple(serializingType))
            {
                return CreateValueTupleSerializer(serializingType);
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

        private static bool IsValueTuple(Type type)
        {
            return type.IsValueType && typeof(ITuple).IsAssignableFrom(type);
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

        private static object CreateValueTupleSerializer(Type serializingType)
        {
            var genericArguments = serializingType.GetGenericArguments();
            if (genericArguments == null)
                throw new MessagePackUnsupportedSerializationType("type has no generic arguments");
            
            var length = genericArguments.Length;
            Type factoryType;
            
            switch (length)
            {
                case 1:
                    factoryType = typeof(ReflectionSerializerFactoryValueTuple1<>).MakeGenericType(genericArguments);
                    break;
                
                case 2:
                    factoryType = typeof(ReflectionSerializerFactoryValueTuple2<,>).MakeGenericType(genericArguments);
                    break;
                
                case 3:
                    factoryType = typeof(ReflectionSerializerFactoryValueTuple3<,,>).MakeGenericType(genericArguments);
                    break;
                
                case 4:
                    factoryType = typeof(ReflectionSerializerFactoryValueTuple4<,,,>).MakeGenericType(genericArguments);
                    break;
                
                case 5:
                    factoryType = typeof(ReflectionSerializerFactoryValueTuple5<,,,,>).MakeGenericType(genericArguments);
                    break;
                
                case 6:
                    factoryType = typeof(ReflectionSerializerFactoryValueTuple6<,,,,,>).MakeGenericType(genericArguments);
                    break;
                
                case 7:
                    factoryType = typeof(ReflectionSerializerFactoryValueTuple7<,,,,,,>).MakeGenericType(genericArguments);
                    break;
                
                case 8:
                    factoryType = typeof(ReflectionSerializerFactoryValueTuple8<,,,,,,,>).MakeGenericType(genericArguments);
                    break;
                
                default:
                    throw new MessagePackUnsupportedSerializationType("invalid tuple length");
            }
            
            var factory = (IReflectionSerializationFactory) Activator.CreateInstance(factoryType, Array.Empty<object>());
            return factory.Create();
        }

        private static object CreateReflectionSerializer(Type serializingType)
        {
            var serializationAttributes = serializingType.GetCustomAttributes(typeof(MessagePackSerializedAttribute), false);
            
            if (serializationAttributes == null || serializationAttributes.Length == 0) 
                throw new MessagePackUnsupportedSerializationType($"MessagePackSerializedAttribute is missing for {serializingType}");
            
            var serializationAttribute = (MessagePackSerializedAttribute) serializationAttributes[0];
            var serializer = Activator.CreateInstance(serializationAttribute.SerializedType);

            return serializer;
        }
    }
}