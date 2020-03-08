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

        public SerializationBenchmark()
        {
            _cliStream = new MemoryStream();
            _cliBuffer = Packer.Create(_cliStream);
            _sekougiBuffer = new MessagePackStreamBuffer();
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
            _sekougiBuffer.Position = 0;
            var serializer = MessagePackSerializersReposetory.Get<int>();

            for (var i = 0; i < count; i++)
            {
                serializer.Serialize(value, _sekougiBuffer);
            }
        }
    }
}