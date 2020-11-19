using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Threading.Tasks;
using System.Threading;

namespace _TielJ.Player.Crawln_Grab {
    class bufferedStream {
        audioInfo audioInfo;
        Stream stream;
        int bufferedcount = -1;
        int totalbuffered = 0;
        MemoryStream memstr;
        long memstrpos;
        Thread BufferThread;
        bool abortnite = false;
        public bufferedStream(audioInfo info) {
            audioInfo = info;
            stream = audioInfo.serverAudioStream;
        }
        public void setReadStream(Stream str) {
            stream = str;
        }

        public void Dispose(){
            audioInfo.serverAudioStream.Close();
            abortnite = true;
        }

        public void readToStream() {
            int prevbufcount = totalbuffered;
            do{
                Console.Write($"Caching {totalbuffered}\r");
                byte[] buf = new byte[1024];
                bufferedcount = stream.Read(buf, 0, 1024);
                memstr.Position = memstrpos;
                totalbuffered += bufferedcount;
                memstr.Write(buf, 0, bufferedcount);
                memstrpos = memstr.Position;
            }
            while (stream.CanRead && bufferedcount != 0 && !abortnite);
        }

        public MemoryStream getStream() {
            this.memstr = new MemoryStream();
            this.memstr.SetLength(audioInfo.contentLength);
            Fill(memstr, 255, (int)audioInfo.contentLength);
            memstr.Position = 0;
            BufferThread = new Thread(new ThreadStart(readToStream));
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
