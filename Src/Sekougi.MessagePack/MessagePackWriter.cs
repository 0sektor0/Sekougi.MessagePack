using System;



namespace Sekougi.MessagePack
{
    public readonly struct MessagePackWriter
    {
        private readonly IMessagePackBuffer _buffer;
        
        
        public MessagePackWriter(IMessagePackBuffer buffer)
        {
            _buffer = buffer;
        }

        public void WriteNull()
        {
            _buffer.WriteByte(MessagePackConstants.NIL_CODE);
        }

        public void WriteString(byte[] stringBytes)
        {
            if (stringBytes == null)
                WriteNull();
            else if (stringBytes.Length < MessagePackConstants.FIX_STR_MAX_LEN)
                WriteFixString(stringBytes);
            else if (stringBytes.Length < MessagePackConstants.ARRAY8_MAX_LEN)
                WriteBinaryData8(MessagePackConstants.STR8_CODE, stringBytes);
            else if (stringBytes.Length < MessagePackConstants.ARRAY16_MAX_LEN)
                WriteBinaryData16(MessagePackConstants.STR16_CODE, stringBytes);
            else if (stringBytes.Length < MessagePackConstants.ARRAY32_MAX_LEN)
                WriteBinaryData32(MessagePackConstants.STR32_CODE, stringBytes);
            else
                throw new MessagePackException("raw string to big");
        }
        
        public void WriteBinary(byte[] bytes)
        {
            if (bytes == null)
                WriteNull();
            if (bytes.Length < MessagePackConstants.ARRAY8_MAX_LEN)
                WriteBinaryData8(MessagePackConstants.BIN8_CODE, bytes);
            if (bytes.Length < MessagePackConstants.ARRAY16_MAX_LEN)
                WriteBinaryData16(MessagePackConstants.BIN16_CODE, bytes);
            if (bytes.Length < MessagePackConstants.ARRAY32_MAX_LEN)
                WriteBinaryData32(MessagePackConstants.BIN32_CODE, bytes);
            else
                throw new MessagePackException("raw binary to big");
        }

        public void WriteArrayHeader(int length)
        {
            throw new NotImplementedException();
            
            if (length < 1)
                throw new MessagePackException("array length less then 1");
        }
        
        public void Write(bool value)
        {
            _buffer.WriteByte(value ? MessagePackConstants.TRUE_CODE : MessagePackConstants.FALSE_CODE);
        }

        public void Write(sbyte value)
        {
            var byteValue = unchecked((byte) value);
            
            if (value < MessagePackConstants.NEGATIVE_FIX_NUM_MIN)
                _buffer.WriteByte(MessagePackConstants.INT8_CODE);
            
            _buffer.WriteByte(byteValue);
        }

        public void Write(byte value)
        {
            if (value > MessagePackConstants.POSITIVE_FIX_NUM_MAX)
            {
                _buffer.WriteByte(MessagePackConstants.UINT8_CODE);
            }
            
            _buffer.WriteByte(value);
        }

        public void Write(ushort value)
        {
            _buffer.WriteByte(MessagePackConstants.UINT16_CODE);
            WriteBigEndian(value);
        }

        public void Write(uint value)
        {
            _buffer.WriteByte(MessagePackConstants.UINT32_CODE);
            WriteBigEndian(value);
        }

        public void Write(ulong value)
        {
            _buffer.WriteByte(MessagePackConstants.UINT64_CODE);
            WriteBigEndian(value);
        }

        public void Write(short value)
        {
            _buffer.WriteByte(MessagePackConstants.INT16_CODE);
            WriteBigEndian(value);
        }
        
        public void Write(int value)
        {
            _buffer.WriteByte(MessagePackConstants.INT32_CODE);
            WriteBigEndian(value);
        }
        
        public void Write(long value)
        {
            _buffer.WriteByte(MessagePackConstants.INT64_CODE);
            WriteBigEndian(value);
        }
        
        public void Write(float value)
        {
            _buffer.WriteByte(MessagePackConstants.FLOAT32_CODE);
            WriteBigEndian(value);
        }
        
        public void Write(double value)
        {
            _buffer.WriteByte(MessagePackConstants.FLOAT64_CODE);
            WriteBigEndian(value);
        }
        
        private void WriteFixString(byte[] stringBytes)
        {
            var byteSize = (byte) stringBytes.Length;
            _buffer.WriteByte(byteSize);
            _buffer.Write(stringBytes);
        }
        
        private void WriteBinaryData8(byte typeCode, byte[] bytes)
        {
            _buffer.WriteByte(typeCode);
            
            var byteSize = (byte) bytes.Length;
            _buffer.WriteByte(byteSize);
            _buffer.Write(bytes);
        }
        
        private void WriteBinaryData16(byte typeCode, byte[] bytes)
        {
            _buffer.WriteByte(typeCode);
            
            var byteSize = (ushort) bytes.Length;
            WriteBigEndian(byteSize);
            _buffer.Write(bytes);
        }
        
        private void WriteBinaryData32(byte typeCode, byte[] bytes)
        {
            _buffer.WriteByte(typeCode);
            
            WriteBigEndian(bytes.Length);
            _buffer.Write(bytes);
        }
        
        private void WriteBigEndian(ushort value)
        {
            unchecked
            {
                var bigEndian1 = (byte) value;
                _buffer.WriteByte(bigEndian1);
                
                var bigEndian2 = (byte) (value >> 8);
                _buffer.WriteByte(bigEndian2);
            }
        }

        private void WriteBigEndian(short value) => 
            WriteBigEndian(unchecked((ushort) value));

        private void WriteBigEndian(uint value)
        {
            unchecked
            {
                var bigEndian1 = (byte) value;
                _buffer.WriteByte(bigEndian1);
                
                var bigEndian2 = (byte) (value >> 8);
                _buffer.WriteByte(bigEndian2);
                
                var bigEndian3 = (byte) (value >> 16);
                _buffer.WriteByte(bigEndian3);
                
                var bigEndian4 = (byte) (value >> 24);
                _buffer.WriteByte(bigEndian4);
            }
        }
        
        private void WriteBigEndian(int value) => 
            WriteBigEndian(unchecked((uint) value));

        private void WriteBigEndian(ulong value)
        {
            unchecked
            {
                var bigEndian1 = (byte) value;
                _buffer.WriteByte(bigEndian1);
                
                var bigEndian2 = (byte) (value >> 8);
                _buffer.WriteByte(bigEndian2);
                
                var bigEndian3 = (byte) (value >> 16);
                _buffer.WriteByte(bigEndian3);
                
                var bigEndian4 = (byte) (value >> 24);
                _buffer.WriteByte(bigEndian4);
                
                var bigEndian5 = (byte) (value >> 32);
                _buffer.WriteByte(bigEndian5);
                
                var bigEndian6 = (byte) (value >> 40);
                _buffer.WriteByte(bigEndian6);
                
                var bigEndian7 = (byte) (value >> 48);
                _buffer.WriteByte(bigEndian7);
                
                var bigEndian8 = (byte) (value >> 56);
                _buffer.WriteByte(bigEndian8);
            }
        }
        
        private void WriteBigEndian(long value) => 
            WriteBigEndian(unchecked((ulong) value));

        private unsafe void WriteBigEndian(float value) =>
            WriteBigEndian(*(int*) &value);

        private unsafe void WriteBigEndian(double value) =>
            WriteBigEndian(*(long*) &value);
    }
}