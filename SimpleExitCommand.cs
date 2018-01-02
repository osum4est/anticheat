using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SimpleTools
{
    class SimpleExitCommand : ModCommand
    {
        public override CommandType Type
        {
            get { return CommandType.Console; }
        }

        public override string Command
        {
            get { return "exitac"; }
        }

        // TODO: WTF -config serverconfig.txt
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            foreach (var player in Terraria.Main.player.Where(p => p.name != ""))
            {
                AntiCheat.PlayerDisconnected(player);
            }

            PostMessage(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, 0x100, 0x45, 0);
            PostMessage(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, 0x100, 0x58, 0);
            PostMessage(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, 0x100, 0x49, 0);
            PostMessage(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, 0x100, 0x54, 0);
            PostMessage(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, 0x100, 0x0D, 0);
        }

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;

        static void Main(string[] args)
        {
            Console.Write("Switch focus to another window now to verify this works in a background process.\n");

            ThreadPool.QueueUserWorkItem((o) =>
            {
                Thread.Sleep(4000);

                var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0);
            });

            Console.ReadLine();

            Console.Write("ReadLine() successfully aborted by background thread.\n");
            Console.Write("[any key to exit]");
            Console.ReadKey();
        }
    }
}
