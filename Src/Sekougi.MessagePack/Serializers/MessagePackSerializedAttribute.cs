using System;



namespace Sekougi.MessagePack.Serializers
{
    public class MessagePackSerializedAttribute : Attribute
    {
        public Type SerializedType { get; }

        
        public MessagePackSerializedAttribute(Type serializedType)
        {
            SerializedType = serializedType;
        }
    }
}