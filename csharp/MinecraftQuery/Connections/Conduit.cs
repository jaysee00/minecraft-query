using System;
using System.Collections.Generic;
using System.Text;

namespace MinecraftQuery.Connections
{
    public abstract class Conduit<MessageType>
    {
        private readonly IMessageGenerator<MessageType> generator;
        private readonly IMessageConsumer<MessageType> consumer;

        protected IMessageGenerator<MessageType> Generator
        {
            get
            {
                return generator;
            }
        }

        protected IMessageConsumer<MessageType> Consumer
        {
            get
            {
                return consumer;
            }
        }

        public Conduit(IMessageGenerator<MessageType> generator, IMessageConsumer<MessageType> consumer)
        {
            this.generator = generator;
            this.consumer = consumer;
        }

        public abstract void Channel();
    }
}
