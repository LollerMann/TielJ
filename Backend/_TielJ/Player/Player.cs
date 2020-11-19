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
        static Dictionary<string,Type> supportedSites = new Dictionary<string,Type>(){ //I only plan to support youtube and maybe soundcloud.
            { "youtube.com",typeof(youtube) }
        };

        static WaveOut WaveOut;
        static BaseClass Agent;
        static audioInfo current;

        public static musicInfo MusicInfo {
            get {
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
        static int seconds =0;
        public static void InitUrl(string url) {
            seconds = 0;
            if (Agent != null) Agent.Dispose();
            if (current.bufferedStream != null) current.bufferedStream.Dispose();
            Match sitename = Regex.Match(url, @"\.((.*?)\..*?)/");
            if (!supportedSites.ContainsKey(sitename.Groups[1].Value)) throw new NotImplementedException($"TielJ does not support this website yet! {sitename.Groups[2].Value}");
            Agent = (BaseClass)Activator.CreateInstance(supportedSites[sitename.Groups[1].Value]);
            Agent.Initialize(url);
            current = Agent.getAudioInfo();
        }
        public static void Play(string url = "") { 
            if (url != "") {
                InitUrl(url);
                current.bufferedStream = new bufferedStream(current);
                Stream piss = current.bufferedStream.getStream();
                StreamMediaFoundationReader streamRead = new StreamMediaFoundationReader(piss, new MediaFoundationReader.MediaFoundationReaderSettings() { SingleReaderObject = true });
                WaveOut.Init(streamRead);
            }
            WaveOut.Play();
        }

        public static void Tick() {
            if(WaveOut.PlaybackState == PlaybackState.Playing) {
                seconds++;
                if (seconds % 30 > 20) {
                    current.bufferedStream.readToStream();
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
