using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Terraria;
using Terraria.ModLoader.IO;

namespace SimpleTools
{
    class AntiCheat
    {
        private const string file = "anticheatdata.dat";
        public static void PlayerConnected(Player player)
        {
            Log(player.name, "Checking inventory...");
            PlayerJson playerJson = GetInventory(LoadJson(), player);
            if (playerJson == null)
            {
                LogN(player.name, "First connection to server. Ignoring.");
            }
            else
            {
                bool modified = Diff(player.name, "Inventory", playerJson.inventory, player.inventory) | 
                                Diff(player.name, "Armor", playerJson.armor, player.armor) | 
                                Diff(player.name, "Armor Dye", playerJson.dye, player.dye) |
                                Diff(player.name, "Misc Equip", playerJson.miscEquips, player.miscEquips) |
                                Diff(player.name, "Misc Equip Dye", playerJson.miscDyes, player.miscDyes);

                if (playerJson.maxHealth != player.statLifeMax)
                {
                    LogE(player.name, "Max Health", playerJson.maxHealth.ToString(), player.statLifeMax.ToString());
                    modified = true;
                }

                if (playerJson.maxMana != player.statManaMax)
                {
                    LogE(player.name, "Max Mana", playerJson.maxMana.ToString(), player.statManaMax.ToString());
                    modified = true;
                }

                if (!modified)
                    LogS(player.name, "No cheating detected :)");
                else
                    LogR(player.name, "Cheating detected :(");
            }
        }

        private static bool Diff(string playername, string category, List<InventoryItem> saved, Item[] current)
        {
            bool modified = false;
            for (int i = 0; i < current.Length; i++)
            {
                if (saved[i].name != current[i].Name ||
                    saved[i].amount != current[i].stack)
                {
                    LogE(playername, category + "[" + i + "]", saved[i].name + "(" + saved[i].amount + ")",
                         current[i].Name + "(" + current[i].stack + ")");
                    modified = true;
                }
            }

            return modified;
        }

        public static void PlayerDisconnected(Player player)
        {
            Log(player.name, "Disconnected. Saving inventory...");
            PlayerJson playerJson = SaveInventory(player);
            LogN(player.name, "Saved " + playerJson.inventory.Count(i => i.name != "" && i.amount != 0) + " item(s).");
        }

        private static AntiCheatJson LoadJson()
        {
            return File.Exists(file) ? JsonConvert.DeserializeObject<AntiCheatJson>(File.ReadAllText(file)) : new AntiCheatJson { worlds = new List<WorldJson>() };
        }

        private static PlayerJson GetInventory(AntiCheatJson antiCheatJson, Player player)
        {
            bool newPlayer = false;

            if (antiCheatJson.worlds.All(wx => wx.name != Main.worldName))
            {
                antiCheatJson.worlds.Add(new WorldJson { name = Main.worldName, players = new List<PlayerJson>() });
                newPlayer = true;
            }

            if (antiCheatJson.worlds.First(wx => wx.name == Main.worldName).players.All(px => px.name != player.name))
            {
                antiCheatJson.worlds.First(wx => wx.name == Main.worldName).players.Add(new PlayerJson
                {
                    name = player.name,
                    inventory = new List<InventoryItem>(),
                    armor = new List<InventoryItem>(),
                    dye = new List<InventoryItem>(),
                    miscDyes = new List<InventoryItem>(),
                    miscEquips = new List<InventoryItem>(),
                    maxHealth = 0,
                    maxMana = 0
                });
                newPlayer = true;
            }

            if (newPlayer)
            {
                File.WriteAllText(file, JsonConvert.SerializeObject(antiCheatJson));
                return null;
            }

            return antiCheatJson.worlds.First(wx => wx.name == Main.worldName).players
                .First(px => px.name == player.name);

        }

        private static PlayerJson SaveInventory(Player player)
        {
            AntiCheatJson antiCheatJson = LoadJson();
            PlayerJson playerJson = GetInventory(antiCheatJson, player);
            
            SaveItemList(playerJson.inventory, player.inventory);
            SaveItemList(playerJson.armor, player.armor);
            SaveItemList(playerJson.dye, player.dye);
            SaveItemList(playerJson.miscEquips, player.miscEquips);
            SaveItemList(playerJson.miscDyes, player.miscDyes);

            playerJson.maxHealth = player.statLifeMax;
            playerJson.maxMana = player.statManaMax;

            File.WriteAllText(file, JsonConvert.SerializeObject(antiCheatJson));
            return playerJson;
        }

        private static void SaveItemList(List<InventoryItem> to, Item[] from)
        {
            to.Clear();

            foreach (var item in from)
            {
                to.Add(new InventoryItem
                {
                    name = item.Name,
                    amount = item.stack
                });
            }
        }

        private static void Log(string playername, string message)
        {
//            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[AntiCheat - " + playername + "] " + message);
//            Console.ResetColor();
        }

        private static void LogN(string playername, string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[AntiCheat - " + playername + "] " + message);
            Console.ResetColor();
        }

        private static void LogS(string playername, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[AntiCheat - " + playername + "] " + message);
            Console.ResetColor();
        }

        private static void LogR(string playername, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[AntiCheat - " + playername + "] " + message);
            Console.ResetColor();
        }

        private static void LogE(string playername, string property, string from, string to, string nothingString = "(0)")
        {
            Console.Write("[AntiCheat - " + playername + "] " + property + ": ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(from == nothingString ? "" : from + " ");
            Console.ResetColor();
            Console.Write("->");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(to == nothingString ? "" : " " + to);
            Console.ResetColor();
        }

        private class AntiCheatJson
        {
            public List<WorldJson> worlds;
        }

        private class WorldJson
        {
            public string name;
            public List<PlayerJson> players;
        }

        private class PlayerJson
        {
            public string name;
            public List<InventoryItem> inventory;
            public List<InventoryItem> armor;
            public List<InventoryItem> dye;
            public List<InventoryItem> miscEquips;
            public List<InventoryItem> miscDyes;
            public int maxHealth;
            public int maxMana;
        }

        private class InventoryItem
        {
            public string name;
            public int amount;
        }
    }
}
