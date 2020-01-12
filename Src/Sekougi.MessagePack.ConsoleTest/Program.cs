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

            var capacity = 36;
            using (var buffer = new MessagePackBuffer(capacity))
            {
                var writer = new MessagePackWriter(buffer);

                writer.Write((byte) 0);
                writer.Write((sbyte) 0);
                writer.Write((short) 0);
                writer.Write((ushort) 0);
                writer.Write((int) 0);
                writer.Write((uint) 0);
                writer.Write((long) 0);
                writer.Write((ulong) 0);

                writer.Write((byte) 64);
                writer.Write((sbyte) 64);
                writer.Write((short) 64);
                writer.Write((ushort) 64);
                writer.Write((int) 64);
                writer.Write((uint) 64);
                writer.Write((long) 64);
                writer.Write((ulong) 64);

                writer.Write((byte) 127);
                writer.Write((sbyte) 127);
                writer.Write((short) 127);
                writer.Write((ushort) 127);
                writer.Write((int) 127);
                writer.Write((uint) 127);
                writer.Write((long) 127);
                writer.Write((ulong) 127);

                writer.Write((sbyte) -1);
                writer.Write((short) -1);
                writer.Write((int) -1);
                writer.Write((long) -1);

                writer.Write((sbyte) -16);
                writer.Write((short) -16);
                writer.Write((int) -16);
                writer.Write((long) -16);

                writer.Write((sbyte) -32);
                writer.Write((short) -32);
                writer.Write((int) -32);
                writer.Write((long) -32);

                var data = buffer.GetAll();
            }
        }
    }
}