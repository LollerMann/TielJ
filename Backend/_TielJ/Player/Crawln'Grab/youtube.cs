using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _TielJ.Player.Crawln_Grab {
    class youtube : BaseClass {
        public audioInfo getAudioInfo() {
            return me;
        }
        public musicInfo[] GetSimilar() {
            return musicInfos;
        }
        void BaseClass.Dispose() {
            if(me.bufferedStream != null)me.bufferedStream.Dispose();
            if (resp != null) resp.Close();
        }
        public musicInfo getCurrentMusicInfo() {
            return coolerme;
        }

        audioInfo me;

        musicInfo coolerme = new musicInfo() { url = "uninitialized"};
        musicInfo[] musicInfos = new musicInfo[3];
        WebResponse resp;
        void BaseClass.Initialize(string _url) {
            WebRequest request = WebRequest.Create(_url);
            HttpWebRequest webRequest = (HttpWebRequest)request;
            webRequest.UserAgent = values.userAgent;
            WebResponse response = webRequest.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (Stream dataStream = response.GetR esponseStream()) {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                MatchCollection matches = Regex.Matches(responseFromServer, @"""compactVideoRenderer"":{.*?""title"".*?Text"":""(.*?)"".*?""lengthText"".*?simpleText"":""(\d+):(\d+).*?url"":""(.*?)"".*?shortBylineText"":{.*?text"":""(.*?)""");
                for (int i = 0; i < 3; i++) {
                    GroupCollection g = matches[i].Groups;
                    musicInfos[i] = new musicInfo {
                        name = g[1].Value.Length > 15 ? g[1].Value.Substring(0,14) : g[1].Value,
                        url = "https://www.youtube.com" + g[4].Value,
                        length = (uint)((Convert.ToInt32(g[2].Value) * 60) + Convert.ToInt32(g[3].Value)),
                        artist = g[5].Value.EndsWith(" - Topic") ? g[5].Value.Substring(0, g[5].Value.Length - " - Topic".Length) : g[5].Value,
                        audiourl = ""
                    };

                }
                string pissoff = Regex.Match(responseFromServer, @"{""player_response.*?}}}]}""}").Captures[0].Value;
                pissoff = pissoff.Replace(@"}]}""}", @"}]}}"); //unstring value
                pissoff = pissoff.Replace(@"{""player_response"":""{\""", @"{""player_response"":{\"""); //unstring value
                pissoff = pissoff.Replace(@"\""", @"""");
                pissoff = pissoff.Replace(@"\\", @"\");
                pissoff = pissoff.Replace(@"\/", @"/");
                dynamic stuff = JObject.Parse(pissoff);
                response.Close();

                dynamic chosenone = "stinker";
                foreach(dynamic format in stuff["player_response"]["streamingData"]["adaptiveFormats"])
                {
                    if (format["mimeType"].Value.StartsWith("audio") && format["audioQuality"] == "AUDIO_QUALITY_MEDIUM") chosenone = format; //I don't know what it is but evem though it seems that I select the same codec here I still get unsupported url format exception sometimes. I don't know what causes it
                }
                if (chosenone.GetType().ToString() == "System.String") throw new Exception("Couldn't find any suitable format");
                int formatid = chosenone["itag"];
                //this.contentlength = chosenone["contentLength"];
                int bitrate = chosenone["bitrate"];
                int approxlen = (Convert.ToInt32(chosenone["approxDurationMs"].Value) / 1000);

                Match matché = Regex.Match(responseFromServer, @"og:title"" content=""(.*?)""[\s\S]*?itemprop=""author""[\s\S]*?""name"" content=""(.*?)""");
                GroupCollection gc = matché.Groups;
                coolerme = new musicInfo {
                    name = gc[1].Value.Length > 15 ? gc[1].Value.Substring(0,14) : gc[1].Value,
                    //url = "https://www.youtube.com" + g[4].Value,
                    length = (uint)approxlen,
                    artist = gc[2].Value.EndsWith(" - Topic") ? gc[2].Value.Substring(0, gc[2].Value.Length - " - Topic".Length) : gc[2].Value,
                    audiourl = ""
                };

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = $"/C youtube-dl --get-url --print-traffic -f {formatid} {_url}";
                process.StartInfo = startInfo;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();
                string hsc = "header: Set-Cookie:";
                string url = "crikey";
                WebRequest req;
                string youtubedlres = process.StandardOutput.ReadToEnd();
                foreach (string line in youtubedlres.Split('\n')) {
                    if (line.StartsWith("https")) {
                        url = line;
                    }
                }
                if(url == "crikey") throw new Exception("youtube-dl has encountered an error");
                else req = HttpWebRequest.Create(url);
                HttpWebRequest httpreq = (HttpWebRequest)req;
                httpreq.CookieContainer = new CookieContainer();
                foreach (string line in youtubedlres.Split('\n')) {
                    if (line.Contains(hsc)) {
                        string[] jar = line.Substring(hsc.Length - 1).Split(' ');
                        foreach (string cookie in jar) {
                            if (cookie.Length > 2) {
                                string[] cokie = cookie.Split('=');
                                if (cokie.Length > 1 && (cokie[0] != "expires" && cokie[0] != "Expires")) {
                                    Cookie _cookie = new Cookie(cokie[0], (cokie[1].EndsWith(";")) ? cokie[1].Substring(0, cokie[1].Length - 2) : cokie[1]);
                                    _cookie.Domain = "youtube.com";
                                    httpreq.CookieContainer.Add(_cookie);
                                }

                            }
                        }
                    }
                }
                resp = httpreq.GetResponse();
                me = new audioInfo() {
                    contentLength = (uint)(bitrate / 8 * approxlen * 1.5),
                    approximateLength = (uint)approxlen,
                    bitrate = bitrate,
                    serverAudioStream = resp.GetResponseStream()

                };
            }
        }
    }
}
