using System;
using System.Collections.Generic;
using System.Text;

namespace MinecraftQuery.Connections
{
    public interface IMessageConsumer<MessageType>
    {
        void Start();
        void Stop();

        void ConsumeMessage(MessageType message);

    }
}
