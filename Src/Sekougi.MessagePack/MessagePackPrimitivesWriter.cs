using System;
using System.Text;
using Sekougi.MessagePack.Exceptions;



namespace Sekougi.MessagePack
{
    public static class MessagePackPrimitivesWriter
    {
        public static void WriteNull(IMessagePackBuffer buffer)
        {
            buffer.Write(MessagePackTypeCode.NIL);
        }

        // TODO: find way to avoid allocations on string to byte[] cast
        public static void Write(string str, Encoding encoding, IMessagePackBuffer buffer)
        {
            var bytes = str != null ? encoding.GetBytes(str) : null;
            WriteString(bytes, buffer);
        }
        
        public static void WriteBinary(byte[] bytes, IMessagePackBuffer buffer)
        {
            if (bytes == null)
                WriteNull(buffer);
            else if (bytes.Length < byte.MaxValue)
                WriteBinaryData8(MessagePackTypeCode.BIN8, bytes, buffer);
            else if (bytes.Length < ushort.MaxValue)
                WriteBinaryData16(MessagePackTypeCode.BIN16, bytes, buffer);
            else
                WriteBinaryData32(MessagePackTypeCode.BIN32, bytes, buffer);
        }

        public static void WriteArrayHeader(int length, IMessagePackBuffer buffer)
        {
            WriteCollectionHeader(length, MessagePackTypeCode.FIX_ARRAY, 
                MessagePackTypeCode.ARRAY16, MessagePackTypeCode.ARRAY32, buffer);
        }

        public static void WriteDictionaryHeader(int length, IMessagePackBuffer buffer)
        {
            WriteCollectionHeader(length, MessagePackTypeCode.FIX_MAP, 
                MessagePackTypeCode.MAP16, MessagePackTypeCode.MAP32, buffer);
        }
        
        public static void Write(bool value, IMessagePackBuffer buffer)
        {
            buffer.Write(value ? MessagePackTypeCode.TRUE : MessagePackTypeCode.FALSE);
        }

        public static void Write(byte value, IMessagePackBuffer buffer)
        {
            if (value > MessagePackRange.POSITIVE_FIX_INT_MAX)
            {
                buffer.Write(MessagePackTypeCode.UINT8);
            }
            
            buffer.Write(value);
        }

        public static void Write(sbyte value, IMessagePackBuffer buffer)
        {
            var byteValue = unchecked((byte) value);
            if (value >= 0)
            {
                Write(byteValue, buffer);
                return;
            }

            var isNegativeFixNum = value < 0 && value >= MessagePackRange.NEGATIVE_FIX_INT_MIN;
            if (isNegativeFixNum)
            {
                buffer.Write(byteValue);
                return;
            }
            
            buffer.Write(MessagePackTypeCode.INT8);
            buffer.Write(byteValue);
        }
        
        public static void Write(short value, IMessagePackBuffer buffer)
        {
            var canUseSbyte = value <= sbyte.MaxValue && value >= sbyte.MinValue;
            if (canUseSbyte)
            {
                Write((sbyte)value, buffer);
                return;
            }
            
            buffer.Write(MessagePackTypeCode.INT16);
            WriteBigEndian(value, buffer);
        }
        
        public static void Write(ushort value, IMessagePackBuffer buffer)
        {
            var canUseByte = value <= byte.MaxValue;
            if (canUseByte)
            {
                Write((byte)value, buffer);
                return;
            }
            
            buffer.Write(MessagePackTypeCode.UINT16);
            WriteBigEndian(value, buffer);
        }

        public static void Write(uint value, IMessagePackBuffer buffer)
        {
            var canUseUshort = value <= ushort.MaxValue;
            if (canUseUshort)
            {
                Write((ushort)value, buffer);
                return;
            }

            buffer.Write(MessagePackTypeCode.UINT32);
            WriteBigEndian(value, buffer);
        }

        public static void Write(ulong value, IMessagePackBuffer buffer)
        {
            var canUseUint = value <= uint.MaxValue;
            if (canUseUint)
            {
                Write((uint)value, buffer);
                return;
            }
            
            buffer.Write(MessagePackTypeCode.UINT64);
            WriteBigEndian(value, buffer);
        }
        
        public static void Write(int value, IMessagePackBuffer buffer)
        {
            var canUseShort = value <= short.MaxValue && value >= short.MinValue;
            if (canUseShort)
            {
                Write((short)value, buffer);
                return;
            }
            
            buffer.Write(MessagePackTypeCode.INT32);
            WriteBigEndian(value, buffer);
        }
        
        public static void Write(long value, IMessagePackBuffer buffer)
        {
            var canUseInt = value <= int.MaxValue && value >= int.MinValue;
            if (canUseInt)
            {
                Write((int)value, buffer);
                return;
            }
            
            buffer.Write(MessagePackTypeCode.INT64);
            WriteBigEndian(value, buffer);
        }
        
        public static void Write(float value, IMessagePackBuffer buffer)
        {
            buffer.Write(MessagePackTypeCode.FLOAT32);
            WriteBigEndian(value, buffer);
        }
        
        public static void Write(double value, IMessagePackBuffer buffer)
        {
            buffer.Write(MessagePackTypeCode.FLOAT64);
            WriteBigEndian(value, buffer);
        }

        public static void Write(DateTime dateTime, IMessagePackBuffer buffer)
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
                
                if (nanoseconds == 0)
                {
                    var data32 = (uint)data64;
                    buffer.Write(MessagePackTypeCode.TIMESTAMP32);
                    buffer.Write(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP));
                    WriteBigEndian(data32, buffer);
                }
                else
                {
                    buffer.Write(MessagePackTypeCode.TIMESTAMP64);
                    buffer.Write(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP));
                    WriteBigEndian(data64, buffer);
                }
            }
            else
            {
                buffer.Write(MessagePackTypeCode.TIMESTAMP96);
                buffer.Write(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP96));
                buffer.Write(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP));
                WriteBigEndian((uint)nanoseconds, buffer);
                WriteBigEndian(seconds, buffer);
            }
        }

        private static void WriteCollectionHeader(int length, byte prefix, byte code16, byte code32, IMessagePackBuffer buffer)
        {
            if (length < 0)
            {
                throw new MessagePackException("collection can't be less then 0");
            }
            
            if (length < MessagePackRange.FIX_ARRAY_MAX_LEN)
            {
                var code = prefix + length;
                buffer.Write((byte) code);
            }
            else if (length < ushort.MaxValue)
            {
                buffer.Write(code16);
                WriteBigEndian((ushort) length, buffer);
            }
            else
            {
                buffer.Write(code32);
                WriteBigEndian(length, buffer);
            }
        }

        private static void WriteString(byte[] stringBytes, IMessagePackBuffer buffer)
        {
            if (stringBytes == null)
                WriteNull(buffer);
            else if (stringBytes.Length < MessagePackRange.FIX_STR_MAX_LEN)
                WriteFixString(stringBytes, buffer);
            else if (stringBytes.Length < byte.MaxValue)
                WriteBinaryData8(MessagePackTypeCode.STR8, stringBytes, buffer);
            else if (stringBytes.Length < ushort.MaxValue)
                WriteBinaryData16(MessagePackTypeCode.STR16, stringBytes, buffer);
            else
                WriteBinaryData32(MessagePackTypeCode.STR32, stringBytes, buffer);
        }
        
        private static void WriteFixString(byte[] stringBytes, IMessagePackBuffer buffer)
        {
            var prefix = MessagePackTypeCode.FIX_STRING + stringBytes.Length;
            buffer.Write((byte) prefix);
            buffer.Write(stringBytes);
        }
        
        private static void WriteBinaryData8(byte typeCode, byte[] bytes, IMessagePackBuffer buffer)
        {
            buffer.Write(typeCode);
            
            var byteSize = (byte) bytes.Length;
            buffer.Write(byteSize);
            buffer.Write(bytes);
        }
        
        private static void WriteBinaryData16(byte typeCode, byte[] bytes, IMessagePackBuffer buffer)
        {
            buffer.Write(typeCode);
            
            var byteSize = (ushort) bytes.Length;
            WriteBigEndian(byteSize, buffer);
            buffer.Write(bytes);
        }
        
        private static void WriteBinaryData32(byte typeCode, byte[] bytes, IMessagePackBuffer buffer)
        {
            buffer.Write(typeCode);
            
            var byteSize = bytes.Length;
            WriteBigEndian(byteSize, buffer);
            buffer.Write(bytes);
        }
        
        private static void WriteBigEndian(short value, IMessagePackBuffer buffer) => 
            WriteBigEndian(unchecked((ushort) value), buffer);
        
        private static void WriteBigEndian(int value, IMessagePackBuffer buffer) => 
            WriteBigEndian(unchecked((uint) value), buffer);
        
        private static void WriteBigEndian(long value, IMessagePackBuffer buffer) => 
            WriteBigEndian(unchecked((ulong) value), buffer);

        private static unsafe void WriteBigEndian(float value, IMessagePackBuffer buffer) =>
            WriteBigEndian(*(int*) &value, buffer);

        private static unsafe void WriteBigEndian(double value, IMessagePackBuffer buffer) =>
            WriteBigEndian(*(long*) &value, buffer);
        
        private static void WriteBigEndian(ushort value, IMessagePackBuffer buffer)
        {
            unchecked
            {
                var bigEndian0 = (byte) value;
                var bigEndian1 = (byte) (value >> 8);
                
                buffer.Write(bigEndian1);
                buffer.Write(bigEndian0);
            }
        }
        
        // temporary optimization
        // TODO: make writer non static
        private static readonly byte[] _serializerBuffer = new byte[4];
        
        private static void WriteBigEndian(uint value, IMessagePackBuffer buffer)
        {
            unchecked
            {
                _serializerBuffer[3] = (byte) value;
                _serializerBuffer[2] = (byte) (value >> 8);
                _serializerBuffer[1] = (byte) (value >> 16);
                _serializerBuffer[0] = (byte) (value >> 24);
                
                buffer.Write(_serializerBuffer);
            }
        }

        private static void WriteBigEndian(ulong value, IMessagePackBuffer buffer)
        {
            unchecked
            {
                var bigEndian0 = (byte) value;
                var bigEndian1 = (byte) (value >> 8);
                var bigEndian2 = (byte) (value >> 16);
                var bigEndian3 = (byte) (value >> 24);
                var bigEndian4 = (byte) (value >> 32);
                var bigEndian5 = (byte) (value >> 40);
                var bigEndian6 = (byte) (value >> 48);
                var bigEndian7 = (byte) (value >> 56);
                
                buffer.Write(bigEndian7);
                buffer.Write(bigEndian6);
                buffer.Write(bigEndian5);
                buffer.Write(bigEndian4);
                buffer.Write(bigEndian3);
                buffer.Write(bigEndian2);
                buffer.Write(bigEndian1);
                buffer.Write(bigEndian0);
            }
        }
    }
}