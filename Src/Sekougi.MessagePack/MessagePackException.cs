using System;

namespace Sekougi.MessagePack
{
    public class MessagePackException : Exception
    {
        public MessagePackException(string message) : base(message)
        {
            
        }
    }
}