using MinecraftQuery.Connections;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace MinecraftQuery
{
    class Program
    {
        private const String DefaultPortName = "/dev/tty.usbmodemfd121";
        private const int baud = 9600;

        static void Main(string[] args)
        {
            var portName = "";
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("No port name specified. Looking for a suitable default.");
                var ports = SerialPort.GetPortNames();
                foreach (var port in ports)
                {
                    if (port.StartsWith("/dev/tty.usbmodem", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Found port " + port);
                        portName = port;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Skipping " + port);
                    }
                }

                if (String.IsNullOrEmpty(portName))
                {
                    Console.WriteLine("Falling back to Default Port Name " + DefaultPortName);
                    portName = DefaultPortName;

                }
            }
            else
            {
                portName = args[0];
                Console.WriteLine("Connecting to port " + portName);
            }
            
            var condition = new SettableCondition();
            condition.Value = false;

            var conduit = ConduitFactory//.Connect<MinecraftQueryMessage>(new RandomMessageGenerator())
                                        .Connect<MinecraftQueryMessage>(new MinecraftQueryMessageGenerator("localhost", 25565))
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
