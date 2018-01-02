using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SimpleTools
{
    static class Extensions
    {
        public static Player FromName(this Player[] players, string name)
        {
            foreach (var player in players)
            {
                if (player.name == name)
                {
                    return player;
                }
            }

            throw new Exception("Could not find player " + name);
        }
    }
}
