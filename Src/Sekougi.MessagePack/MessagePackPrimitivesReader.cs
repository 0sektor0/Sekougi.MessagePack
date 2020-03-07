using System;
using System.Buffers;
using System.IO;
using System.Text;
using Sekougi.MessagePack.Exceptions;



namespace Sekougi.MessagePack
{
    public static class MessagePackPrimitivesReader
    {
        // TODO: maybe i should move all offset sizes to constants
        private const byte MOVED_FIX_INT_CODE = MessagePackTypeCode.FIX_INT >> 5;
        private const byte MOVED_FIX_STR_CODE = MessagePackTypeCode.FIX_STRING >> 5;
        private const byte MOVED_FIX_ARRAY_CODE = MessagePackTypeCode.FIX_ARRAY >> 4;
        private const byte MOVED_FIX_MAP_CODE = MessagePackTypeCode.FIX_MAP >> 4;
        
        
        public static sbyte ReadSbyte(Stream stream)
        {
            var longValue = ReadLong(stream);
            if (longValue > sbyte.MaxValue || longValue < sbyte.MinValue)
                throw new InvalidCastException();

            var value = (sbyte) longValue;
            return value;
        }

        public static byte ReadByte(Stream stream)
        {
            var longValue = ReadUlong(stream);
            if (longValue > byte.MaxValue)
                throw new InvalidCastException();

            var value = (byte) longValue;
            return value;
        }

        public static short ReadShort(Stream stream)
        {
            var longValue = ReadLong(stream);
            if (longValue > short.MaxValue || longValue < short.MinValue)
                throw new InvalidCastException();

            var value = (short) longValue;
            return value;
        }

        public static ushort ReadUshort(Stream stream)
        {
            var longValue = ReadUlong(stream);
            if (longValue > ushort.MaxValue)
                throw new InvalidCastException();

            var value = (ushort) longValue;
            return value;
        }

        public static int ReadInt(Stream stream)
        {
            var longValue = ReadLong(stream);
            if (longValue > int.MaxValue || longValue < int.MinValue)
                throw new InvalidCastException();

            var value = (int) longValue;
            return value;
        }

        public static uint ReadUint(Stream stream)
        {
            var longValue = ReadUlong(stream);
            if (longValue > uint.MaxValue)
                throw new InvalidCastException();

            var value = (uint) longValue;
            return value;
        }

        public static long ReadLong(Stream stream)
        {
            var typeCode = (byte) stream.ReadByte();

            var isFixNum = typeCode >> 5 == MOVED_FIX_INT_CODE || typeCode >> 7 == 0;
            if (isFixNum)
                return (sbyte) typeCode;

            switch (typeCode)
            {
                case MessagePackTypeCode.INT8:
                    return (sbyte) stream.ReadByte();
                
                case MessagePackTypeCode.INT16:
                    return ReadBigEndianShort(stream);

                case MessagePackTypeCode.INT32:
                    return ReadBigEndianInt(stream);

                case MessagePackTypeCode.INT64:
                    return ReadBigEndianLong(stream);

                default:
                    throw new InvalidCastException();
            }
        }

        public static ulong ReadUlong(Stream stream)
        {
            var typeCode = (byte) stream.ReadByte();

            var isFixNum = typeCode >> 7 == 0;
            if (isFixNum)
                return typeCode;

            switch (typeCode)
            {
                case MessagePackTypeCode.UINT8:
                    return (byte) stream.ReadByte();
                
                case MessagePackTypeCode.UINT16:
                    return ReadBigEndianUshort(stream);

                case MessagePackTypeCode.UINT32:
                    return ReadBigEndianUint(stream);

                case MessagePackTypeCode.UINT64:
                    return ReadBigEndianUlong(stream);

                default:
                    throw new InvalidCastException();
            }
        }

        public static float ReadFloat(Stream stream)
        {
            var typeCode = stream.ReadByte();
            if (typeCode != MessagePackTypeCode.FLOAT32)
                throw new InvalidCastException();

            var value = ReadBigEndianFloat(stream);
            return value;
        }

        public static double ReadDouble(Stream stream)
        {
            var typeCode = stream.ReadByte();
            if (typeCode != MessagePackTypeCode.FLOAT64)
                throw new InvalidCastException();

            var value = ReadBigEndianDouble(stream);
            return value;
        }

        public static string ReadString(Stream stream, Encoding encoding)
        {
            var typeCode = (byte) stream.ReadByte();
            int bytesLength;

            var isFixStr = typeCode >> 5 == MOVED_FIX_STR_CODE;
            if (isFixStr)
                bytesLength = typeCode - MessagePackTypeCode.FIX_STRING;
            else switch (typeCode)
            {
                case MessagePackTypeCode.NIL:
                    return null;
                
                case MessagePackTypeCode.STR8: 
                    bytesLength = stream.ReadByte();
                    break;
                
                case MessagePackTypeCode.STR16:
                    bytesLength = ReadBigEndianUshort(stream);
                    break;
                
                case MessagePackTypeCode.STR32:
                    var length = ReadBigEndianUint(stream);
                    if (length > int.MaxValue)
                        throw new MessagePackStringTooLongException("string too long");
                    bytesLength = (int) length;
                    break;
                
                default:
                    throw new InvalidCastException();
            }

            var str = ReadStringInternal(stream, encoding, bytesLength);
            return str;
        }

        private static string ReadStringInternal(Stream stream, Encoding encoding, int bytesLength)
        {
            if (bytesLength == 0)
            {
                return string.Empty;
            }
            
            if (bytesLength < 0)
            {
                throw new IndexOutOfRangeException();
            }
            
            var bytes = ArrayPool<byte>.Shared.Rent(bytesLength);

            try
            {
                var buffer = new Span<byte>(bytes, 0, bytesLength);
                stream.Read(buffer);
                
                return encoding.GetString(buffer);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        public static byte[] ReadBinary(Stream stream)
        {
            var typeCode = (byte) stream.ReadByte();
            var length = 0;
            
            switch (typeCode)
            {
                case MessagePackTypeCode.BIN8:
                    length = stream.ReadByte();
                    break;
                
                case MessagePackTypeCode.BIN16:
                    length = ReadBigEndianUshort(stream);
                    break;
                    
                case MessagePackTypeCode.BIN32:
                    length = ReadBigEndianInt(stream);
                    break;
                
                default:
                    throw new InvalidCastException();
            }

            var binaryData = new byte[length];
            stream.Read(binaryData);

            return binaryData;
        }

        public static int ReadDictionaryLength(Stream stream)
        {
            return ReadCollectionLength(stream, MessagePackTypeCode.FIX_MAP, MOVED_FIX_MAP_CODE,
                MessagePackTypeCode.MAP16, MessagePackTypeCode.MAP32);
        }

        public static int ReadArrayLength(Stream stream)
        {
            return ReadCollectionLength(stream, MessagePackTypeCode.FIX_ARRAY, MOVED_FIX_ARRAY_CODE,
                MessagePackTypeCode.ARRAY16, MessagePackTypeCode.ARRAY32);
        }
                
        public static DateTime ReadDateTime(Stream stream)
        {
            var typeCode = (byte) stream.ReadByte();
            switch (typeCode)
            {
                case MessagePackTypeCode.TIMESTAMP32:
                    return ReadDatetime32(stream);
                
                case MessagePackTypeCode.TIMESTAMP64:
                    return ReadDatetime64(stream);
                
                case MessagePackTypeCode.TIMESTAMP96:
                    return ReadDatetime96(stream);
                
                default:
                    throw new InvalidCastException();
            }
        }

        private static DateTime ReadDatetime32(Stream stream)
        {
            stream.ReadByte();
            var seconds = ReadBigEndianUint(stream);
            
            var unixTime = new DateTime(1970, 1, 1);
            var time = unixTime.AddSeconds(seconds);

            return time;
        }

        private static DateTime ReadDatetime64(Stream stream)
        {
            stream.ReadByte();
            var timeData = ReadBigEndianUlong(stream);

            var seconds = (int) (0x3_FFFF_FFFF & timeData);
            var nanoSeconds = (int) (timeData >> 34);
            var ticks = nanoSeconds / DateTimeConstants.NanosecondsPerTick;
            
            var unixTime = new DateTime(1970, 1, 1);
            var time = unixTime.AddSeconds(seconds);
            time = time.AddTicks(ticks);

            return time;
        }

        private static DateTime ReadDatetime96(Stream stream)
        {
            stream.ReadByte();
            stream.ReadByte();
            var nanoSeconds = ReadBigEndianUint(stream);
            var seconds = ReadBigEndianUlong(stream);
            var ticks = nanoSeconds / DateTimeConstants.NanosecondsPerTick;
            
            var unixTime = new DateTime(1970, 1, 1);
            var time = unixTime.AddSeconds(seconds);
            time = time.AddTicks(ticks);

            return time;
        }
        
        private static int ReadCollectionLength(Stream stream, byte prefix, byte movedPrefix, byte code16, byte code32)
        {
            var typeCode = (byte) stream.ReadByte();
            if (typeCode >> 4 == movedPrefix)
                return typeCode - prefix;

            if (typeCode == code16)
                return ReadBigEndianUshort(stream);
            
            if (typeCode == code32)
                return ReadBigEndianInt(stream);

            throw new InvalidCastException();
        }
        
        private static ushort ReadBigEndianUshort(Stream stream)
        {
            var value = ReadBigEndianShort(stream);
            return unchecked((ushort)value);
        }

        private static uint ReadBigEndianUint(Stream stream)
        {
            var value = ReadBigEndianInt(stream);
            return unchecked((uint)value);
        }

        private static ulong ReadBigEndianUlong(Stream stream)
        {
            var value = ReadBigEndianLong(stream);
            return unchecked((ulong)value);
        }

        private static short ReadBigEndianShort(Stream stream)
        {
            var bigEndian0 = (short) (stream.ReadByte() << 8);
            var bigEndian1 = (short) stream.ReadByte();

            var value = (short) (bigEndian0 | bigEndian1);
            return value;
        }

        private static int ReadBigEndianInt(Stream stream)
        {
            var bigEndian0 = stream.ReadByte() << 24;
            var bigEndian1 = stream.ReadByte() << 16;
            var bigEndian2 = stream.ReadByte() << 8;
            var bigEndian3 = stream.ReadByte();

            var value = bigEndian0 | bigEndian1 | bigEndian2 | bigEndian3;
            return value;
        }

        private static long ReadBigEndianLong(Stream stream)
        {
            var bigEndian0 = (long) stream.ReadByte() << 56;
            var bigEndian1 = (long) stream.ReadByte() << 48;
            var bigEndian2 = (long) stream.ReadByte() << 40;
            var bigEndian3 = (long) stream.ReadByte() << 32;
            var bigEndian4 = (long) stream.ReadByte() << 24;
            var bigEndian5 = (long) stream.ReadByte() << 16;
            var bigEndian6 = (long) stream.ReadByte() << 8;
            var bigEndian7 = (long) stream.ReadByte();

            var value = bigEndian0 | bigEndian1 | bigEndian2 | bigEndian3 | bigEndian4 | bigEndian5 | bigEndian6 | bigEndian7;
            return value;
        }

        private static unsafe float ReadBigEndianFloat(Stream stream)
        {
            var intValue = ReadBigEndianInt(stream);
            var floatValue = *(float*) &intValue;

            return floatValue;
        }

        private static unsafe double ReadBigEndianDouble(Stream stream)
        {
            var longValue = ReadBigEndianLong(stream);
            var doubleValue = *(double*) &longValue;

            return doubleValue;
        }
    }
}