using System;
using System.Buffers;
using System.IO;
using System.Text;


namespace Sekougi.MessagePack
{
    public static class MessagePackPrimitivesReader
    {
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

            var isFixNum = typeCode >> 5 == 7 || typeCode >> 7 == 0;
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

            var isFixStr = typeCode >> 5 == 5;
            if (isFixStr)
                bytesLength = typeCode - MessagePackTypeCode.FIX_STR;
            else switch (typeCode)
            {
                case MessagePackTypeCode.STR8: 
                    bytesLength = stream.ReadByte();
                    break;
                
                case MessagePackTypeCode.STR16:
                    bytesLength = ReadBigEndianShort(stream);
                    break;
                
                case MessagePackTypeCode.STR32:
                    bytesLength = ReadBigEndianInt(stream);
                    break;
                
                default:
                    throw new InvalidCastException();
            }

            var str = ReadStringInternal(stream, encoding, bytesLength);
            return str;
        }

        public static DateTime ReadDateTime(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static byte[] ReadBinary(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static int ReadDictionaryLength(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static int ReadArrayLength(Stream stream)
        {
            throw new NotImplementedException();
        }

        private static string ReadStringInternal(Stream stream, Encoding encoding, int bytesLength)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(bytesLength);

            try
            {
                var buffer = new Span<byte>(bytes, 0, bytesLength);
                stream.Read(buffer);
                
                return encoding.GetString(buffer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
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

            var value = (short) checked(bigEndian0 + bigEndian1);
            return value;
        }

        private static int ReadBigEndianInt(Stream stream)
        {
            var bigEndian0 = stream.ReadByte() << 24;
            var bigEndian1 = stream.ReadByte() << 16;
            var bigEndian2 = stream.ReadByte() << 8;
            var bigEndian3 = stream.ReadByte();

            var value = checked(bigEndian0 + bigEndian1 + bigEndian2 + bigEndian3);
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

            var value = checked(bigEndian0 + bigEndian1 + bigEndian2 + bigEndian3 + bigEndian4 + bigEndian5 + bigEndian6 + bigEndian7);
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