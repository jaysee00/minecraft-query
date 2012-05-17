using MinecraftQuery.Connections;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace MinecraftQuery
{
    class Program
    {
        private const String portName = "/dev/tty.usbmodemfd121";
        private const int baud = 9600;

        static void Main(string[] args)
        {
            //var buff = new byte[] { 0x39, 0x35, 0x31, 0x33, 0x33, 0x30, 0x37 };

            //int result = Convert.ToInt32(new String(Encoding.ASCII.GetChars(buff)));
            //Console.WriteLine("signed result is " + result);

            //UInt32 uresult = Convert.ToUInt32(new String(Encoding.ASCII.GetChars(buff)));
            //Console.WriteLine("unsigned result is " + uresult);

            //Console.ReadLine();
            var condition = new SettableCondition();
            condition.Value = false;

            var conduit = ConduitFactory//.Connect<MinecraftQueryMessage>(new RandomMessageGenerator())
                                        .Connect<MinecraftQueryMessage>(new MinecraftQueryMessageGenerator("172.20.0.2", 25565))
                                        .With(new ArduinoMessageConsumer(portName, baud))
                                        //.With(new ConsoleMessageConsumer<MinecraftQueryMessage>())
                                        .StopOn(condition).Build();

            conduit.Channel();

            Console.WriteLine("Running.. any key to stop");
            Console.ReadLine();
            condition.Value = true;
        }
    }
}
