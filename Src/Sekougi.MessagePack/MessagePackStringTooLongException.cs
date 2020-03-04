namespace Sekougi.MessagePack
{
    public class MessagePackStringTooLongException : MessagePackException
    {
        public MessagePackStringTooLongException(string message) : base(message)
        {
            
        }
    }
}