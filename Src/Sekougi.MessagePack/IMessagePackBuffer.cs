using System;



namespace Sekougi.MessagePack
{
    public interface IMessagePackBuffer : IDisposable
    {
        void Write(byte value);
        
        void Write(byte[] values);
        
        void Write(byte[] values, int offset, int length);
        
        int Read(byte[] destination, int offset, int length);
        
        int Read(Span<byte> destination);
        
        byte Read();
        
        void Drop();
        
        Span<byte> GetPart(int start, int length);
        
        Span<byte> GetAll();
        
        int Length { get; }
    }
}