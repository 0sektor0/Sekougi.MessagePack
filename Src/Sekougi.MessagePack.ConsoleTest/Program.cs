using System;
using System.Text;
using Sekougi.MessagePack.Serializers;



namespace Sekougi.MessagePack.ConsoleTest
{
    class Program
    {
        static void Main()
        {
            var shortStr = new string(new char[31]);
            var str8 = new string(new char[byte.MaxValue - 10]);
            var str16 = new string(new char[ushort.MaxValue - 10]);
            var str32 = new string(new char[ushort.MaxValue + 10]);
            
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.Write("", Encoding.UTF8, buffer);

            buffer.Position = 0;
            var a1 = MessagePackPrimitivesReader.ReadString(buffer, Encoding.UTF8);
        }
    }
}