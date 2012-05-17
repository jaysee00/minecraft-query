using MinecraftQuery.Connections;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinecraftQuery
{
    class RandomMessageGenerator : IMessageGenerator<MinecraftQueryMessage>
    {
        private Random random;

        public void Start()
        {
            random = new Random();
        }

        public void Stop()
        {
            // no-op
        }

        public MinecraftQueryMessage GetNextMessage()
        {
            return new MinecraftQueryMessage
            {
                PlayersOnline = random.Next(0, 3)
            };
        }
    }
}
