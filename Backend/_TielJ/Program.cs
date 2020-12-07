using System;
using System.IO;
using _TielJ.Player.Crawln_Grab;
using NAudio.Wave;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Globalization;

namespace _TielJ {

    class Program {       
        static void Main(string[] args) {
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            if (args.Length < 1) throw new Exception("Please specify a starting video url");
            if(args.Length > 1) {
                args[1].Replace(',', '.');
                if (float.Parse(args[1]) > 7.2) throw new Exception("Please lower your in-game sensitivity below 7.3, refer to https://github.com/LollerMann/TielJ for limitations");
                string[] numberseven = args[1].Split('.');
                if (numberseven.Length > 1) {
                    args[1] = $"{args[1].Replace(".", "")}{new String('0', 2 - numberseven[1].Length)}";
                } else args[1] = $"{args[2]}00";
                OWStuff.MouseSens = short.Parse(args[1],style);
            }
            /*List<byte[]> pss = OWStuff.MakeItLeet("720");
            Console.Write('[');
            foreach(byte[] barr in pss) {
                foreach(byte bb in barr) {
                    Console.Write(bb);
                    Console.Write(',');
                }
            }
            Console.Write(']');*/
            Console.WriteLine("TielJ will continue execution in 3 seconds");
            //Thread.Sleep(5000);
            //Tests.PlayerTest();
            //Tests.TestMouse();

            new TielJ(args[0]);
            Console.ReadKey();
        }
    }
}
