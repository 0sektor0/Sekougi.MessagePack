using System;
using System.Collections.Generic;
using Sekougi.MessagePack.Serializers;



namespace Sekougi.MessagePack.ConsoleTest
{
    class Program
    {
        static void Main()
        {
            var dictionary = new Dictionary<string, int>();
            dictionary.Add("asd", 123);
            dictionary.Add("dsa", 321);
            
            using var buffer = new MessagePackBuffer();
            var serializer = MessagePackSerializersReposetory.Get<Dictionary<string, int>>();
            serializer.Serialize(buffer, dictionary);

            buffer.Position = 0;
            var value = serializer.Deserialize(buffer);
        }
    }
}