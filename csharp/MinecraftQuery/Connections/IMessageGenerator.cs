using System;
using System.Collections.Generic;
using System.Text;

namespace MinecraftQuery.Connections
{
    public interface IMessageGenerator<MessageType>
    {
        void Start();
        void Stop();

        MessageType GetNextMessage();
    }
}
