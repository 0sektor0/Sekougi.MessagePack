using System;
using System.IO;



namespace Sekougi.MessagePack.Buffers
{
    // TODO: make normal buffer
    public class MessagePackBuffer : MemoryStream, IMessagePackBuffer
    {
        public new int Length => (int) base.Length;
        
        
        public MessagePackBuffer(int capacity) : base(capacity) {}
        
        public MessagePackBuffer() {}
        
        public void Write(byte[] values) => Write(values, 0, values.Length);
        
        public void Write(byte value) => WriteByte(value);
        
        public byte Read() => (byte) base.ReadByte();
        
        public void ResetPosition() => Position = 0;

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

        public void Clear()
        {
            Position = 0;
            SetLength(0);
        }
    }
}