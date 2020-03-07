using BenchmarkDotNet.Running;



namespace Sekougi.MessagePack.Benchmarks
{
    class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<SerializationBenchmark>();
        }
    }
}