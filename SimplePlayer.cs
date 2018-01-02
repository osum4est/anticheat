using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SimpleTools
{
    class SimplePlayer : ModPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write(true);
                packet.Write(player.name);
                packet.Send();
            }
            base.OnEnterWorld(player);
        }
    }
}
