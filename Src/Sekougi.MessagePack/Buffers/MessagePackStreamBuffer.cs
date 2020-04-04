using System.IO;
using System;



namespace Sekougi.MessagePack.Buffers
{
    public class MessagePackStreamBuffer : IMessagePackBuffer
    {
        private Stream _stream;

        public int Length => (int) _stream.Length;
        public bool CanSeek => _stream.CanSeek;

        
        public MessagePackStreamBuffer(Stream stream)
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
        
        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }
        
        public void Write(byte[] values)
        {
            _stream.Write(values);
        }
        
        public void Write(byte[] values, int offset, int length)
        {
            _stream.Write(values, offset, length);
        }
        
        public int Read(byte[] destination, int offset, int length)
        {
            return _stream.Read(destination, offset, length);
        }
        
        public int Read(Span<byte> destination)
        {
            return _stream.Read(destination);
        }
        
        public byte Read()
        {
            return (byte) _stream.ReadByte();
        }
        
        public void Drop()
        {
            _stream.Position = 0;
        }
        
        public Span<byte> GetPart(int start, int length)
        {
            var part = new byte[length];
            var position = _stream.Position;
            
            _stream.Seek(start, SeekOrigin.Begin);
            _stream.Read(part);
            _stream.Seek(position, SeekOrigin.Begin);

            return part;
        }
        
        public Span<byte> GetAll()
        {
            return GetPart(0, Length);
        }

        public long Seek(long offset, SeekOrigin seekOrigin)
        {
            return _stream.Seek(offset, seekOrigin);
        }
    }
}