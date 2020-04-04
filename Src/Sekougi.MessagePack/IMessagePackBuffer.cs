using System;
using System.IO;


namespace Sekougi.MessagePack
{
    public interface IMessagePackBuffer
    {
        int Length { get; }

        void Write(byte value);
        
        void Write(byte[] values);
        
        void Write(byte[] values, int offset, int length);
        
        int Read(byte[] destination, int offset, int length);
        
        int Read(Span<byte> destination);
        
        byte Read();
        
        void Clear();

        long Seek(long offset, SeekOrigin seekOrigin);
    }
}