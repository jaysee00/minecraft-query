using MinecraftQuery.Connections;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace MinecraftQuery
{
    class MinecraftQueryMessageGenerator : IMessageGenerator<MinecraftQueryMessage>
    {
        private readonly String remoteHostName;
        private readonly int remotePort;

        private UdpClient client;
        private Random random;
        private IPEndPoint remoteEndPoint;

        public MinecraftQueryMessageGenerator(String remoteHostName, int remotePort)
        {
            this.remoteHostName = remoteHostName;
            this.remotePort = remotePort;
        }

        public void Start()
        {
            random = new Random();
            client = new UdpClient(remoteHostName, remotePort);

            Console.WriteLine("Resolving " + remoteHostName);
            var hostEntry = Dns.GetHostEntry(remoteHostName);
            Console.WriteLine("Has " + hostEntry.AddressList.Length + " addresses");
            var address = hostEntry.AddressList[0].GetAddressBytes();

            var ipAddress = new IPAddress(address);
            remoteEndPoint = new IPEndPoint(ipAddress, remotePort);
        }

        public void Stop()
        {
            client = null;
        }

        public MinecraftQueryMessage GetNextMessage()
        {
            /**
             * To receive your challenge token, you send a request like the following:
                [
                    0xFE, 0xFD,             // Magic bytes
                    0x09,                   // Challenge type
                    0x01, 0x02, 0x03, 0x04  // Your ID token
                ]
             * **/

            // Do handshake to obtain challenge token.
            var handshakeSend = new byte[]  { 0xFE, 0xFD, // Magic bytes
                                              0x09, // Challenge type,
                                              0x00, 0x00, 0x00, 0x00 // My ID Token. To be filled.
                                            };
            var id = new byte[4];
            random.NextBytes(id);
            Array.Copy(id, 0, handshakeSend, 3, id.Length);

            var bytesSent = client.Send(handshakeSend, handshakeSend.Length);
            Debug.Assert(bytesSent == handshakeSend.Length);

            /**
             * The ID token may be anything you like, you may even omit it entirely (which will default it to 4 NUL bytes). You'll then receive the following:
                [
                    0x09,                   // Challenge type
                    0x01, 0x02, 0x03, 0x04, // Your ID token (or 4 NUL)
                    ----------------0xDE, 0xAD, 0xBE, 0xEF  // Your new challenge token (int-32)-------------
                    string   // Your challenge token - int32 packed as null-terminated string.
                ]
             **/
            var handshakeResponse = client.Receive(ref remoteEndPoint);
            var challengeTokenReader = new MemoryStream(handshakeResponse);

            var challengeType = Convert.ToByte(challengeTokenReader.ReadByte());
            Debug.Assert(challengeType == 0x09);

            var responseId = new byte[4];
            challengeTokenReader.Read(responseId, 0, responseId.Length);
            if (!ArrayEquals(id, responseId))
            {
                Console.WriteLine("uh-oh, the IDs have changed somehow. :(. Continuing anyway");
            }

            var challengeToken = ReadIntFromString(challengeTokenReader);
            Console.WriteLine("Challenge Token: " + challengeToken);

            /**
             * I said before that there's only one more packet, which is to receive the server status. However, that packet is split into two, depending on exactly how much you want to know about the server. There's no flag saying which you want, instead it's dependent on the structure of your request. Let's go for the "short and sweet" reply, which contains everything you'd want to know at a glance. Send the following:

                [
                    0xFE, 0xFD,             // Magic bytes
                    0x00,                   // Status type
                    0x01, 0x02, 0x03, 0x04, // Your ID token
                    0xDE, 0xAD, 0xBE, 0xEF  // Your challenge token (packed as int32)
                ]
             **/
            var dataSendList = new List<byte>();

            dataSendList.Add(0xFE);
            dataSendList.Add(0xFD); // Magix Bytes
            dataSendList.Add(0x00); // Status Type
            dataSendList.AddRange(id); // My ID token

            Console.WriteLine("Is Little Endian?" + BitConverter.IsLittleEndian);
            var challengeBytes = BitConverter.GetBytes(challengeToken);
            if (BitConverter.IsLittleEndian)
            {
                // Reverse to big-endian.
                Array.Reverse(challengeBytes);
            }

            dataSendList.AddRange(challengeBytes);


            var getDataSend = dataSendList.ToArray();

            var bytesSent2 = client.Send(getDataSend, getDataSend.Length);
            Debug.Assert(bytesSent2 == getDataSend.Length);

            /**
             * Nothing fancy there. The challenge must be valid and that's it. You'll receive the following:
                [
                    0x00,                   // Status type
                    0x01, 0x02, 0x03, 0x04, // Your ID token (that you registered your challenge with)
                    payload                 // See below!
                ]
             * Payload consists of the following information. All strings are null-terminated C-style, and what I refer to as numbers are integers converted into a string in base 10 (So if you receive "30", it's actually 0x1E). Shorts are little-endian, whereas everything else is big-endian.
                string                    // Server MoTD as displayed in the in-game server browser.
                string                    // Game type. Currently hardcoded to "SMP".
                string                    // Name of the default world.
                number                    // How many players are currently online.
                number                    // Maximum number of players this server supports.
                short                     // Port the server is listening on.
                string                    // Host that the server may receive connections on.
            **/
            var statusResponse = client.Receive(ref remoteEndPoint);
            var reader = new MemoryStream(statusResponse, false);
            
            // Status type
            reader.Position = 0;
            var statusType = reader.ReadByte();
            Debug.Assert(statusType == 0x00);

            var idStatusResponse = new byte[4];
            reader.Read(idStatusResponse, 0, idStatusResponse.Length);
            Debug.Assert(ArrayEquals(id, idStatusResponse));

            // Server MotD
            var motd = ReadString(reader);
            var gameType = ReadString(reader);
            var worldName = ReadString(reader);
            var playersOnline = Convert.ToInt32(ReadString(reader));
            var maxPlayers = Convert.ToInt32(ReadString(reader));
            // TODO: read port value
            // TODO: read host value

            return new MinecraftQueryMessage
            {
                Motd = motd,
                GameType = gameType,
                PlayersOnline = playersOnline,
                MaxPlayers = maxPlayers
            };
        }

        private int ReadIntFromString(Stream stream)
        {
            String value = ReadString(stream);
            return Convert.ToInt32(value);

            //int result = Convert.ToInt32(new String(Encoding.ASCII.GetChars(buff)));
            //return BitConverter.ToInt32(buffer.ToArray(), 0);
        }

        private String ReadString(Stream stream)
        {
            var buffer = new List<Byte>();
            do
            {
                int current = stream.ReadByte();
                if (current == -1 || current == 0)
                    break;
                buffer.Add(Convert.ToByte(current));
            }
            while (true);

            return new String(Encoding.ASCII.GetChars(buffer.ToArray()));
        }

        private static bool ArrayEquals(byte[] first, byte[] second)
        {
            if (first.Length != second.Length)
                return false;

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                    return false;
            }

            return true;
        }

    }
}
