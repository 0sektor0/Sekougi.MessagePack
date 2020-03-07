using System;
using System.IO;



namespace Sekougi.MessagePack
{
    // TODO: make normal buffer
    // buffer prototype
    public class MessagePackBuffer : MemoryStream, IMessagePackBuffer
    {
        public MessagePackBuffer(int capacity) : base(capacity)
        {
            
        }
        
        public MessagePackBuffer()
        {
            
        }
        
        public void Write(byte[] values)
        {
            Write(values, 0, values.Length);
        }

        public Span<byte> GetNext(int partLength)
        {
            var buffer = GetBuffer();
            var start = (int) Length;
            var end = start + partLength;
            
            var span = new Span<byte>(buffer, start, end);
            return span;
        }

        public Span<byte> GetAll()
        {
            var buffer = GetBuffer();
            var span = new Span<byte>(buffer, 0, (int) Length);
            return span;
        }
    }
}