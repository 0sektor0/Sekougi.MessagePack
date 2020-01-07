using System;


namespace Sekougi.MessagePack.ConsoleTest
{
    class Program
    {
        static void Main()
        {
            using (var buffer = new MessagePackBuffer())
            {
                var writer = new MessagePackWriter(buffer);
                writer.WriteString(new byte[] {});

                var messagePack = buffer.GetAll();
                foreach (var byteValue in messagePack)
                {
                    var stringValue = byteValue.ToString("X").PadLeft(2, '0');
                    Console.WriteLine(stringValue);
                }
            }
        }
    }
}