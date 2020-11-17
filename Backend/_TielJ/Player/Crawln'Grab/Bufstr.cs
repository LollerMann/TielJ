using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Threading.Tasks;

namespace _TielJ.Player.Crawln_Grab {
    class bufferedStream {
        audioInfo audioInfo;
        Stream stream;
        int bufferedcount = -1;
        int totalbuffered = 0;
        MemoryStream memstr;
        long memstrpos;
        public bufferedStream(audioInfo info) {
            audioInfo = info;
            stream = audioInfo.serverAudioStream;
        }
        public void setReadStream(Stream str) {
            stream = str;
        }

        internal void Close() {
            audioInfo.serverAudioStream.Close();
        }

        public void readToStream() {
            int prevbufcount = totalbuffered;
            //if (bufferedcount == 0) jumpstart();
            uint bytestobeloaded = (uint)(30 * (audioInfo.bitrate / 8));
            while (stream.CanRead && prevbufcount + bytestobeloaded > totalbuffered && bufferedcount != 0) { //read around 30 seconds of content;
                Console.Write($"Caching {prevbufcount + bytestobeloaded}/{totalbuffered}\r");
                byte[] buf = new byte[1024];
                bufferedcount = stream.Read(buf, 0, 1024);
                memstr.Position = memstrpos;
                totalbuffered += bufferedcount;
                memstr.Write(buf, 0, bufferedcount);
                memstrpos = memstr.Position;
            }
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
        }
        public MemoryStream getStream() {
            this.memstr = new MemoryStream();
            this.memstr.SetLength(audioInfo.contentLength);
            Fill(memstr, 255, (int)audioInfo.contentLength);
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
