using System;
using Sekougi.MessagePack.Serializers;



namespace Sekougi.MessagePack.ConsoleTest
{
    class Program
    {
        static void Main()
        {
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.Write(byte.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(ushort.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(uint.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(ulong.MaxValue, buffer);

            buffer.Position = 0;
            var byteValue = MessagePackPrimitivesReader.ReadByte(buffer);
            var ushortValue = MessagePackPrimitivesReader.ReadUshort(buffer);
            var uintValue = MessagePackPrimitivesReader.ReadUint(buffer);
            var ulongValue = MessagePackPrimitivesReader.ReadUlong(buffer);
        }
    }
}