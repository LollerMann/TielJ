using System;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using NAudio.Wave;
using System.Diagnostics;
using System.Threading;
using _TielJ.Player.Crawln_Grab;

namespace _TielJ.PROTOTYPES.Player.Crawln_Grab {
    class ytt : BaseClass {
        private WebRequest webRequest;
        public Tuple<musicInfo, musicInfo[]> GetSimilar(musicInfo musicInfo) {
            throw new NotImplementedException();
        }
        static void pissoff(object arg) {
            (byte[], MemoryStream,Stream) args = ((byte[],MemoryStream,Stream))arg;
            int count = 0;
            do {
                byte[] buf = new byte[4096];
                count = args.Item3.Read(buf, 0, 4096);
                args.Item2.Write(buf, 0, count);
            } while (args.Item3.CanRead && count > 0);
            args.Item1 = args.Item2.ToArray();
        }
        public void Initialize() {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C youtube-dl --get-url --print-traffic -f 249 https://youtube.com/watch?v=SuperDuperFunUrl";
            process.StartInfo = startInfo;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit();
            string hsc = "header: Set-Cookie:";
            WebRequest req = HttpWebRequest.Create("http://www.retardedispsayswhat.net");
            string youtubedlres = process.StandardOutput.ReadToEnd();
            foreach (string line in youtubedlres.Split('\n')) {
                if (line.StartsWith("https")) {
                    req = HttpWebRequest.Create(line);
                }
            }
            HttpWebRequest httpreq = (HttpWebRequest)req;
            httpreq.CookieContainer = new CookieContainer();
            foreach (string line in youtubedlres.Split('\n')) {
                if(line.Contains(hsc)) {
                    string[] jar = line.Substring(hsc.Length - 1).Split(' ');
                    foreach (string cookie in jar) {
                        if(cookie.Length > 2) {
                            string[] cokie = cookie.Split('=');
                            if (cokie.Length > 1 && cokie[0] != "expires") {
                                Cookie cock = new Cookie(cokie[0], (cokie[1].EndsWith(";")) ? cokie[1].Substring(0, cokie[1].Length - 2) : cokie[1]);
                                cock.Domain = "youtube.com";
                                httpreq.CookieContainer.Add(cock);
                            }
                            
                        }
                    }
                }
            }
            int vacIndex = -69;
            for (int i = 0; i < WaveOut.DeviceCount; i++) {
                WaveOutCapabilities cap = WaveOut.GetCapabilities(i);
                if (cap.ProductName.Contains("VB-Audio Virtual")) vacIndex = i;
            }
            if (vacIndex == -69) throw new Exception("Couldn't find virtual audio cable device. Do you have it installed?");
            WaveOut waveOut = new WaveOut();
            waveOut.DeviceNumber = vacIndex;
            WebResponse response = httpreq.GetResponse();
            Stream stream = response.GetResponseStream();
            long len = response.ContentLength;
            byte[] b = new byte[len];
            byte[] p = null;
            Console.WriteLine("Caching shit");
            MemoryStream ns = new MemoryStream();
            //new Thread(new ParameterizedThreadStart(pissoff)).Start((object)(b, ms,stream));
            int count = 0;
            do {
                byte[] buf = new byte[4096];
                count = stream.Read(buf, 0, 4096);
                ns.Write(buf, 0, count);
            } while (stream.CanRead && count > 0);
            p = ns.ToArray();
            Console.WriteLine("playing it");
            MemoryStream ms = new MemoryStream(b);
            bool fookinequal = ms.ToArray().SequenceEqual(ns.ToArray());
            StreamMediaFoundationReader streamread = new StreamMediaFoundationReader(ns);
            waveOut.Init(streamread);
            waveOut.Play();
            while(waveOut.PlaybackState == PlaybackState.Playing) {
                Thread.Sleep(1000);
            }
        }

        musicInfo[] BaseClass.GetSimilar() {
            throw new NotImplementedException();
        }

        audioInfo BaseClass.getAudioInfo() {
            throw new NotImplementedException();
        }

        void BaseClass.Initialize(string url) {
            throw new NotImplementedException();
        }

        void BaseClass.Dispose() {
            throw new NotImplementedException();
        }

        musicInfo BaseClass.getCurrentMusicInfo() {
            throw new NotImplementedException();
        }
    }
}
