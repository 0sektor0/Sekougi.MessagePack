using System.Collections.Generic;
using MsgPack;
using MsgPack.Serialization;
using Sekougi.MessagePack.Buffers;
using Xunit;



namespace Sekougi.MessagePack.Tests
{
    public class MessagePackCompatibilityTests
    {
        [Fact]
        public void BoolCompatibilityTest()
        {
            var values = new[]
            {
                true,
                false,
            };
            
            TestCompatibility(values);
        }
        
        [Fact]
        public void IntegerNumbersCompatibilityTest()
        {
            var values = new[]
            {
                0,
                -1,
                32,
                sbyte.MaxValue,
                sbyte.MinValue,
                short.MaxValue,
                short.MinValue,
                int.MaxValue,
                int.MinValue,
                long.MaxValue,
                long.MinValue,
            };
            
            TestCompatibility(values);
        }

        [Fact]
        public void FloatCompabilityTest()
        {
            var values = new[]
            {
                0,
                -1f,
                1f,
                -1.1f,
                1.1f,
                sbyte.MaxValue,
                sbyte.MinValue,
                short.MaxValue,
                short.MinValue,
                int.MaxValue,
                int.MinValue,
                long.MaxValue,
                long.MinValue,
            };
            
            TestCompatibility(values);
        }

        [Fact]
        public void DoubleCompabilityTest()
        {
            var values = new[]
            {
                0,
                -1d,
                1d,
                -1.1d,
                1.1d,
                sbyte.MaxValue,
                sbyte.MinValue,
                short.MaxValue,
                short.MinValue,
                int.MaxValue,
                int.MinValue,
                long.MaxValue,
                long.MinValue,
                double.MaxValue,
                double.MinValue,
                double.MaxValue / 2,
                double.MinValue / 2,
            };
            
            TestCompatibility(values);
        }

        [Fact]
        public void ArrayCompabilityTest()
        {
            TestArrayCompatibility<sbyte>();
            TestArrayCompatibility<short>();
            TestArrayCompatibility<ushort>();
            TestArrayCompatibility<int>();
            TestArrayCompatibility<uint>();
            TestArrayCompatibility<float>();
            TestArrayCompatibility<double>();
            TestArrayCompatibility<bool>();
        }

        [Fact]
        public void DictionaryCompabilityTest()
        {
            var values = new[]
            {
                new Dictionary<int, int>(),
                CreateTestDictionary(14),
                CreateTestDictionary(short.MaxValue),
                CreateTestDictionary(short.MaxValue + 1),
            };
            
            TestCompatibility(values);
        }
        
        private Dictionary<int, int> CreateTestDictionary(int size)
        {
            var dictionary = new Dictionary<int, int>(size);
            for (var i = 0; i < size; i++)
            {
                dictionary.Add(i, i);
            }

            return dictionary;
        }

        private void TestArrayCompatibility<T>()
        {
            var values = new[]
            {
                new T[byte.MaxValue],
                new T[short.MaxValue],
                new T[short.MaxValue + 1],
            };
            
            TestCompatibility(values);
        }

        private void TestCompatibility<T>(T[] values)
        {
            using var buffer = new MessagePackMemoryStreamBuffer();
            using var packer = Packer.Create(buffer);
            
            var cliSerializer = MessagePackSerializer.Get<T>();
            var sekougiSerializer = MessagePackSerializersRepository.Get<T>();
            
            foreach (var value in values)
            {
                cliSerializer.PackTo(packer, value);
            }

            buffer.Drop();
            foreach (var value in values)
            {
                var deserializedValue = sekougiSerializer.Deserialize(buffer);
                Assert.Equal(value, deserializedValue);
            }
        }
    }
}