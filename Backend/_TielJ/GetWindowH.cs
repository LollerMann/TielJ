using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _TielJ {
    public static class GetWindowH {
        public static byte[] ImageToByte2(Bitmap img) {
            using (var stream = new MemoryStream()) {
                img.Save(stream,ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public static IntPtr getproc(string wname) {
            Process[] processes = Process.GetProcessesByName(wname);

            foreach (Process p in processes) {
                return p.MainWindowHandle;
            }
            return IntPtr.Zero;
        }

    }
}
