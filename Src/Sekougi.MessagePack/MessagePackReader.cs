using System;
using System.Buffers;
using System.Text;
using Sekougi.MessagePack.Exceptions;



namespace Sekougi.MessagePack
{
    public class MessagePackReader
    {
        // TODO: maybe i should move all offset sizes to constants
        private const byte MOVED_FIX_INT_CODE = MessagePackTypeCode.FIX_INT >> 5;
        private const byte MOVED_FIX_STR_CODE = MessagePackTypeCode.FIX_STRING >> 5;
        private const byte MOVED_FIX_ARRAY_CODE = MessagePackTypeCode.FIX_ARRAY >> 4;
        private const byte MOVED_FIX_MAP_CODE = MessagePackTypeCode.FIX_MAP >> 4;

        private readonly IMessagePackBuffer _messagePackBuffer;
        
        
        public MessagePackReader(IMessagePackBuffer messagePackBuffer)
        {
            _messagePackBuffer = messagePackBuffer;
        }
        
        public bool ReadBool()
        {
            var typeCode = _messagePackBuffer.Read();
            switch (typeCode)
            {
                case MessagePackTypeCode.FALSE:
                    return false;
                
                case MessagePackTypeCode.TRUE:
                    return true;
                
                default:
                    throw new InvalidCastException();
            }
        }
        
        public sbyte ReadSbyte()
        {
            var longValue = ReadLong();
            if (longValue > sbyte.MaxValue || longValue < sbyte.MinValue)
                throw new InvalidCastException();

            var value = (sbyte) longValue;
            return value;
        }

        public byte ReadByte()
        {
            var longValue = ReadUlong();
            if (longValue > byte.MaxValue)
                throw new InvalidCastException();

            var value = (byte) longValue;
            return value;
        }

        public short ReadShort()
        {
            var longValue = ReadLong();
            if (longValue > short.MaxValue || longValue < short.MinValue)
                throw new InvalidCastException();

            var value = (short) longValue;
            return value;
        }

        public ushort ReadUshort()
        {
            var longValue = ReadUlong();
            if (longValue > ushort.MaxValue)
                throw new InvalidCastException();

            var value = (ushort) longValue;
            return value;
        }

        public int ReadInt()
        {
            var longValue = ReadLong();
            if (longValue > int.MaxValue || longValue < int.MinValue)
                throw new InvalidCastException();

            var value = (int) longValue;
            return value;
        }

        public uint ReadUint()
        {
            var longValue = ReadUlong();
            if (longValue > uint.MaxValue)
                throw new InvalidCastException();

            var value = (uint) longValue;
            return value;
        }

        public long ReadLong()
        {
            var typeCode = _messagePackBuffer.Read();

            var isFixNum = typeCode >> 5 == MOVED_FIX_INT_CODE || typeCode >> 7 == 0;
            if (isFixNum)
                return (sbyte) typeCode;

            switch (typeCode)
            {
                case MessagePackTypeCode.INT8:
                    return (sbyte) _messagePackBuffer.Read();
                
                case MessagePackTypeCode.INT16:
                    return ReadBigEndianShort();

                case MessagePackTypeCode.INT32:
                    return ReadBigEndianInt();

                case MessagePackTypeCode.INT64:
                    return ReadBigEndianLong();

                default:
                    throw new InvalidCastException();
            }
        }

        public ulong ReadUlong()
        {
            var typeCode = _messagePackBuffer.Read();

            var isFixNum = typeCode >> 7 == 0;
            if (isFixNum)
                return typeCode;

            switch (typeCode)
            {
                case MessagePackTypeCode.UINT8:
                    return _messagePackBuffer.Read();
                
                case MessagePackTypeCode.UINT16:
                    return ReadBigEndianUshort();

                case MessagePackTypeCode.UINT32:
                    return ReadBigEndianUint();

                case MessagePackTypeCode.UINT64:
                    return ReadBigEndianUlong();

                default:
                    throw new InvalidCastException();
            }
        }

        public float ReadFloat()
        {
            var typeCode = _messagePackBuffer.Read();
            if (typeCode != MessagePackTypeCode.FLOAT32)
                throw new InvalidCastException();

            var value = ReadBigEndianFloat();
            return value;
        }

        public double ReadDouble()
        {
            var typeCode = _messagePackBuffer.Read();
            if (typeCode != MessagePackTypeCode.FLOAT64)
                throw new InvalidCastException();

            var value = ReadBigEndianDouble();
            return value;
        }

        public string ReadString(Encoding encoding)
        {
            var typeCode = _messagePackBuffer.Read();
            int bytesLength;

            var isFixStr = typeCode >> 5 == MOVED_FIX_STR_CODE;
            if (isFixStr)
                bytesLength = typeCode - MessagePackTypeCode.FIX_STRING;
            else switch (typeCode)
            {
                case MessagePackTypeCode.NIL:
                    return null;
                
                case MessagePackTypeCode.STR8: 
                    bytesLength = _messagePackBuffer.Read();
                    break;
                
                case MessagePackTypeCode.STR16:
                    bytesLength = ReadBigEndianUshort();
                    break;
                
                case MessagePackTypeCode.STR32:
                    var length = ReadBigEndianUint();
                    if (length > int.MaxValue)
                        throw new MessagePackStringTooLongException("string too long");
                    bytesLength = (int) length;
                    break;
                
                default:
                    throw new InvalidCastException();
            }

            var str = ReadStringInternal(encoding, bytesLength);
            return str;
        }

        private string ReadStringInternal(Encoding encoding, int bytesLength)
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
                _messagePackBuffer.Read(buffer);
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

        public byte[] ReadBinary()
        {
            var typeCode = _messagePackBuffer.Read();
            var length = 0;
            
            switch (typeCode)
            {
                case MessagePackTypeCode.BIN8:
                    length = _messagePackBuffer.Read();
                    break;
                
                case MessagePackTypeCode.BIN16:
                    length = ReadBigEndianUshort();
                    break;
                    
                case MessagePackTypeCode.BIN32:
                    length = ReadBigEndianInt();
                    break;
                
                default:
                    throw new InvalidCastException();
            }

            var binaryData = new byte[length];
            _messagePackBuffer.Read(binaryData, 0, length);

            return binaryData;
        }

        public int ReadDictionaryLength()
        {
            return ReadCollectionLength(MessagePackTypeCode.FIX_MAP, MOVED_FIX_MAP_CODE,
                MessagePackTypeCode.MAP16, MessagePackTypeCode.MAP32);
        }

        public int ReadArrayLength()
        {
            return ReadCollectionLength(MessagePackTypeCode.FIX_ARRAY, MOVED_FIX_ARRAY_CODE,
                MessagePackTypeCode.ARRAY16, MessagePackTypeCode.ARRAY32);
        }
                
        public DateTime ReadDateTime()
        {
            var timeSpan = ReadTimeSpan();
            var unixTime = new DateTime(1970, 1, 1);
            var time = unixTime.Add(timeSpan);
            
            return time;
        }

        public TimeSpan ReadTimeSpan()
        {
            var typeCode = _messagePackBuffer.Read();
            switch (typeCode)
            {
                case MessagePackTypeCode.TIMESTAMP32:
                    return ReadTimeSpan32();
                
                case MessagePackTypeCode.TIMESTAMP64:
                    return ReadTimeSpan64();
                
                case MessagePackTypeCode.TIMESTAMP96:
                    return ReadTimeSpan96();
                
                default:
                    throw new InvalidCastException();
            }
        }

        private TimeSpan ReadTimeSpan32()
        {
            _messagePackBuffer.Read();
            
            var ticks = ReadBigEndianUint() * TimeSpan.TicksPerSecond;
            var timeSpan = TimeSpan.FromTicks(ticks);
            
            return timeSpan;
        }

        private TimeSpan ReadTimeSpan64()
        {
            _messagePackBuffer.Read();
            
            var timeData = ReadBigEndianUlong();
            var seconds = (int) (0x3_FFFF_FFFF & timeData);
            var nanoSeconds = (int) (timeData >> 34);
            
            var ticks = nanoSeconds / DateTimeConstants.NanosecondsPerTick + seconds * TimeSpan.TicksPerSecond;
            var timeSpan = TimeSpan.FromTicks(ticks);
            
            return timeSpan;
        }

        private TimeSpan ReadTimeSpan96()
        {
            _messagePackBuffer.Read();
            _messagePackBuffer.Read();
            
            var nanoSeconds = ReadBigEndianUint();
            var seconds = ReadBigEndianUlong();

            var ticks = nanoSeconds / DateTimeConstants.NanosecondsPerTick;
            var fromTicks = TimeSpan.FromTicks(ticks);
            var fromSeconds = TimeSpan.FromSeconds(seconds);
            var timeSpan = fromSeconds.Add(fromTicks);
            
            return timeSpan;
        }
        
        private int ReadCollectionLength(byte prefix, byte movedPrefix, byte code16, byte code32)
        {
            var typeCode = _messagePackBuffer.Read();
            if (typeCode >> 4 == movedPrefix)
                return typeCode - prefix;

            if (typeCode == code16)
                return ReadBigEndianUshort();
            
            if (typeCode == code32)
                return ReadBigEndianInt();

            throw new InvalidCastException();
        }
        
        private ushort ReadBigEndianUshort()
        {
            var value = ReadBigEndianShort();
            return unchecked((ushort)value);
        }

        private uint ReadBigEndianUint()
        {
            var value = ReadBigEndianInt();
            return unchecked((uint)value);
        }

        private ulong ReadBigEndianUlong()
        {
            var value = ReadBigEndianLong();
            return unchecked((ulong)value);
        }

        private short ReadBigEndianShort()
        {
            var bigEndian0 = (short) (_messagePackBuffer.Read() << 8);
            var bigEndian1 = (short) _messagePackBuffer.Read();

            var value = (short) (bigEndian0 | bigEndian1);
            return value;
        }

        private int ReadBigEndianInt()
        {
            var bigEndian0 = _messagePackBuffer.Read() << 24;
            var bigEndian1 = _messagePackBuffer.Read() << 16;
            var bigEndian2 = _messagePackBuffer.Read() << 8;
            var bigEndian3 = _messagePackBuffer.Read();

            var value = bigEndian0 | bigEndian1 | bigEndian2 | bigEndian3;
            return value;
        }

        private long ReadBigEndianLong()
        {
            var bigEndian0 = (long) _messagePackBuffer.Read() << 56;
            var bigEndian1 = (long) _messagePackBuffer.Read() << 48;
            var bigEndian2 = (long) _messagePackBuffer.Read() << 40;
            var bigEndian3 = (long) _messagePackBuffer.Read() << 32;
            var bigEndian4 = (long) _messagePackBuffer.Read() << 24;
            var bigEndian5 = (long) _messagePackBuffer.Read() << 16;
            var bigEndian6 = (long) _messagePackBuffer.Read() << 8;
            var bigEndian7 = (long) _messagePackBuffer.Read();

            var value = bigEndian0 | bigEndian1 | bigEndian2 | bigEndian3 | bigEndian4 | bigEndian5 | bigEndian6 | bigEndian7;
            return value;
        }

        private unsafe float ReadBigEndianFloat()
        {
            var intValue = ReadBigEndianInt();
            var floatValue = *(float*) &intValue;

            return floatValue;
        }

        private unsafe double ReadBigEndianDouble()
        {
            var longValue = ReadBigEndianLong();
            var doubleValue = *(double*) &longValue;

            return doubleValue;
        }
    }
}