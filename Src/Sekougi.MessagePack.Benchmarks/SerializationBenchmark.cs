using System.IO;
using BenchmarkDotNet.Attributes;
using MsgPack.Serialization;
using Sekougi.MessagePack.Serializers;



namespace Sekougi.MessagePack.Benchmarks
{
    [MemoryDiagnoser]
    public class SerializationBenchmark
    {
        private readonly Stream cliBuffer = new MemoryStream();
        private readonly MessagePackBuffer sekougiBuffer = new MessagePackBuffer();


        [Benchmark]
        [Arguments(1, int.MaxValue)]
        [Arguments(100, int.MaxValue)]
        [Arguments(10000, int.MaxValue)]
        public void SerializeIntCli(int count, int value)
        {
            cliBuffer.Position = 0;
            var serializer = MessagePackSerializer.Get<int>();

            for (var i = 0; i < count; i++)
            {
                serializer.Pack(cliBuffer, value);
            }
        }
        
        [Benchmark]
        [Arguments(1, int.MaxValue)]
        [Arguments(100, int.MaxValue)]
        [Arguments(10000, int.MaxValue)]
        public void SerializeIntSekougi(int count, int value)
        {
            sekougiBuffer.Position = 0;
            var serializer = MessagePackSerializersReposetory.Get<int>();

            for (var i = 0; i < count; i++)
            {
                serializer.Serialize(sekougiBuffer, value);
            }
        }
    }
}