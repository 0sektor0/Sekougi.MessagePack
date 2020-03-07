using System;

namespace Sekougi.MessagePack.Exceptions
{
    public class MessagePackException : Exception
    {
        public MessagePackException(string message) : base(message)
        {
            
        }
    }
}