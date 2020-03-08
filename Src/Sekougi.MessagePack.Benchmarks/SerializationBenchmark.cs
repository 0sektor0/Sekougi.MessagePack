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
        private readonly Stream _cliStream;
        private readonly Packer _cliBuffer;
        private readonly MessagePackStreamBuffer _sekougiBuffer;
        private readonly MessagePackWriter _sekougiWriter;

        public SerializationBenchmark()
        {
            _cliStream = new MemoryStream();
            _cliBuffer = Packer.Create(_cliStream);
            _sekougiBuffer = new MessagePackStreamBuffer();
            _sekougiWriter = new MessagePackWriter(_sekougiBuffer);
        }


        [Benchmark]
        [Arguments(1, int.MaxValue)]
        [Arguments(100, int.MaxValue)]
        public void SerializeIntCli(int count, int value)
        {
            _cliStream.Position = 0;
            var serializer = MessagePackSerializer.Get<int>();

            for (var i = 0; i < count; i++)
            {
                serializer.PackTo(_cliBuffer, value);
            }
        }
        
        [Benchmark]
        [Arguments(1, int.MaxValue)]
        [Arguments(100, int.MaxValue)]
        public void SerializeIntSekougi(int count, int value)
        {
            _sekougiBuffer.Drop();
            var serializer = MessagePackSerializersReposetory.Get<int>();

            for (var i = 0; i < count; i++)
            {
                serializer.Serialize(value, _sekougiWriter);
            }
        }
    }
}