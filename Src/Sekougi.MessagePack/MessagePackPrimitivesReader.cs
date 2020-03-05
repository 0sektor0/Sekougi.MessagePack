using System;
using System.IO;
using System.Collections.Generic;


namespace Sekougi.MessagePack
{
    public static class MessagePackPrimitivesReader
    {
        public static sbyte ReadSbyte(Stream stream)
        {
            var longValue = ReadLong(stream);
            if (longValue > sbyte.MaxValue || longValue < sbyte.MinValue)
                throw new OverflowException();
            
            var value = (sbyte) longValue;
            return value;
        }

        public static byte ReadByte(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static short ReadShort(Stream stream)
        {
            var longValue = ReadLong(stream);
            if (longValue > short.MaxValue || longValue < short.MinValue)
                throw new OverflowException();
            
            var value = (short) longValue;
            return value;
        }
        
        public static ushort ReadUhort(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static int ReadInt(Stream stream)
        {
            var longValue = ReadLong(stream);
            if (longValue > int.MaxValue || longValue < int.MinValue)
                throw new OverflowException();
            
            var value = (int) longValue;
            return value;
        }

        public static uint Readuint(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static long ReadLong(Stream stream)
        {
            var typeCode = (byte) stream.ReadByte();

            var isFixNum = typeCode >> 5 == 7 || typeCode >> 7 == 0;
            if (isFixNum)
                return (sbyte) typeCode;

            if (typeCode == MessagePackTypeCode.INT8) 
                return (sbyte) stream.ReadByte();

            if (typeCode == MessagePackTypeCode.INT16)
                return ReadBigEndianShort(stream);

            if (typeCode == MessagePackTypeCode.INT32)
                return ReadBigEndianInt(stream);

            if (typeCode == MessagePackTypeCode.INT64)
                return ReadBigEndianLong(stream);
            
            throw new OverflowException();
        }

        public static ulong ReadUlong(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static float ReadFloat(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static double ReadDouble(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static string ReadString(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static DateTime ReadDateTime(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static byte[] ReadBinary(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static int ReadBinaryLength(Stream stream)
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
        
        // TODO: need to create normal serializer/deserializer for generic types
        public static T[] ReadArray<T>(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(Stream stream)
        {
            throw new NotImplementedException();
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
    }
}