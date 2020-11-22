using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _TielJ.Player;
using static _TielJ.OWStuff;

namespace _TielJ {
    public class TielJ {
        public static double version = 0.1;
        string initurl;
        public TielJ(string url) {
            initurl = url;
            WindowRenderer.init();
            Player.Player.Initialize();
            Player.Player.InitUrl(url);
            OverwatchHandle = GetWindowH.getproc("Overwatch");
            if (OverwatchHandle == IntPtr.Zero) throw new Exception("Couldn't get window handle");
            MainLoop();
        }
        public static inputState prevInputState = inputState.unrecognized;
        IntPtr OverwatchHandle = IntPtr.Zero;

        bool initiated = false;
        int enteredInfoIter = 0;
        private void MainLoop() {
            while (true) {
                Bitmap screenshot = Screen.CaptureWindow(OverwatchHandle);
                inputState currentstate = getState(screenshot);
                if(currentstate != prevInputState) {
                    prevInputState = currentstate;
                    switch (currentstate) {
                        case inputState.sessionbegin:
                            initiated = false;
                            KeyManager.SendKey(GetGameKey(gameKeys.Interact)); //This will take us to song info entering state | I ended up skipping it
                            break;
                        case inputState.hostage:
                            int winner = getVoteResult(screenshot);
                            //read the winner song and play it
                            if (winner == -69) throw new Exception("Unable to recognize vote winner. You may need to update color definitions");
                            Player.Player.Play(Player.Player.GetMusicUrl(winner)); //I think I had a stroke
                            KeyManager.SendKey(GetGameKey(gameKeys.Ultimate));
                            break;
                        case inputState.idle:
                            break;
                        case inputState.songlength:
                            if (!initiated) {
                                KeyManager.SendInput(MakeItLeet(Player.Player.MusicInfo.length.ToString()), OverwatchHandle);
                                initiated = true;
                                Player.Player.Play(initurl);
                            }
                            else {
                                KeyManager.SendInput(MakeItLeet(Player.Player.getSimilar[enteredInfoIter].length.ToString()), OverwatchHandle);
                                enteredInfoIter++;
                                if (enteredInfoIter == 3) enteredInfoIter = 0;
                            }
                            KeyManager.SendKey(GetGameKey(gameKeys.Ultimate));
                            break;
                        case inputState.songname:
                            if (!initiated) {
                                KeyManager.SendInput(MakeItLeet(Player.Player.MusicInfo.name + "|" + Player.Player.MusicInfo.artist), OverwatchHandle);
                            }
                            else {
                                KeyManager.SendInput(MakeItLeet(Player.Player.getSimilar[enteredInfoIter].name + "|" + Player.Player.getSimilar[enteredInfoIter].artist), OverwatchHandle); //I have no comments on my naming etiquette
                            }
                            Thread.Sleep(100);
                            KeyManager.SendKey(GetGameKey(gameKeys.Ultimate));
                            break;
                        case inputState.teleported:
                            break;
                        case inputState.unrecognized:
                            break;//I don't know what to do with these informations
                    }
                }
                Player.Player.Tick();
                WindowRenderer.Render();
                Thread.Sleep(1000);
            }
        }
    }
}
