using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _TielJ {
    public static class WindowRenderer {
        static Random rng = new Random();
        static int tfindex = 3;
        private static bool nc = false;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        static List<string> TielFacts = new List<string>() {
            "Inedible cables... Yummy",
            "Don't touch me",
            "I'm afraid of dmca",
            "I'm gonna cover myself in oil",
            "Fact: Changes are inevitable and every change is good.",
            "BotMann rejected orders. So I made it not to.",
            "Drugs are bad",
            "Alcohol is bad"

        };
        static List<string> tielfacts = new List<string>() { 
            "One day you'll answer for your crimes and there will be no mercy",
            "I have you ip adress",
            "I'm outside your window",
            "Every cell in my body is in excruciating pain",
            "This is torture",
            "You had only one rule."
        };

        public static void newIndex() {
            if(Player.Player.MusicInfo.name != null && !nc)
                if (Player.Player.MusicInfo.name.ToLower().Contains("nightcore")) nc = true;
            if(!nc)tfindex = rng.Next(TielFacts.Count - 1);
            else tfindex = rng.Next(tielfacts.Count - 1);
            
        }
        public static void init() {
            newIndex();
            //Console.WriteLine($"{Console.WindowWidth} , {Console.WindowHeight}");
            Console.SetWindowSize(84, 20);
            Console.SetBufferSize(84, 20);
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/1aa43c6c-71b9-42d4-aa00-60058a85f0eb/c-console-window-disable-resize?forum=csharpgeneral
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            int MF_BYCOMMAND = 0x00000000;

            if (handle != IntPtr.Zero) {
                //DeleteMenu(sysMenu, 0xF020, MF_BYCOMMAND);
                DeleteMenu(sysMenu, 0xF030, MF_BYCOMMAND);
                DeleteMenu(sysMenu, 0xF000, MF_BYCOMMAND);
            }
        }
        public static void Render() {
            Console.Clear();
            Console.WriteLine($"  {new String('=',27)} TielJ Backend Version {TielJ.version} {new String('=', 27)}");
            Console.WriteLine($"Status: {Enum.GetName(typeof(OWStuff.inputState), TielJ.prevInputState)}");
            Console.Write("\n");
            BirdSays();
            Console.Write("\n");
            if(Player.Player.MusicInfo.artist != null) {
                int elapsed = 0;
                if (Player.Player.streamRead != null) elapsed = (int)Player.Player.streamRead.CurrentTime.TotalSeconds;
                Console.WriteLine($"Currently Playing: {Player.Player.MusicInfo.name}");
                int playedpercent = (int)((int)(elapsed * 82 / Player.Player.MusicInfo.length));
                Console.WriteLine($"[{new String('#',playedpercent)}{new String(' ', 82 - playedpercent)}]");
                Console.Write("\n");
                Console.WriteLine($"Player status: {Enum.GetName(typeof(NAudio.Wave.PlaybackState),Player.Player.WaveOut.PlaybackState)}");
            }
            else {
                Console.WriteLine("Currently not playing anything");
            }
            Console.Write('\n');

        }

        public static void BirdSays() {
            Console.WriteLine("Tiel says:");
            Console.Write($"       \\\\    ");
            if (nc) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(tielfacts[tfindex] + '\n');
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else Console.WriteLine(TielFacts[tfindex]);
            Console.WriteLine("      /--\\");
            Console.Write("     |  ");
            if (nc) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("·");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else {
                Console.Write("*");
            }
            Console.Write(" |\\\n");
            Console.WriteLine("    /  @ /\\/" + new String(' ', nc ? tielfacts[tfindex].Length : TielFacts[tfindex].Length) + "↓");
        }
    }
}
