using System;
using System.Collections.Generic;
using System.IO;



namespace Sekougi.MessagePack
{
    public static class MessagePackPrimitivesReader
    {
        public static sbyte ReadSbyte(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static byte ReadByte(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static short ReadShort(Stream stream)
        {
            throw new NotImplementedException();
        }
        
        public static ushort ReadUhort(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static int ReadInt(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static uint Readuint(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static long ReadLong(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static ulong RealUlong(Stream stream)
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
    }
}