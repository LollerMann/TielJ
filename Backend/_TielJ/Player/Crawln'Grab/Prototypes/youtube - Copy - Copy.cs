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
    class ytm : BaseClass {
        private WebRequest webRequest;
        public Tuple<musicInfo, musicInfo[]> GetSimilar(musicInfo musicInfo) {
            throw new NotImplementedException();
        }

        dynamic scrapePage(string url) {
            WebRequest request = WebRequest.Create(url);
            HttpWebRequest webRequest = (HttpWebRequest)request;
            webRequest.UserAgent = values.userAgent;
            // Get the response.
            //webRequest.CookieContainer.Add(new Cookie("PREF", "f1=50000000&f6=8&hl=en", "/", ".youtube.com"));//set page lang to en;
            WebResponse response = webRequest.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (Stream dataStream = response.GetResponseStream()) {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                string pissoff = Regex.Match(responseFromServer, @"{""player_response.*?}}}]}""}").Captures[0].Value;
                pissoff = pissoff.Replace(@"}]}""}", @"}]}}"); //unstring value
                pissoff = pissoff.Replace(@"{""player_response"":""{\""", @"{""player_response"":{\"""); //unstring value
                pissoff = pissoff.Replace(@"\""", @"""");
                pissoff = pissoff.Replace(@"\\", @"\");
                pissoff = pissoff.Replace(@"\/", @"/");
                dynamic stuff = JObject.Parse(pissoff);
                response.Close();
                return stuff["player_response"]["streamingData"]["adaptiveFormats"];
                // Close the response.
            }
        }
        public void Initialize() {
            string url = "https://www.youtube.com/watch?v=SuperDuperFunURL";
            Bufstr_yt bufstr = new Bufstr_yt(scrapePage(url),url);
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C youtube-dl --get-url --print-traffic -f {bufstr.formatid} {url}";
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
                            if (cokie.Length > 1 && (cokie[0] != "expires" && cokie[0] != "Expires")) {
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
            //BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(WaveFormat.CreateALawFormat(bufstr.getAudioData().Item1, bufstr.getAudioData().Item2));
            bufstr.setReadStream(httpreq.GetResponse().GetResponseStream());
            //bufstr.setWriteStream(bufferedWaveProvider);
            StreamMediaFoundationReader streamread = new StreamMediaFoundationReader(bufstr.getStream(), new MediaFoundationReader.MediaFoundationReaderSettings() { SingleReaderObject = true });
            waveOut.Init(streamread);
            waveOut.Play();
            int secks = 0;
            while(secks < bufstr.approxlen) {
                while (waveOut.PlaybackState == PlaybackState.Playing) {
                    secks++;
                    Thread.Sleep(1000);
                    //bufstr.readToStream();
                    if (secks % 30 > 10) {
                        bufstr.readToStream();
                    }
                    //if (waveOut.PlaybackState == PlaybackState.Stopped) waveOut.Play();
                }
                waveOut.Play();
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
