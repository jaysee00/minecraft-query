using System;
using System.Collections.Generic;
using System.Text;

namespace MinecraftQuery.Connections
{
    public class ConduitFactory
    {
        public class ConduitBuilder<MessageType>
        {
            private IMessageConsumer<MessageType> consumer;
            private IMessageGenerator<MessageType> generator;
            private Condition stopCondition;

            public ConduitBuilder(IMessageGenerator<MessageType> generator)
            {
                this.generator = generator;
            }


            public ConduitBuilder<MessageType> With(IMessageConsumer<MessageType> consumer)
            {
                this.consumer = consumer;
                return this;
            }

            public ConduitBuilder<MessageType> StopOn(Condition condition)
            {
                this.stopCondition = condition;
                return this;
            }

            public Conduit<MessageType> Build()
            {
                return new DaemonConduit<MessageType>(generator, consumer, stopCondition);
            }

        }

        public static ConduitBuilder<MessageType> Connect<MessageType>(IMessageGenerator<MessageType> generator)
        {
            return new ConduitBuilder<MessageType>(generator);
        }

    }
}
