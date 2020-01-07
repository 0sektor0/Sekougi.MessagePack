namespace Sekougi.MessagePack
{
    public static class MessagePackRange
    {
        public const byte  POSITIVE_FIX_NUM_MAX = 127;
        public const sbyte NEGATIVE_FIX_NUM_MIN = -32;
        
        public const byte   FIX_ARRAY_MAX_LEN = 16;
        public const byte   FIX_STR_MAX_LEN   = 32;
        public const byte   MAX_LEN_8         = byte.MaxValue;
        public const ushort MAX_LEN_16        = ushort.MaxValue;
        public const uint   MAX_LEN_32        = uint.MaxValue;
    }
}