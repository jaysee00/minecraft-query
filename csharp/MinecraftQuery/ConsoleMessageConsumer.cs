using MinecraftQuery.Connections;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinecraftQuery
{
    public class ConsoleMessageConsumer<MessageType> : IMessageConsumer<MessageType>
    {
        public void Start()
        {
            // no-op
        }

        public void Stop()
        {
            // no-op
        }

        public void ConsumeMessage(MessageType message)
        {
            Console.WriteLine("Message recieved: " + message.ToString());
        }

    }
}
