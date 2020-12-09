using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace _TielJ
{
    public static class Tests
    {
        [DllImport("MARSELO.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void MoveVader(int x,int y);

        [DllImport("MARSELO.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void ToggleMouseAccel();
        [DllImport("MARSELO.dll", CallingConvention = CallingConvention.Cdecl)]

        static extern bool GetMouseAccel(); 
        public static void PlayerTest()
        {
            Player.Player.Initialize();
            Player.Player.Play("https://www.youtube.com/watch?v=HBpXtVtfaWE");
            Thread.Sleep(10000);
        }
        private static int cocainer(int stuff) { return (int)(stuff * 1.32); }

        private static int whatpercentof(double i1, double i2) { return (int)((100 * i2) / i1); }
        private static double magiknumber = 47.4;
        public static void TestMouse() {

            int x = KeyManager.GetCursorPosition().X;
            int y = KeyManager.GetCursorPosition().Y;
            //double sens = 7.80;
            bool maccelon = false;
            if (GetMouseAccel()) {
                maccelon = true;
                ToggleMouseAccel();
                if (GetMouseAccel()) throw new Exception("It don't work");
                Console.WriteLine("Maccel was on");
            }
            Console.WriteLine($"{x},{y}");
            //int magicx = 29 - (int)(29 / sens);
            //KeyManager.SetCursorPosition(x, y+10);
            /*for(int i = 0; i < 1080 / 2; i++) {
                MoveVader(0, i);
                System.Threading.Thread.Sleep(100);
                if(i != 539)KeyManager.SendKey(OWStuff.GetGameKey(OWStuff.gameKeys.Interact));
                System.Threading.Thread.Sleep(100);
            }*/
            MoveVader(0, whatpercentof(magiknumber,20d));//Move send 128 input
            //MoveVader(0, 267);//Move send 128 input
            x = KeyManager.GetCursorPosition().X;
            y = KeyManager.GetCursorPosition().Y;
            Console.WriteLine($"{x},{y}");
            if (maccelon) ToggleMouseAccel();
            //MoveVader(29, 9);//Move by 100 pixels right;
            Console.WriteLine("Done");
        }
    }
}
  