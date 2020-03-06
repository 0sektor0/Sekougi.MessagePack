namespace Sekougi.MessagePack
{
    public static class MessagePackTypeCode
    {
        public const byte NIL     = 0xc0;
        public const byte FALSE   = 0xc2;
        public const byte TRUE    = 0xc3;
        public const byte UINT8   = 0xcc;
        public const byte UINT16  = 0xcd;
        public const byte UINT32  = 0xce;
        public const byte UINT64  = 0xcf;
        public const byte INT8    = 0xd0;
        public const byte INT16   = 0xd1;
        public const byte INT32   = 0xd2;
        public const byte INT64   = 0xd3;
        public const byte FLOAT32 = 0xca;
        public const byte FLOAT64 = 0xcb;
        public const byte STR8    = 0xd9;
        public const byte STR16   = 0xda;
        public const byte STR32   = 0xdb;
        public const byte BIN8    = 0xc4;
        public const byte BIN16   = 0xc5;
        public const byte BIN32   = 0xc6;
        public const byte ARRAY16 = 0xdc;
        public const byte ARRAY32 = 0xdd;
        public const byte MAP16   = 0xde;
        public const byte MAP32   = 0xdf;
        
        public const byte TIMESTAMP32 = 0xd6;
        public const byte TIMESTAMP64 = 0xd7;
        public const byte TIMESTAMP96 = 0xc7;
        
        public const byte FIX_INT    = 0b_1110_0000;
        public const byte FIX_MAP    = 0b_1001_0000;
        public const byte FIX_ARRAY  = 0b_1000_0000;
        public const byte FIX_STRING = 0b_1010_0000;
    }
}