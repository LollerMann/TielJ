using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TielJ.Player.Crawln_Grab {


    public struct musicInfo {
        public string name;
        public string artist;
        public string url;
        public string audiourl;
        public uint length;
    }
    struct audioInfo {
        public uint contentLength;
        public uint approximateLength;
        public int bitrate;
        public Stream serverAudioStream;
        public bufferedStream bufferedStream;
        public string RefUrl;
    }
    static public class values {
        public static readonly string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:82.0) Gecko/20100101 Firefox/82.0"; //I don't know how to have partial functions and istances
    }
    interface BaseClass {
        musicInfo[] GetSimilar(); //I don't know why but this is the only single common method that comes to my mind
        //musicInfo GetMusicInfo();
        audioInfo getAudioInfo();
        musicInfo getCurrentMusicInfo();
        void Initialize(string url);
        void Dispose();
    }
}
