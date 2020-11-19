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
using _TielJ.Player.Crawln_Grab;

namespace _TielJ.PROTOTYPES.Player.Crawln_Grab {
    class youtube : BaseClass {
        private WebRequest webRequest;
        public Tuple<musicInfo, musicInfo[]> GetSimilar(musicInfo musicInfo) {
            throw new NotImplementedException();
        }

        public void Initialize() {
            // Create a request for the URL.
            WebRequest request = WebRequest.Create(
              "https://youtube.com/watch?v=WowSoFun");
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
                pissoff = pissoff.Replace(@"\\",@"\");
                pissoff = pissoff.Replace(@"\/", @"/");
                dynamic stuff = JObject.Parse(pissoff);
                dynamic formats = stuff["player_response"]["streamingData"]["adaptiveFormats"];
                string audiourl = formats[formats.Count - 1]["signatureCipher"];
                string signature = Regex.Match(audiourl, "s=.+&sp").Value.Substring(2);
                signature = System.Uri.UnescapeDataString(signature.Substring(0, signature.Length - 3));
                audiourl = Regex.Match(audiourl, "(https.+)").Value;
                audiourl = System.Uri.UnescapeDataString(audiourl);
                response.Close();
                string playerurl = Regex.Match(responseFromServer, @"<script[^>]+\bsrc=(""[^""]+"")[^>]+\bname=[""\\']player_ias/base").Value; //I yanked this straight from youtube-dl's source. Props to the fella who let their household pet run over their keyboard to produce that pattern.
                playerurl = Regex.Match(playerurl, @"src.+js").Value.Split('"')[1];//Am I edgy doe? Am I quirky doe?
                string playerpage = new StreamReader(WebRequest.Create("https://youtube.com" + playerurl).GetResponse().GetResponseStream()).ReadToEnd();

                string funcstart = Regex.Match(playerpage, @"(?:\b|[^a-zA-Z0-9$])(?<sig>[a-zA-Z0-9$]{2})\s*=\s*function\(\s*a\s*\)\s*{\s*a\s*=\s*a\.split\(\s*""""\s*\)").Value;
                string func = Regex.Match(playerpage, funcstart.Trim().Replace("(","\\(").Replace(")", "\\)") + @".+};").Value;
                string funcname = func.Substring(0, 2);
                ExtractFunc(funcname, playerpage);
                /*engineer.Execute(@"document=""pssoff""");
                engineer.Execute(@"location=""pssoff""");
                engineer.Execute(playerpage.Replace("Uint8Array", "Array"));*/
                //engineer.Execute(func);
                //var obj = engineer.Execute($@"{funcname}(""{signature}"")");

                int vacIndex =-69;
                for (int i = 0; i < WaveOut.DeviceCount; i++) {
                    WaveOutCapabilities cap = WaveOut.GetCapabilities(i);
                    if (cap.ProductName.Contains("VB-Audio Virtual")) vacIndex = i;
                }
                Console.WriteLine(audiourl);
                if (vacIndex == -69) throw new Exception("Couldn't find virtual audio cable device. Do you have it installed?");
                WaveOut waveOut = new WaveOut();
                waveOut.DeviceNumber = vacIndex;
                
                /*NAudio.Wave.StreamMediaFoundationReader streamRead = new StreamMediaFoundationReader(WebRequest.Create(audiourl).GetResponse().GetResponseStream());
                waveOut.Init(streamRead);
                waveOut.Play();*/
                

                Console.WriteLine("kkk");

                //Console.WriteLine(responseFromServer);
                using (FileStream fs = new FileStream("homofunny.funnymemes", FileMode.Create)) {
                    byte[] bruhfunny = Encoding.UTF8.GetBytes(responseFromServer);
                    fs.Write(bruhfunny, 0, bruhfunny.Length);
                }
                using (FileStream fs = new FileStream("hazbinfunny.funnymemes", FileMode.OpenOrCreate)) {
                    byte[] bruhfunny = Encoding.UTF8.GetBytes(Regex.Match(responseFromServer, @"{""player_response.*?}}}]}""}").Captures[0].Value);
                    fs.Write(bruhfunny, 0, bruhfunny.Length);
                }
                using (FileStream fs = new FileStream("bruhfunny.funnymemes", FileMode.OpenOrCreate)) {
                    byte[] bruhfunny = Encoding.UTF8.GetBytes(pissoff);
                    fs.Write(bruhfunny,0,bruhfunny.Length);
                }
                // Close the response.
            }
        }
        private int piss = 0;
        void ExtractFunc(string funcname,string wholepage,int argc = -1) {
            string leregex;
            string fuck = "{.+}";
            if (argc != -1) {
                leregex = $@"{funcname}=function\(";
                for (int i = 97; i < 97 + argc; i++) {
                    leregex += (char)i;
                    if (i != 96 + argc) leregex += ',';
                    else leregex += @"\)";
                }
                leregex += @"{.+};";
            }
            else leregex = String.Format(@"{0}=.+{1};", funcname, fuck);
            string extractedfunc = Regex.Match(wholepage, leregex).Value;
            if (extractedfunc == "") {
                if (argc != -1) {
                    leregex = $@"{funcname}:function\(";
                    for (int i = 97; i < 97 + argc; i++) {
                        leregex += (char)i;
                        if (i != 96 + argc) leregex += ',';
                        else leregex += @"\)";
                    }
                    leregex += @"{.+};";
                }
                else leregex = String.Format(@"{0}:.+{1};", funcname, fuck);
            }
            extractedfunc = Regex.Match(wholepage, leregex).Value;
            if (extractedfunc == "") Console.WriteLine("Didn't work");
                MatchCollection matches = Regex.Matches(extractedfunc, @"\w+(\((\w,)+|(\w)\))\w+\)"); //get sub functions
            if(matches.Count > 0) {
                foreach(Match match in matches) {
                    string matchstr = match.Value;
                    /*string classmethod = Regex.Match(match, @"\w+\.\w+").Value;
                    if(classmethod.Length == 0)throw new Exception("This function does not have a class")*/
                    int argcount = matchstr.Split(',').Length;
                    if(!matchstr.Contains("function")) { //I'm no pro regexer
                        string jsfuncname = matchstr.Substring(0, 2);
                        ExtractFunc(jsfuncname, wholepage, argcount); //I hate albania
                    }

                }
            }
            extractedfunc = Regex.Replace(extractedfunc, @"(\w+\.(\w+))", "$2");
            piss++;
            Console.WriteLine(piss);
            if (extractedfunc.Substring(0, 4).Contains(":")) {
                extractedfunc = extractedfunc.Replace(':', '=').Substring(0, extractedfunc.Length - 2);
            }
            //engine.Execute(extractedfunc.Replace(':','='));
        }

        audioInfo BaseClass.getAudioInfo() {
            throw new NotImplementedException();
        }

        void BaseClass.Initialize(string url) {
            throw new NotImplementedException();
        }

        public musicInfo[] GetSimilar() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        musicInfo BaseClass.getCurrentMusicInfo() {
            throw new NotImplementedException();
        }
    }
}
