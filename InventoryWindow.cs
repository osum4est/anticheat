using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SimpleTools.TerraUI.Objects;
using Terraria;
using TerraUI;
using TerraUI.Objects;
using TerraUI.Panels;

namespace SimpleTools
{
    class InventoryWindow
    {
        private UIPanel Window;
        private UIItemSlot[][] InventorySlots;

        private int SlotSize = 36;
        private int Offset = 2;

        private int PlayerLeftOffset = 10;
        private int PlayerListWidth = 100;
        private int PlayerListItemHeight = 36;
        private Dictionary<Player, UILabel> PlayerLabels;

        private int CurrentPlayer = Main.myPlayer;

        private bool MovingWindow;
        private Vector2 MovingOffset;

        private UIConfirmationBox confirmationBox;

        public InventoryWindow()
        {
            float Width = PlayerLeftOffset + PlayerListWidth + Offset + (SlotSize + Offset) * 10 + Offset;
            float Height = (SlotSize + Offset) * 5 + Offset;

            Window = new UIPanel(
                new Vector2(Main.screenWidth / 2f - Width / 2f,
                    Main.screenHeight / 2f - Height / 2f),
                new Vector2(Width, Height),
                true);

            InventorySlots = new UIItemSlot[10][];
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                InventorySlots[i] = new UIItemSlot[5];
                for (int j = 0; j < InventorySlots[i].Length; j++)
                {
                    InventorySlots[i][j] =
                        new UIItemSlot(
                            new Vector2(PlayerLeftOffset + PlayerListWidth + Offset + i * (SlotSize + Offset) + Offset,
                                j * (SlotSize + Offset) + Offset), SlotSize);
                    InventorySlots[i][j].Parent = Window;
                    Window.Children.Add(InventorySlots[i][j]);

//                    int iCopy = i;
//                    int jCopy = j;
//                    InventorySlots[i][j].DoubleClick += (a, b) =>
//                    {
//                        confirmationBox = new UIConfirmationBox("Are you sure you want to delete this stack of items?");
//                        confirmationBox.OnYesClick += () =>
//                        {
//                            Main.player[CurrentPlayer].inventory[jCopy * InventorySlots.Length + iCopy] = null;
//                            confirmationBox = null;
//                        };
//                        confirmationBox.OnNoClick += () => confirmationBox = null;
//                    };
                }
            }

            PlayerLabels = new Dictionary<Player, UILabel>();


            Window.Click += (a, b) =>
            {
                if (b.Button == MouseButtons.Left)
                {
                    MovingWindow = true;
                    MovingOffset = new Vector2(Main.mouseX - Window.Position.X, Main.mouseY - Window.Position.Y);
                    return true;
                }

                return false;
            };

            Window.MouseUp += (a, b) => { MovingWindow = false; };
        }

        public void Update()
        {

            if (MovingWindow)
            {
                Window.Position = new Vector2(Main.mouseX - MovingOffset.X, Main.mouseY - MovingOffset.Y);
            }

            for (int i = 0; i < InventorySlots.Length; i++)
            {
                for (int j = 0; j < InventorySlots[i].Length; j++)
                {
                    InventorySlots[i][j].Item = Main.player[CurrentPlayer].inventory[j * InventorySlots.Length + i];
                }
            }

            for (var i = 0; i < Main.player.Length; i++)
            {
                var player = Main.player[i];
                if (player != Main.player[Main.myPlayer] && player.name != "" && player.active)
                {
                    if (!PlayerLabels.ContainsKey(player) && PlayerLabels.All(p => p.Key.name != player.name))
                    {
                        PlayerLabels.Add(player, new UILabel(new Vector2(PlayerLeftOffset, PlayerLeftOffset + PlayerLabels.Count * PlayerListItemHeight),
                            new Vector2(PlayerListWidth, PlayerListItemHeight), player.name, Main.fontItemStack,
                            parent: Window));

                        int iCopy = i;
                        Window.Children.Add(PlayerLabels[player]);
                        PlayerLabels[player].Click += (a, b) =>
                        {
                            if (b.Button == MouseButtons.Left)
                            {
                                CurrentPlayer = iCopy;
                                return true;
                            }

                            return false;
                        };
                    }
                }
            }
            // See if a player left
            foreach (var player in Main.player)
            {
                if (!player.active && player.name != "" && PlayerLabels.ContainsKey(player))
                {
                    Window.Children.Remove(PlayerLabels[player]);
                    PlayerLabels.Remove(player);
                }
            }


            if (confirmationBox != null)
                confirmationBox.Update();
            Window.Update();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (confirmationBox != null)
                confirmationBox.Draw(spriteBatch);
            Window.Draw(spriteBatch);
        }
    }
}
