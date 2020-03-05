using System;
using Sekougi.MessagePack.Serializers;



namespace Sekougi.MessagePack.ConsoleTest
{
    class Program
    {
        static void Main()
        {
            using var buffer = new MessagePackBuffer();
            MessagePackPrimitivesWriter.Write(-1, buffer);
            MessagePackPrimitivesWriter.Write(1, buffer);
            MessagePackPrimitivesWriter.Write(sbyte.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(sbyte.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(short.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(short.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(int.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(int.MaxValue, buffer);
            MessagePackPrimitivesWriter.Write(long.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(long.MaxValue, buffer);

            buffer.Position = 0;
            var negativeOne = MessagePackPrimitivesReader.ReadInt(buffer);
            var positiveOne = MessagePackPrimitivesReader.ReadInt(buffer);
            var sbyteValueMin = MessagePackPrimitivesReader.ReadSbyte(buffer);
            var sbyteValueMax = MessagePackPrimitivesReader.ReadSbyte(buffer);
            var shortValueMin = MessagePackPrimitivesReader.ReadInt(buffer);
            var shortValueMax = MessagePackPrimitivesReader.ReadInt(buffer);
            var intValueMin = MessagePackPrimitivesReader.ReadInt(buffer);
            var intValueMax = MessagePackPrimitivesReader.ReadInt(buffer);
            var longValueMin = MessagePackPrimitivesReader.ReadLong(buffer);
            var longValueMax = MessagePackPrimitivesReader.ReadLong(buffer);
        }
    }
}