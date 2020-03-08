using System;
using System.Collections.Generic;
using Sekougi.MessagePack.Serializers;



namespace Sekougi.MessagePack.ConsoleTest
{
    class Program
    {
        static void Main()
        {
            var capacity = 10;
            using var buffer = new MessagePackStreamBuffer(capacity);
            
            MessagePackPrimitivesWriter.Write(int.MinValue, buffer);
            MessagePackPrimitivesWriter.Write(int.MaxValue, buffer);
            
            var data = buffer.GetAll();
            
            var firstValue = data.Slice(0, 5);
            /*Assert.Equal(firstValue[0], 210);
            Assert.Equal(firstValue[1], 128);
            Assert.Equal(firstValue[2], 0);
            Assert.Equal(firstValue[3], 0);
            Assert.Equal(firstValue[4], 0);*/
            
            var secondValue = data.Slice(5, 5);
            /*Assert.Equal(secondValue[0], 210);
            Assert.Equal(secondValue[1], 127);
            Assert.Equal(secondValue[2], 255);
            Assert.Equal(secondValue[3], 255);
            Assert.Equal(secondValue[4], 255);*/
        }
    }
}