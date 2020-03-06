using System;
using System.Text;
using Sekougi.MessagePack.Serializers;



namespace Sekougi.MessagePack.ConsoleTest
{
    class Program
    {
        static void Main()
        {
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.Write("test", Encoding.UTF8, buffer);

            buffer.Position = 0;
            var str = MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8);
        }
    }
}