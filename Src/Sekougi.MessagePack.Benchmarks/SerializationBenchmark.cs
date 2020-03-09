using System.IO;
using BenchmarkDotNet.Attributes;
using MsgPack;
using MsgPack.Serialization;
using Sekougi.MessagePack.Serializers;



namespace Sekougi.MessagePack.Benchmarks
{
    [MemoryDiagnoser]
    public class SerializationBenchmark
    {
        private readonly Packer _packerCli;
        private readonly MsgPack.Serialization.MessagePackSerializer<int> _serializerIntCli;
        private readonly MsgPack.Serialization.MessagePackSerializer<double> _serializerDoubleCli;
        
        private readonly MessagePackWriter _writerSekougi;
        private readonly Serializers.MessagePackSerializer<int> _serializerIntSekougi;
        private readonly Serializers.MessagePackSerializer<double> _serializerDoubleSekougi;

        
        public SerializationBenchmark()
        {
            var cliStream = new MemoryStream();
            _packerCli = Packer.Create(cliStream);            
            _serializerIntCli =  MessagePackSerializer.Get<int>();
            _serializerDoubleCli = MessagePackSerializer.Get<double>();

            var sekougiBuffer = new MessagePackStreamBuffer();
            _writerSekougi = new MessagePackWriter(sekougiBuffer);
            _serializerIntSekougi = MessagePackSerializersReposetory.Get<int>();
            _serializerDoubleSekougi = MessagePackSerializersReposetory.Get<double>();
        }


        [Benchmark]
        [Arguments(1, int.MaxValue)]
        [Arguments(100, int.MaxValue)]
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
        public void SerializeDoubleSekougi(int count, double value)
        {
            for (var i = 0; i < count; i++)
            {
                _serializerDoubleSekougi.Serialize(value, _writerSekougi);
            }
        }
    }
}