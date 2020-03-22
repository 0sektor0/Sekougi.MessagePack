using System;
using System.Text;
using Sekougi.MessagePack.Exceptions;



namespace Sekougi.MessagePack
{
    public class MessagePackWriter
    {
        private readonly IMessagePackBuffer _messagePackBuffer;
        private readonly byte[] _serializationBuffer;


        public MessagePackWriter(IMessagePackBuffer messagePackBuffer)
        {
            _messagePackBuffer = messagePackBuffer;
            _serializationBuffer = new byte[8];
        }

        // TODO: find way to avoid allocations on string to byte[] cast
        public void Write(string str, Encoding encoding)
        {
            var bytes = str != null ? encoding.GetBytes(str) : null;
            WriteString(bytes);
        }
        
        public void WriteBinary(byte[] bytes)
        {
            if (bytes == null)
                WriteNull();
            else if (bytes.Length < byte.MaxValue)
                WriteBinaryData8(MessagePackTypeCode.BIN8, bytes);
            else if (bytes.Length < ushort.MaxValue)
                WriteBinaryData16(MessagePackTypeCode.BIN16, bytes);
            else
                WriteBinaryData32(MessagePackTypeCode.BIN32, bytes);
        }

        public void WriteArrayHeader(int length)
        {
            WriteCollectionHeader(length, MessagePackTypeCode.FIX_ARRAY, 
                MessagePackTypeCode.ARRAY16, MessagePackTypeCode.ARRAY32);
        }

        public void WriteDictionaryHeader(int length)
        {
            WriteCollectionHeader(length, MessagePackTypeCode.FIX_MAP, 
                MessagePackTypeCode.MAP16, MessagePackTypeCode.MAP32);
        }
        
        public void Write(bool value)
        {
            _messagePackBuffer.Write(value ? MessagePackTypeCode.TRUE : MessagePackTypeCode.FALSE);
        }

        public void Write(byte value)
        {
            if (value > MessagePackRange.POSITIVE_FIX_INT_MAX)
            {
                _messagePackBuffer.Write(MessagePackTypeCode.UINT8);
            }
            
            _messagePackBuffer.Write(value);
        }

        public void Write(sbyte value)
        {
            var byteValue = unchecked((byte) value);
            if (value >= 0)
            {
                Write(byteValue);
                return;
            }

            var isNegativeFixNum = value < 0 && value >= MessagePackRange.NEGATIVE_FIX_INT_MIN;
            if (isNegativeFixNum)
            {
                _messagePackBuffer.Write(byteValue);
                return;
            }
            
            _messagePackBuffer.Write(MessagePackTypeCode.INT8);
            _messagePackBuffer.Write(byteValue);
        }
        
        public void Write(short value)
        {
            var canUseSbyte = value <= sbyte.MaxValue && value >= sbyte.MinValue;
            if (canUseSbyte)
            {
                Write((sbyte)value);
                return;
            }
            
            _messagePackBuffer.Write(MessagePackTypeCode.INT16);
            WriteBigEndian(value);
        }
        
        public void Write(ushort value)
        {
            var canUseByte = value <= byte.MaxValue;
            if (canUseByte)
            {
                Write((byte)value);
                return;
            }
            
            _messagePackBuffer.Write(MessagePackTypeCode.UINT16);
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

            _messagePackBuffer.Write(MessagePackTypeCode.UINT32);
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
            
            _messagePackBuffer.Write(MessagePackTypeCode.UINT64);
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
            
            _messagePackBuffer.Write(MessagePackTypeCode.INT32);
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
            
            _messagePackBuffer.Write(MessagePackTypeCode.INT64);
            WriteBigEndian(value);
        }
        
        public void Write(float value)
        {
            _messagePackBuffer.Write(MessagePackTypeCode.FLOAT32);
            WriteBigEndian(value);
        }
        
        public void Write(double value)
        {
            _messagePackBuffer.Write(MessagePackTypeCode.FLOAT64);
            WriteBigEndian(value);
        }

        public void Write(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Local)
            {
                dateTime = dateTime.ToUniversalTime();
            }
            
            WriteTimeTicks(dateTime.Ticks);
        }

        public void Write(TimeSpan timeSpan)
        {
            WriteTimeTicks(timeSpan.Ticks);
        }

        private void WriteTimeTicks(long ticks)
        {

            var secondsSinceBclEpoch = ticks / TimeSpan.TicksPerSecond;
            var seconds = secondsSinceBclEpoch - DateTimeConstants.BclSecondsAtUnixEpoch;
            var nanoseconds = ticks % TimeSpan.TicksPerSecond * DateTimeConstants.NanosecondsPerTick;

            if (seconds >> 34 == 0)
            {
                var data64 = unchecked((ulong)((nanoseconds << 34) | seconds));
                
                if (nanoseconds == 0)
                {
                    var data32 = (uint)data64;
                    _messagePackBuffer.Write(MessagePackTypeCode.TIMESTAMP32);
                    _messagePackBuffer.Write(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP));
                    WriteBigEndian(data32);
                }
                else
                {
                    _messagePackBuffer.Write(MessagePackTypeCode.TIMESTAMP64);
                    _messagePackBuffer.Write(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP));
                    WriteBigEndian(data64);
                }
            }
            else
            {
                _messagePackBuffer.Write(MessagePackTypeCode.TIMESTAMP96);
                _messagePackBuffer.Write(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP96));
                _messagePackBuffer.Write(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP));
                WriteBigEndian((uint)nanoseconds);
                WriteBigEndian(seconds);
            }
        }
        
        private void WriteNull()
        {
            _messagePackBuffer.Write(MessagePackTypeCode.NIL);
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
                _messagePackBuffer.Write((byte) code);
            }
            else if (length < ushort.MaxValue)
            {
                _messagePackBuffer.Write(code16);
                WriteBigEndian((ushort) length);
            }
            else
            {
                _messagePackBuffer.Write(code32);
                WriteBigEndian(length);
            }
        }

        private void WriteString(byte[] stringBytes)
        {
            if (stringBytes == null)
                WriteNull();
            else if (stringBytes.Length < MessagePackRange.FIX_STR_MAX_LEN)
                WriteFixString(stringBytes);
            else if (stringBytes.Length < byte.MaxValue)
                WriteBinaryData8(MessagePackTypeCode.STR8, stringBytes);
            else if (stringBytes.Length < ushort.MaxValue)
                WriteBinaryData16(MessagePackTypeCode.STR16, stringBytes);
            else
                WriteBinaryData32(MessagePackTypeCode.STR32, stringBytes);
        }
        
        private void WriteFixString(byte[] stringBytes)
        {
            var prefix = MessagePackTypeCode.FIX_STRING + stringBytes.Length;
            _messagePackBuffer.Write((byte) prefix);
            _messagePackBuffer.Write(stringBytes);
        }
        
        private void WriteBinaryData8(byte typeCode, byte[] bytes)
        {
            _messagePackBuffer.Write(typeCode);
            
            var byteSize = (byte) bytes.Length;
            _messagePackBuffer.Write(byteSize);
            _messagePackBuffer.Write(bytes);
        }
        
        private void WriteBinaryData16(byte typeCode, byte[] bytes)
        {
            _messagePackBuffer.Write(typeCode);
            
            var byteSize = (ushort) bytes.Length;
            WriteBigEndian(byteSize);
            _messagePackBuffer.Write(bytes);
        }
        
        private void WriteBinaryData32(byte typeCode, byte[] bytes)
        {
            _messagePackBuffer.Write(typeCode);
            
            var byteSize = bytes.Length;
            WriteBigEndian(byteSize);
            _messagePackBuffer.Write(bytes);
        }
        
        private void WriteBigEndian(short value) => 
            WriteBigEndian(unchecked((ushort) value));
        
        private void WriteBigEndian(int value) => 
            WriteBigEndian(unchecked((uint) value));
        
        private void WriteBigEndian(long value) => 
            WriteBigEndian(unchecked((ulong) value));

        private unsafe void WriteBigEndian(float value) =>
            WriteBigEndian(*(int*) &value);

        private unsafe void WriteBigEndian(double value) =>
            WriteBigEndian(*(long*) &value);
        
        private void WriteBigEndian(ushort value)
        {
            unchecked
            {
                _serializationBuffer[1] = (byte) value;
                _serializationBuffer[0] = (byte) (value >> 8);
                
                _messagePackBuffer.Write(_serializationBuffer, 0, 2);
            }
        }
        
        private void WriteBigEndian(uint value)
        {
            unchecked
            {
                _serializationBuffer[3] = (byte) value;
                _serializationBuffer[2] = (byte) (value >> 8);
                _serializationBuffer[1] = (byte) (value >> 16);
                _serializationBuffer[0] = (byte) (value >> 24);
                
                _messagePackBuffer.Write(_serializationBuffer, 0, 4);
            }
        }

        private void WriteBigEndian(ulong value)
        {
            unchecked
            {
                _serializationBuffer[7] = (byte) value;
                _serializationBuffer[6] = (byte) (value >> 8);
                _serializationBuffer[5] = (byte) (value >> 16);
                _serializationBuffer[4] = (byte) (value >> 24);
                _serializationBuffer[3] = (byte) (value >> 32);
                _serializationBuffer[2] = (byte) (value >> 40);
                _serializationBuffer[1] = (byte) (value >> 48);
                _serializationBuffer[0] = (byte) (value >> 56);
                
                _messagePackBuffer.Write(_serializationBuffer, 0, 8);
            }
        }
    }
}