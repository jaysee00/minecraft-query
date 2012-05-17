using System;
using System.Collections.Generic;
using System.Text;

namespace MinecraftQuery
{
    class MinecraftQueryMessage
    {
        public String Motd { get; set; }

        public String GameType { get; set; }

        public int PlayersOnline { get; set; }

        public int MaxPlayers { get; set; }





        public override string ToString()
        {
            return "Players Online: " + PlayersOnline;
        }

    }
}
