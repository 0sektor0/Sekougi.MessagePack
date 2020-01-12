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
            _buffer.WriteByte(MessagePackTypeCode.NIL);
        }

        public void WriteString(byte[] stringBytes)
        {
            if (stringBytes == null)
                WriteNull();
            else if (stringBytes.Length < MessagePackRange.FIX_STR_MAX_LEN)
                WriteFixString(stringBytes);
            else if (stringBytes.Length < MessagePackRange.MAX_LEN_8)
                WriteBinaryData8(MessagePackTypeCode.STR8, stringBytes);
            else if (stringBytes.Length < MessagePackRange.MAX_LEN_16)
                WriteBinaryData16(MessagePackTypeCode.STR16, stringBytes);
            else 
                WriteBinaryData32(MessagePackTypeCode.STR32, stringBytes);
        }
        
        public void WriteBinary(byte[] bytes)
        {
            if (bytes == null)
                WriteNull();
            else if (bytes.Length < MessagePackRange.MAX_LEN_8)
                WriteBinaryData8(MessagePackTypeCode.BIN8, bytes);
            else if (bytes.Length < MessagePackRange.MAX_LEN_16)
                WriteBinaryData16(MessagePackTypeCode.BIN16, bytes);
            else
                WriteBinaryData32(MessagePackTypeCode.BIN32, bytes);
        }

        public void WriteArrayHeader(int length)
        {
            WriteCollectionHeader(length, MessagePackTypeCode.FIX_ARRAY, 
                MessagePackTypeCode.ARRAY16, MessagePackTypeCode.ARRAY32);
        }

        public void WriteMapHeader(int length)
        {
            WriteCollectionHeader(length, MessagePackTypeCode.FIX_MAP, 
                MessagePackTypeCode.MAP16, MessagePackTypeCode.MAP32);
        }
        
        public void Write(bool value)
        {
            _buffer.WriteByte(value ? MessagePackTypeCode.TRUE : MessagePackTypeCode.FALSE);
        }

        public void Write(byte value)
        {
            if (value > MessagePackRange.POSITIVE_FIX_NUM_MAX)
            {
                _buffer.WriteByte(MessagePackTypeCode.UINT8);
            }
            
            _buffer.WriteByte(value);
        }

        public void Write(ushort value)
        {
            var canUseByte = value <= byte.MaxValue;
            if (canUseByte)
            {
                Write((byte)value);
                return;
            }
            
            _buffer.WriteByte(MessagePackTypeCode.UINT16);
            WriteBigEndian(value);
        }

        public void Write(uint value)
        {
            var canUseUshort = value <= ushort.MaxValue;
            if (canUseUshort)
            {
                Write((ushort)value);
                return;
            }
            
            _buffer.WriteByte(MessagePackTypeCode.UINT32);
            WriteBigEndian(value);
        }

        public void Write(ulong value)
        {
            var canUseUint = value <= uint.MaxValue;
            if (canUseUint)
            {
                Write((uint)value);
                return;
            }
            
            _buffer.WriteByte(MessagePackTypeCode.UINT64);
            WriteBigEndian(value);
        }

        public void Write(sbyte value)
        {
            var byteValue = unchecked((byte) value);
            if (value >= 0 && value <= byte.MaxValue)
            {
                Write(byteValue);
                return;
            }

            if (value < 0 && value >= MessagePackRange.NEGATIVE_FIX_NUM_MIN)
            {
                var valueWithPrefix = (byte) (byteValue + MessagePackTypeCode.NEGATIVE_FIX_NUM);
                _buffer.WriteByte(valueWithPrefix);
                return;
            }
            
            _buffer.WriteByte(MessagePackTypeCode.INT8);
            _buffer.WriteByte(byteValue);
        }
        
        public void Write(short value)
        {
            var canUseSbyte = value <= byte.MaxValue && value >= byte.MinValue;
            if (canUseSbyte)
            {
                Write((sbyte)value);
                return;
            }
            
            _buffer.WriteByte(MessagePackTypeCode.INT16);
            WriteBigEndian(value);
        }
        
        public void Write(int value)
        {
            var canUseShort = value <= short.MaxValue && value >= short.MinValue;
            if (canUseShort)
            {
                Write((short)value);
                return;
            }
            
            _buffer.WriteByte(MessagePackTypeCode.INT32);
            WriteBigEndian(value);
        }
        
        public void Write(long value)
        {
            var canUseInt = value <= int.MaxValue && value >= int.MinValue;
            if (canUseInt)
            {
                Write((int)value);
                return;
            }
            
            _buffer.WriteByte(MessagePackTypeCode.INT64);
            WriteBigEndian(value);
        }
        
        public void Write(float value)
        {
            _buffer.WriteByte(MessagePackTypeCode.FLOAT32);
            WriteBigEndian(value);
        }
        
        public void Write(double value)
        {
            _buffer.WriteByte(MessagePackTypeCode.FLOAT64);
            WriteBigEndian(value);
        }

        public void Write(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Local)
            {
                dateTime = dateTime.ToUniversalTime();
            }

            var secondsSinceBclEpoch = dateTime.Ticks / TimeSpan.TicksPerSecond;
            var seconds = secondsSinceBclEpoch - DateTimeConstants.BclSecondsAtUnixEpoch;
            var nanoseconds = dateTime.Ticks % TimeSpan.TicksPerSecond * DateTimeConstants.NanosecondsPerTick;

            if (seconds >> 34 == 0)
            {
                var data64 = unchecked((ulong)((nanoseconds << 34) | seconds));
                var isZeroNanoSeconds = (data64 & 0xffffffff00000000L) == 0;
                if (isZeroNanoSeconds)
                {
                    var data32 = (uint)data64;
                    _buffer.WriteByte(MessagePackTypeCode.TIMESTAMP32);
                    WriteBigEndian(MessagePackExtensionTypeCode.TIMESTAMP);
                    WriteBigEndian(data32);
                }
                else
                {
                    _buffer.WriteByte(MessagePackTypeCode.TIMESTAMP64);
                    WriteBigEndian(MessagePackExtensionTypeCode.TIMESTAMP);
                    WriteBigEndian(data64);
                }
            }
            else
            {
                _buffer.WriteByte(MessagePackTypeCode.TIMESTAMP96);
                WriteBigEndian(MessagePackExtensionTypeCode.TIMESTAMP96);
                WriteBigEndian(MessagePackExtensionTypeCode.TIMESTAMP);
                WriteBigEndian((uint)nanoseconds);
                WriteBigEndian(seconds);
            }
        }

        private void WriteCollectionHeader(int length, byte prefix, byte code16, byte code32)
        {
            if (length < 0)
            {
                throw new MessagePackException("collection can't be less then 0");
            }
            
            if (length < MessagePackRange.FIX_ARRAY_MAX_LEN)
            {
                var code = prefix + length;
                _buffer.WriteByte((byte) code);
            }
            else if (length < MessagePackRange.MAX_LEN_16)
            {
                _buffer.WriteByte(code16);
                WriteBigEndian((ushort) length);
            }
            else
            {
                _buffer.WriteByte(code32);
                WriteBigEndian(length);
            }
        }
        
        private void WriteFixString(byte[] stringBytes)
        {
            var prefix = MessagePackTypeCode.FIX_STR + stringBytes.Length;
            _buffer.WriteByte((byte) prefix);
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
            
            var byteSize = (uint) bytes.Length;
            WriteBigEndian(byteSize);
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