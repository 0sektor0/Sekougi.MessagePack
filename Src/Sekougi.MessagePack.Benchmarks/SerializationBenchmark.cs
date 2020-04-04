using BenchmarkDotNet.Attributes;
using MsgPack.Serialization;
using System.IO;
using MsgPack;
using Sekougi.MessagePack.Buffers;


namespace Sekougi.MessagePack.Benchmarks
{
    [MemoryDiagnoser]
    public class SerializationBenchmark
    {
        private string _shortString;
        private string _averageString;
        private string _largeString;
        
        private Stream _cliStream;
        private Packer _packerCli;
        private MessagePackSerializer<int> _serializerIntCli;
        private MessagePackSerializer<double> _serializerDoubleCli;
        private MessagePackSerializer<float> _serializerFloatCli;
        private MessagePackSerializer<string> _serializerStringCli;

        private MessagePackBuffer _sekougiBuffer;
        private MessagePackWriter _writerSekougi;
        private Serializers.MessagePackSerializer<int> _serializerIntSekougi;
        private Serializers.MessagePackSerializer<double> _serializerDoubleSekougi;
        private Serializers.MessagePackSerializer<float> _serializerFloatSekougi;
        private Serializers.MessagePackSerializer<string> _serializerStringSekougi;

        
        [GlobalSetup]
        public void Setup()
        {
            _shortString = new string(new char[10]);
            _averageString = new string(new char[10000]);
            _largeString = new string(new char[int.MaxValue / 10]);
            
            _serializerIntCli =  MessagePackSerializer.Get<int>();
            _serializerDoubleCli = MessagePackSerializer.Get<double>();
            _serializerFloatCli = MessagePackSerializer.Get<float>();
            _serializerStringCli = MessagePackSerializer.Get<string>();
            
            _serializerIntSekougi = MessagePackSerializersRepository.Get<int>();
            _serializerDoubleSekougi = MessagePackSerializersRepository.Get<double>();
            _serializerFloatSekougi = MessagePackSerializersRepository.Get<float>();
            _serializerStringSekougi = MessagePackSerializersRepository.Get<string>();
        }
        
        [IterationSetup]
        public void IterationSetup()
        {
            const int BUFFER_CAPACITY = 4096;
            
            _cliStream = new MemoryStream(BUFFER_CAPACITY);
            _packerCli = Packer.Create(_cliStream);            

            _sekougiBuffer = new MessagePackBuffer(BUFFER_CAPACITY);
            _writerSekougi = new MessagePackWriter(_sekougiBuffer);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _cliStream.Dispose();
            _sekougiBuffer.Dispose();
        }

        [Benchmark]
        [Arguments(1, int.MaxValue)]
        [Arguments(100, int.MaxValue)]
        [Arguments(100, 255)]
        [Arguments(100, 256)]
        public void SerializeIntCli(int count, int value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerIntCli.PackTo(_packerCli, value);
            }
        }
        
        [Benchmark]
        [Arguments(1, int.MaxValue)]
        [Arguments(100, int.MaxValue)]
        [Arguments(100, 255)]
        [Arguments(100, 256)]
        public void SerializeIntSekougi(int count, int value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerIntSekougi.Serialize(value, _writerSekougi);
            }
        }
        
        [Benchmark]
        [Arguments(1, double.MaxValue)]
        [Arguments(100, double.MaxValue)]
        [Arguments(100, 255.1d)]
        public void SerializeDoubleCli(int count, double value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerDoubleCli.PackTo(_packerCli, value);
            }
        }
        
        [Benchmark]
        [Arguments(1, double.MaxValue)]
        [Arguments(100, double.MaxValue)]
        [Arguments(100, 255.1d)]
        public void SerializeDoubleSekougi(int count, double value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerDoubleSekougi.Serialize(value, _writerSekougi);
            }
        }
        
        [Benchmark]
        [Arguments(1, float.MaxValue)]
        [Arguments(100, float.MaxValue)]
        [Arguments(100, 255.1f)]
        public void SerializeFloatCli(int count, float value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerFloatCli.PackTo(_packerCli, value);
            }
        }
        
        [Benchmark]
        [Arguments(1, float.MaxValue)]
        [Arguments(100, float.MaxValue)]
        [Arguments(100, 255.1f)]
        public void SerializeFloatSekougi(int count, float value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerFloatSekougi.Serialize(value, _writerSekougi);
            }
        }
        
        [Benchmark]
        public void SerializeShortStringCli() => _serializerStringCli.PackTo(_packerCli, _shortString);
        
        [Benchmark]
        public void SerializeAverageStringCli() => _serializerStringCli.PackTo(_packerCli, _averageString);
        
        [Benchmark]
        public void SerializeLargeStringCli() => _serializerStringCli.PackTo(_packerCli, _largeString);
        
        [Benchmark]
        public void SerializeShortStringSekougi() => _serializerStringSekougi.Serialize(_shortString, _writerSekougi);
        
        [Benchmark]
        public void SerializeAverageStringSekougi() => _serializerStringSekougi.Serialize(_averageString, _writerSekougi);
        
        [Benchmark]
        public void SerializeLargeStringSekougi() => _serializerStringSekougi.Serialize(_largeString, _writerSekougi);
    }
}