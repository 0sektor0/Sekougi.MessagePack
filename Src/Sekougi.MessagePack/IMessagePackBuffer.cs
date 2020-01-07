using System;



namespace Sekougi.MessagePack
{
    public interface IMessagePackBuffer : IDisposable
    {
        void WriteByte(byte value);
        void Write(byte[] values);
        Span<byte> GetNext(int length);
        Span<byte> GetAll();
    }
}