using System;
using System.IO;



namespace Sekougi.MessagePack
{
    // TODO: make normal buffer
    public class MessagePackStreamBuffer : MemoryStream, IMessagePackBuffer
    {
        public new int Length => (int) base.Length;
        
        
        public MessagePackStreamBuffer(int capacity) : base(capacity) {}
        
        public MessagePackStreamBuffer() {}
        
        public void Write(byte[] values) => Write(values, 0, values.Length);
        
        public void Write(byte value) => WriteByte(value);
        
        public byte Read() => (byte) base.ReadByte();
        
        public void Drop() => Position = 0;

        public Span<byte> GetPart(int start, int length)
        {
            var buffer = GetBuffer();
            var span = new Span<byte>(buffer, start, length);
            
            return span;
        }

        public Span<byte> GetAll()
        {
            var buffer = GetBuffer();
            var span = new Span<byte>(buffer, 0, Length);
            return span;
        }
    }
}