﻿using System;
using System.IO;
using _TielJ.Player.Crawln_Grab;
using NAudio.Wave;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace _TielJ {

    class Program {       
        static void Main(string[] args) {
            if (args.Length < 1) throw new Exception("Please specify a starting video url");
            /*List<byte[]> pss = OWStuff.MakeItLeet("DecodeMe");
            Console.Write('[');
            foreach(byte[] barr in pss) {
                foreach(byte bb in barr) {
                    Console.Write(bb);
                    //Console.Write(',');
                }
            }
            Console.Write(']');*/
            Console.WriteLine("TielJ will continue execution in 10 seconds");
            Thread.Sleep(10000);
            new TielJ(args[0]);
            Console.ReadKey();
        }
    }
}
