using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Threading.Tasks;

namespace _TielJ.PROTOTYPES.Player.Crawln_Grab {
    class Bufstr_yt {
        public byte[] buffered;
        Stream stream;
        long contentlength;
        uint bitrate;
        int bufferedcount = -1;
        int totalbuffered = 0;
        MemoryStream memstr;
        string url;
        bool loaded = false;
        public ushort formatid;
        int samplerate;
        int channels;
        BufferedWaveProvider bufferedWaveProvider;
        public int approxlen = 0;
        long memstrpos;

        int count = 0;
        public Bufstr_yt(dynamic streamingData,string url) {
            dynamic chosenone = streamingData.Last;
            this.formatid = chosenone["itag"];
            //this.contentlength = chosenone["contentLength"];
            this.bitrate = chosenone["bitrate"];
            this.approxlen = (Convert.ToInt32(chosenone["approxDurationMs"].Value) / 1000);
            this.contentlength = (long)(((bitrate /8) * approxlen)*1.5);
            
            this.samplerate = chosenone["audioSampleRate"];
            this.channels = chosenone["audioChannels"];
            buffered = new byte[contentlength];
            this.url = url;
            bufferedcount = 0;
        }

        public (int,int) getAudioData() {
            return (samplerate, channels);
        }
        public void setReadStream(Stream str) {
            stream = str;
        }
        public void setWriteStream(BufferedWaveProvider bufferedWave) {
            bufferedWaveProvider = bufferedWave;
            bufferedWaveProvider.BufferLength = (int)contentlength;
            //jumpstart();
            readToStream();
        }
        public void readToStream() {
            int prevbufcount = totalbuffered;
            //if (bufferedcount == 0) jumpstart();
            uint piss = (20 * (this.bitrate / 8));
            while (stream.CanRead && prevbufcount + piss > totalbuffered && bufferedcount != 0) { //read around 30 seconds of content;
                Console.Write($"Caching {prevbufcount + piss}/{totalbuffered}\r");
                byte[] buf = new byte[1024];
                bufferedcount = stream.Read(buf, 0, 1024);
                memstr.Position = memstrpos;
                totalbuffered += bufferedcount;
                memstr.Write(buf, 0, bufferedcount);
                memstrpos = memstr.Position;
                //bufferedWaveProvider.AddSamples(buf, 0,bufferedcount);
            }
            



            /*do {
                byte[] buf = new byte[4096];
                count = stream.Read(buf, 0, 4096);
                memstr.Write(buf, 0, count);
            } while (stream.CanRead && count > 0);*/
        }
        private void jumpstart() {
            //int prevbufcount = -1;
            Console.WriteLine($"Jumpstarting");
             byte[] buf = new byte[1024];
             bufferedcount = stream.Read(buf, 0, 1024);
             totalbuffered += bufferedcount;
             memstr.Position = 0;
             memstr.Write(buf, 0, bufferedcount);
            memstrpos = memstr.Position;
            /*int letotal = 0;
            while (letotal < (30 * (this.bitrate / 8))) {
                byte[] buf = new byte[4096];
                count = stream.Read(buf, 0, 4096);
                memstr.Write(buf, 0, count);
                letotal += count;
            }*/
            //bufferedWaveProvider.AddSamples(buf, prevbufcount + 1, (int)(20 * (this.bitrate / 8)));
        }
        public MemoryStream getStream() {
            this.memstr = new MemoryStream();
            this.memstr.SetLength(contentlength);
            Fill(memstr, 255, (int)contentlength);
            memstr.Position = 0;
            jumpstart();
            readToStream();
            return memstr;
        }
        static void Fill(Stream stream, byte value, int count) {
            var buffer = new byte[64];
            for (int i = 0; i < buffer.Length; i++) {
                buffer[i] = value;
            }
            while (count > buffer.Length) {
                stream.Write(buffer, 0, buffer.Length);
                count -= buffer.Length;
            }
            stream.Write(buffer, 0, count);
        }

    }


}
