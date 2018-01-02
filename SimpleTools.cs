using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.UI.Chat;
using TerraUI;
using TerraUI.Objects;
using TerraUI.Panels;
using TerraUI.Utilities;

namespace SimpleTools
{
    class SimpleTools : Mod
    {
        private bool UiOpen = false;
        private InventoryWindow Window;

        private ModHotKey ShowHotKey;

        public override void Load()
        {
            
            if (!Main.dedServ)
            {
                ShowHotKey = RegisterHotKey("Open Experimental Tools", "X");

                UIUtils.Mod = this;
                UIUtils.Subdirectory = "TerraUI";

                Window = new InventoryWindow();
            }

            base.Load();
        }  
        
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (!Main.dedServ && UiOpen)
            {
                Window.Update();
                Window.Draw(spriteBatch);
            }

            base.PostDrawInterface(spriteBatch);
        }

        public override void HotKeyPressed(string name)
        {
            if (!Main.dedServ && name == "Open Experimental Tools" && ShowHotKey.JustPressed && Main.player[Main.myPlayer].name == "Forrest")
            {
                UiOpen = !UiOpen;
            }
            
            base.HotKeyPressed(name);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (Main.dedServ)
            {
                // true = Connected, false = Disconnected
                bool connected = reader.ReadBoolean();
                string playerName = reader.ReadString();

                if (connected)
                {
                    AntiCheat.PlayerConnected(Main.player.FromName(playerName));
                }
                else
                {
                    AntiCheat.PlayerDisconnected(Main.player.FromName(playerName));
                }
            }

            base.HandlePacket(reader, whoAmI);
        }

        public override void PreSaveAndQuit()
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = GetPacket();
                packet.Write(false);
                packet.Write(Main.player[Main.myPlayer].name);
                packet.Send();
            }

            base.PreSaveAndQuit();
        }
    }
}
