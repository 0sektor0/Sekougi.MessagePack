using Sekougi.MessagePack.Serializers;



namespace Sekougi.MessagePack.ConsoleTest
{
    class Program
    {
        static void Main()
        {
            var serializer = MessagePackSerializersReposetory.Get<int[]>();
        }
    }
}