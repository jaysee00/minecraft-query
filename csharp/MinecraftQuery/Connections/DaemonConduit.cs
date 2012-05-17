using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MinecraftQuery.Connections
{
    /// <summary>
    /// Conduit that connects a generator and consumer on a background thread until a signal is received to terminate the connection.
    /// </summary>
    internal class DaemonConduit<MessageType> : Conduit<MessageType>
    {
        private const int Delay = 3000; // 1 second pause between each message;

        private readonly Condition stopCondition;

        public DaemonConduit(IMessageGenerator<MessageType> generator, IMessageConsumer<MessageType> consumer, Condition stopCondition) : base(generator, consumer)
        {
            this.stopCondition = stopCondition;
        }

        public override void Channel()
        {
            var daemon = new Thread(run);
            // TODO: Make this a background thread.
            daemon.Start();
        }

        private void run()
        {
            try
            {
                Generator.Start();
                Consumer.Start();

                while (!stopCondition.Evaluate())
                {
                    Consumer.ConsumeMessage(Generator.GetNextMessage());
                    Thread.Sleep(Delay);
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("Thread was aborted.");
            }
            finally
            {
                Generator.Stop();
                Consumer.Stop();
            }
            Console.WriteLine("Daemon stopped.");
        }
    }
}
