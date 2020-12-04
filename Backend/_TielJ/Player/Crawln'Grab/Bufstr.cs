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
        bool paused = false;

        public int ToBeBuffered;

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
        private static bool jumpstart = false;
        private int stinker;
        public void readToStream() {
            while (!abortnite && bufferedcount != 0)
            {
                int prevbufcount = totalbuffered;
                ToBeBuffered = 20 * (this.audioInfo.bitrate / 8) + prevbufcount + 1000;
                try
                {
                    while (stream.CanRead && bufferedcount != 0 && !abortnite && totalbuffered < ToBeBuffered && !paused)
                    {
                        Console.Write($"Caching {totalbuffered}/{ToBeBuffered}\r");
                        byte[] buf = new byte[1024];
                        bufferedcount = stream.Read(buf, 0, 1024);
                        memstr.Position = memstrpos;
                        totalbuffered += bufferedcount;
                        memstr.Write(buf, 0, bufferedcount);
                        memstrpos = memstr.Position;
                    }
                }
                catch (System.IO.IOException)
                {
                    stinker++;
                    Console.WriteLine("Remote closed the connection");
                    Thread.Sleep(2000);//delay 2 seconds
                    if (stinker > 5) abortnite = true;
                    else readToStream();
                }
                if (jumpstart) break;
                Thread.Sleep(10000);
            }
        }

        public MemoryStream getStream() {
            this.memstr = new MemoryStream();
            this.memstr.SetLength(audioInfo.contentLength);
            Fill(memstr, 255, (int)audioInfo.contentLength);
            memstr.Position = 0;
            stinker = 0;
            jumpstart = true;
            readToStream(); //Jumpstart it 
            jumpstart = false;
            BufferThread = new Thread(new ThreadStart(readToStream));
	    BufferThread.Start();
            return memstr;
        }

        public void pause() {
            this.paused = true;
        }
        public void resume() {
            this.paused = false;
        }
        public void stopBuffering() {
            this.abortnite = true;
            this.BufferThread.Join();
        }
        public void startBuffering() {
            abortnite = false;
            this.BufferThread.Start();
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
