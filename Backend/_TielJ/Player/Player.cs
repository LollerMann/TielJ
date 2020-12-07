using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NAudio.Wave;
using System.Threading;
using _TielJ.Player.Crawln_Grab;
using System.IO;

namespace _TielJ.Player {
    public static class Player {
        enum websites {
            youtube
        };
        static Dictionary<string, Type> supportedSites = new Dictionary<string, Type>(){ //I only plan to support youtube and maybe soundcloud.
            { "youtube.com",typeof(youtube) }
        };

        public static WaveOut WaveOut;
        static BaseClass Agent;
        static audioInfo current;

        public static musicInfo MusicInfo {
            get {
                if (Agent == null) return new musicInfo() {};
                return Agent.getCurrentMusicInfo();
            }
        }
        public static musicInfo[] getSimilar {
            get {
                return Agent.GetSimilar();
            }
        }
        public static void Initialize() {
            int vacIndex = -69;
            for (int i = 0; i < WaveOut.DeviceCount; i++) {
                WaveOutCapabilities cap = WaveOut.GetCapabilities(i);
                if (cap.ProductName.Contains("VB-Audio Virtual")) vacIndex = i;
            }
            if (vacIndex == -69) throw new Exception("Couldn't find virtual audio cable device. Do you have it installed?");
            WaveOut = new WaveOut();
            WaveOut.DeviceNumber = vacIndex;
        }

        public static string GetMusicUrl(int index) {
            return Agent.GetSimilar()[index].url;
        }
        public static int seconds =0;
        public static void InitUrl(string url) {
            seconds = 0;
            if (Agent != null) Agent.Dispose();
            Agent = null;
            if (current.bufferedStream != null) current.bufferedStream.Dispose();
            Match sitename = Regex.Match(url, @"\.((.*?)\..*?)/");
            if (!supportedSites.ContainsKey(sitename.Groups[1].Value)) throw new NotImplementedException($"TielJ does not support this website yet! {sitename.Groups[2].Value}");
            Agent = (BaseClass)Activator.CreateInstance(supportedSites[sitename.Groups[1].Value]);
            bool initialize = false;
            do
            {
                try
                {
                    Agent.Initialize(url);
                    initialize = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occured during initialization {e.Message}");
                }
            } while (initialize == false);
            current = Agent.getAudioInfo();
        }
        public static StreamMediaFoundationReader streamRead;
        public static void Play(string url = "") { 
            if (url != "") {
                bool initialized = false;
                do {
                    try {
                        InitUrl(url);
                        WindowRenderer.newIndex();
                        current.bufferedStream = new bufferedStream(current);
                        Stream piss = current.bufferedStream.getStream();
                        streamRead = new StreamMediaFoundationReader(piss, new MediaFoundationReader.MediaFoundationReaderSettings() { SingleReaderObject = true });
                        WaveOut.Init(streamRead);
                        initialized = true;
                    }
                    catch (Exception e) {
                        Console.WriteLine($"An error occured during initialization of player: {e.Message}");
                    }
                }
                while (!initialized);
            }
            WaveOut.Play();
        }

        public static void Tick() {

                if (WaveOut.PlaybackState == PlaybackState.Playing)
                {
                    seconds++;
                    if (seconds % 30 > 20)
                    {
                        //current.bufferedStream.readToStream(); //you do the multithread buffering yet you still call this???
                    }
                }
            
        }

        static void Pause() {
            WaveOut.Pause();
        }
        static void Stop() {

        }
    }
}
