using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _TielJ {
    public static class OWStuff {
        public static short MouseSens = -60;
        public enum e_HUDLoc {
            topLeft,
            middle,
            middle2,
            topRight
        }
        public struct pixPos {
            public int x;
            public int y;
        }
        public enum colornames {
            UNRECOGNIZED,
            White,
            Purple,
            Lime,
            Orang,
            Blu,
            Red,
            Green,
            Yellow
        }
        public struct color {
            public byte R;
            public byte G;
            public byte B;
            bool Equals(Color color) {
                if (color.R == R && color.G == G && color.B == B) return true;
                return false;
            }
            public static colornames GetName(Color color) {
                color temp = new color() {
                    R = color.R,
                    G = color.G,
                    B = color.B
                };
                if (Colors.ContainsKey(temp)) {
                    return Colors[temp];
                }
                return colornames.UNRECOGNIZED;
            }
            public static colornames GetName(Bitmap screen,e_HUDLoc pos) {
                if (pos == e_HUDLoc.middle) {
                    if (GetName(screen.GetPixel(PixPos[pos].x, PixPos[pos].y)) != colornames.UNRECOGNIZED) return GetName(screen.GetPixel(PixPos[pos].x, PixPos[pos].y));
                    if (GetName(screen.GetPixel(PixPos[e_HUDLoc.middle2].x, PixPos[e_HUDLoc.middle2].y)) != colornames.UNRECOGNIZED) return GetName(screen.GetPixel(PixPos[e_HUDLoc.middle2].x, PixPos[e_HUDLoc.middle2].y));
                }
                return GetName(screen.GetPixel(PixPos[pos].x, PixPos[pos].y));
            }
            public static bool isColor(colornames cname, Color color) {
                if (GetName(color) == cname) return true;
                return false;
            }
        }
        static readonly public Dictionary<e_HUDLoc, pixPos> PixPos = new Dictionary<e_HUDLoc, pixPos>() {
            {e_HUDLoc.topLeft,new pixPos(){x = 42,y=44}},
            {e_HUDLoc.middle, new pixPos(){x=899,y=67}},//????
            {e_HUDLoc.middle2,new pixPos(){x=899,y=118 }},
            {e_HUDLoc.topRight,new pixPos(){x=1711,y=44}}
        };
        static readonly public Dictionary<color, colornames> Colors = new Dictionary<color, colornames>() {
            {new color(){
                R = 255,
                G = 255,
                B = 255
            },colornames.White},
            {new color(){
                R = 160,
                G = 232,
                B = 27
            },colornames.Lime},
            {new color(){
                R = 161,
                G = 73,
                B = 197
            },colornames.Purple},
            {new color(){
                R = 236,
                G = 153,
                B = 0
            },colornames.Orang},
            {new color(){
                R = 39,
                G = 170,
                B = 255
            },colornames.Blu},
            {new color(){
                R = 69,
                G = 255,
                B = 87
            },colornames.Green},
            {new color(){
                R = 200,
                G = 0,
                B = 19
            },colornames.Red},
            {new color(){
                R = 255,
                G = 255,
                B = 0
            },colornames.Yellow},
        };

        public enum inputState {
            sessionbegin,
            teleported,
            songname,
            songlength,
            hostage,
            idle,
            inputtype,
            inputsens,
            unrecognized
        };
        private static colornames readColorFrom(Bitmap screen,e_HUDLoc position) {
            if (color.GetName(screen,position) != colornames.UNRECOGNIZED ) {
                colornames readcolor = color.GetName(screen, position);
                for (int i = 0; i < 22 * 6; i += 22) {
                    if (color.GetName(screen, position) != readcolor) return colornames.UNRECOGNIZED;
                }
                return readcolor;
            }
            return colornames.UNRECOGNIZED;
        }
        public static inputState getState(Bitmap screen) {
            e_HUDLoc location;
            if (screen.Width < 1270 && screen.Height < 720) return inputState.unrecognized;
            //if (readColorFrom(screen, e_HUDLoc.topLeft) != colornames.UNRECOGNIZED) location = e_HUDLoc.topLeft;
            //else 
            location = e_HUDLoc.topLeft;
            colornames readColor = readColorFrom(screen, location);
            switch (readColor) {
                case colornames.UNRECOGNIZED:
                    if (location == e_HUDLoc.topLeft) {
                        if (readColorFrom(screen, e_HUDLoc.middle) != colornames.UNRECOGNIZED) {
                            return inputState.hostage;
                        } else return inputState.idle;
                    }
                    return inputState.unrecognized;
                case colornames.Orang:
                    if (location == e_HUDLoc.topLeft) {
                       return inputState.idle;
                    }
                    else return inputState.unrecognized;
                case colornames.Yellow:
                    return inputState.inputtype;
                case colornames.Lime:
                    return inputState.inputsens;
                case colornames.Blu:
                    if (location == e_HUDLoc.topLeft) return inputState.songlength;
                    else return inputState.hostage;
                case colornames.Green:
                    if (location == e_HUDLoc.topLeft) return inputState.songname;
                    else return inputState.hostage;
                case colornames.Red:
                    if (location == e_HUDLoc.topLeft) return inputState.teleported;
                    else return inputState.hostage;
                case colornames.Purple:
                    if (location == e_HUDLoc.topLeft) return inputState.sessionbegin;
                    else return inputState.hostage;
                default:
                    return inputState.unrecognized;
            }
        }

        public static int getVoteResult(Bitmap screen) {
            colornames cname = readColorFrom(screen, e_HUDLoc.middle);
            switch (cname) {
                case colornames.Red:
                    return 0;
                case colornames.Green:
                    return 1;
                case colornames.Blu:
                    return 2;
                case colornames.Purple: //used for Mouse Input stuff
                    return 3; //In-game angle has been reset
                case colornames.Yellow:
                    return 4; //Mouse input has been recognized
                default:
                    return -69;
            }
        }

        public enum gameKeys {
            Interact,
            Ultimate,
            Crouch,
            Ability1,
            Ability2
        }
        //Customize these nuts
        static readonly private Dictionary<gameKeys, System.Windows.Forms.Keys> KeyMap = new Dictionary<gameKeys, System.Windows.Forms.Keys>() {
            {gameKeys.Interact, Keys.F},
            {gameKeys.Ultimate, Keys.Q },
            {gameKeys.Crouch, Keys.LControlKey },
            {gameKeys.Ability1, Keys.LShiftKey },
            {gameKeys.Ability2,Keys.E }
        };
        static public System.Windows.Forms.Keys GetGameKey(gameKeys key) {
            return KeyMap[key];
        }


        public static List<byte[]> MakeItLeet(string data) {
            Console.WriteLine(data);
            List<byte[]> outofmymind = new List<byte[]>();
            List<byte> quaint = new List<byte>();
            /*string result = string.Empty;
            foreach (char ch in data) {
                result += Convert.ToString((int)ch, 2).PadLeft(8, '0');
            }
            Console.WriteLine(result);*/
            foreach (char ch in data) {
                string fuck = Convert.ToString((int)ch, 2).PadLeft(8, '0');
                foreach(char charac in fuck) {
                    quaint.Add(byte.Parse($"{charac}"));

                }
            }
            outofmymind.Add(quaint.ToArray());
            return outofmymind;
        }

    } 

}
