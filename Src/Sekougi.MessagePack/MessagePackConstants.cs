namespace Sekougi.MessagePack
{
    public static class MessagePackConstants
    {
        public const byte POSITIVE_FIX_NUM_MAX  = 127;
        public const sbyte NEGATIVE_FIX_NUM_MIN = -32;
        
        public const byte FIX_STR_MAX_LEN = 32;
        public const byte FIX_ARRAY_MAX_LEN = 16;
        public const byte ARRAY8_MAX_LEN    = byte.MaxValue;
        public const ushort ARRAY16_MAX_LEN   = ushort.MaxValue;
        public const uint ARRAY32_MAX_LEN  = uint.MaxValue;
        
        public const byte NIL_CODE     = 0xc0;
        public const byte FALSE_CODE   = 0xc2;
        public const byte TRUE_CODE    = 0xc3;
        public const byte UINT8_CODE   = 0xcc;
        public const byte UINT16_CODE  = 0xcd;
        public const byte UINT32_CODE  = 0xce;
        public const byte UINT64_CODE  = 0xcf;
        public const byte INT8_CODE    = 0xd0;
        public const byte INT16_CODE   = 0xd1;
        public const byte INT32_CODE   = 0xd2;
        public const byte INT64_CODE   = 0xd3;
        public const byte FLOAT32_CODE = 0xca;
        public const byte FLOAT64_CODE = 0xcb;
        public const byte STR8_CODE    = 0xd9;
        public const byte STR16_CODE   = 0xda;
        public const byte STR32_CODE   = 0xdb;
        public const byte BIN8_CODE    = 0xc4;
        public const byte BIN16_CODE   = 0xc5;
        public const byte BIN32_CODE   = 0xc6;
    }
}