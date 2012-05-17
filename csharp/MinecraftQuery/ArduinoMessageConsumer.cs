using MinecraftQuery.Connections;
using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MinecraftQuery
{
    class ArduinoMessageConsumer : IMessageConsumer<MinecraftQueryMessage>
    {
        private readonly String serialPortName;
        private readonly int baud;

        private SerialPort port;

        public ArduinoMessageConsumer(String serialPortName, int baud)
        {
            this.serialPortName = serialPortName;
            this.baud = baud;
        }

        public void Start()
        {
            port = new SerialPort(serialPortName, baud);
            port.Open();
            Thread.Sleep(2000); // Give the arduino board some time to schiz out. TODO: is this necessary?
        }

        public void Stop()
        {
            // Return the LEDs on the board to their default state.
            Write(0); 

            if (port != null && port.IsOpen)
                port.Close();
        }

        private void Write(int value)
        {
            // Convert the number of players online into an ASCII byte, since we are sending character data to the arduino board
            // and the mono implementation of SerialPorts allegedly doesn't support writing character data on OSX.
            var bytesToWrite = Encoding.ASCII.GetBytes(value.ToString()); // TODO: Ensure that this is just one byte.

            port.Write(bytesToWrite, 0, bytesToWrite.Length);
        }

        public void ConsumeMessage(MinecraftQueryMessage message)
        {
            Write(message.PlayersOnline);
        }
    }
}
