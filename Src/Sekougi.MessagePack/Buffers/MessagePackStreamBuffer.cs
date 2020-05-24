using System.IO;
using System;



namespace Sekougi.MessagePack.Buffers
{
    public class MessagePackStreamBuffer : IMessagePackBuffer, IDisposable
    {
        private Stream _stream;

        public int Length => (int) _stream.Length;
        
        
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
        
        public long Seek(long offset, SeekOrigin seekOrigin)
        {
            return _stream.Seek(offset, seekOrigin);
        }

        public void Clear()
        {
            _stream.SetLength(0);
            _stream.Position = 0;
        }
    }
}