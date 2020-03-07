using System;
using System.Text;
using Sekougi.MessagePack.Exceptions;



namespace Sekougi.MessagePack
{
    public static class MessagePackPrimitivesWriter
    {
        public static void WriteNull(IMessagePackBuffer buffer)
        {
            buffer.WriteByte(MessagePackTypeCode.NIL);
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
            buffer.WriteByte(value ? MessagePackTypeCode.TRUE : MessagePackTypeCode.FALSE);
        }

        public static void Write(byte value, IMessagePackBuffer buffer)
        {
            if (value > MessagePackRange.POSITIVE_FIX_INT_MAX)
            {
                buffer.WriteByte(MessagePackTypeCode.UINT8);
            }
            
            buffer.WriteByte(value);
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
                buffer.WriteByte(byteValue);
                return;
            }
            
            buffer.WriteByte(MessagePackTypeCode.INT8);
            buffer.WriteByte(byteValue);
        }
        
        public static void Write(short value, IMessagePackBuffer buffer)
        {
            var canUseSbyte = value <= sbyte.MaxValue && value >= sbyte.MinValue;
            if (canUseSbyte)
            {
                Write((sbyte)value, buffer);
                return;
            }
            
            buffer.WriteByte(MessagePackTypeCode.INT16);
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
            
            buffer.WriteByte(MessagePackTypeCode.UINT16);
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

            buffer.WriteByte(MessagePackTypeCode.UINT32);
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
            
            buffer.WriteByte(MessagePackTypeCode.UINT64);
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
            
            buffer.WriteByte(MessagePackTypeCode.INT32);
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
            
            buffer.WriteByte(MessagePackTypeCode.INT64);
            WriteBigEndian(value, buffer);
        }
        
        public static void Write(float value, IMessagePackBuffer buffer)
        {
            buffer.WriteByte(MessagePackTypeCode.FLOAT32);
            WriteBigEndian(value, buffer);
        }
        
        public static void Write(double value, IMessagePackBuffer buffer)
        {
            buffer.WriteByte(MessagePackTypeCode.FLOAT64);
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
                    buffer.WriteByte(MessagePackTypeCode.TIMESTAMP32);
                    buffer.WriteByte(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP));
                    WriteBigEndian(data32, buffer);
                }
                else
                {
                    buffer.WriteByte(MessagePackTypeCode.TIMESTAMP64);
                    buffer.WriteByte(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP));
                    WriteBigEndian(data64, buffer);
                }
            }
            else
            {
                buffer.WriteByte(MessagePackTypeCode.TIMESTAMP96);
                buffer.WriteByte(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP96));
                buffer.WriteByte(unchecked((byte) MessagePackExtensionTypeCode.TIMESTAMP));
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
                buffer.WriteByte((byte) code);
            }
            else if (length < ushort.MaxValue)
            {
                buffer.WriteByte(code16);
                WriteBigEndian((ushort) length, buffer);
            }
            else
            {
                buffer.WriteByte(code32);
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
            buffer.WriteByte((byte) prefix);
            buffer.Write(stringBytes);
        }
        
        private static void WriteBinaryData8(byte typeCode, byte[] bytes, IMessagePackBuffer buffer)
        {
            buffer.WriteByte(typeCode);
            
            var byteSize = (byte) bytes.Length;
            buffer.WriteByte(byteSize);
            buffer.Write(bytes);
        }
        
        private static void WriteBinaryData16(byte typeCode, byte[] bytes, IMessagePackBuffer buffer)
        {
            buffer.WriteByte(typeCode);
            
            var byteSize = (ushort) bytes.Length;
            WriteBigEndian(byteSize, buffer);
            buffer.Write(bytes);
        }
        
        private static void WriteBinaryData32(byte typeCode, byte[] bytes, IMessagePackBuffer buffer)
        {
            buffer.WriteByte(typeCode);
            
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
                
                buffer.WriteByte(bigEndian1);
                buffer.WriteByte(bigEndian0);
            }
        }
        
        private static void WriteBigEndian(uint value, IMessagePackBuffer buffer)
        {
            unchecked
            {
                var bigEndian0 = (byte) value;
                var bigEndian1 = (byte) (value >> 8);
                var bigEndian2 = (byte) (value >> 16);
                var bigEndian3 = (byte) (value >> 24);
                
                buffer.WriteByte(bigEndian3);
                buffer.WriteByte(bigEndian2);
                buffer.WriteByte(bigEndian1);
                buffer.WriteByte(bigEndian0);
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
                
                buffer.WriteByte(bigEndian7);
                buffer.WriteByte(bigEndian6);
                buffer.WriteByte(bigEndian5);
                buffer.WriteByte(bigEndian4);
                buffer.WriteByte(bigEndian3);
                buffer.WriteByte(bigEndian2);
                buffer.WriteByte(bigEndian1);
                buffer.WriteByte(bigEndian0);
            }
        }
    }
}