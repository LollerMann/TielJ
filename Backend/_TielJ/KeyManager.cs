﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static _TielJ.OWStuff;

namespace _TielJ {
    public static class KeyManager {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        //https://ourcodeworld.com/articles/read/520/simulating-keypress-in-the-right-way-using-inputsimulator-with-csharp-in-winforms
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public static void SendKey(System.Windows.Forms.Keys key) {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(100);//I don't know if removing this would trigger overwatch's anticheat but I don't want to find it out either.
            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        }

        public enum mousevent {
            LeftClick,
            RightClick,
            MiddleClick
        }
        private static pixPos midPoint = new pixPos() { x= -3};
        public static void SendMouseEvent(IntPtr windowHandle, mousevent mevent) {
            if(midPoint.x == -3)midPoint = Screen.getMiddle(windowHandle);
            MouseEventFlags upEvent, downEvent;
            switch (mevent) {
                case mousevent.LeftClick:
                    upEvent = MouseEventFlags.LeftUp;
                    downEvent = MouseEventFlags.LeftDown;
                    break;
                case mousevent.MiddleClick:
                    upEvent = MouseEventFlags.MiddleUp;
                    downEvent = MouseEventFlags.MiddleDown;
                    break;
                case mousevent.RightClick:
                    upEvent = MouseEventFlags.RightUp;
                    downEvent = MouseEventFlags.RightDown;
                    break;
                default:
                    throw new Exception("Le logic error has arrived");
            }
            MouseEvent(downEvent, midPoint.x, midPoint.y);
            Thread.Sleep(100);
            MouseEvent(upEvent, midPoint.x, midPoint.y);
        }

        public static void SendInput(List<byte[]> bytelist,IntPtr winHandle) {
            foreach(byte[] bytearr in bytelist) {
                foreach(byte coolbyte in bytearr) {
                    if (coolbyte == 0) SendMouseEvent(winHandle, mousevent.LeftClick);
                    else SendMouseEvent(winHandle, mousevent.RightClick);
                    Thread.Sleep(100);
                }
                SendKey(GetGameKey(gameKeys.Interact));
                Thread.Sleep(100);
            }
        }
        //I yanked this from somewhere but I forgot where it was.Props to whoever made this and sorry.
        [Flags]
        private enum MouseEventFlags {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y) {
            SetCursorPos(x, y);
        }

        public static void SetCursorPosition(MousePoint point) {
            SetCursorPos(point.X, point.Y);
        }

        private static MousePoint GetCursorPosition() {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        private static void MouseEvent(MouseEventFlags value, int offsetX, int offsetY) {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X + offsetX,
                 position.Y + offsetX,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint {
            public int X;
            public int Y;

            public MousePoint(int x, int y) {
                X = x;
                Y = y;
            }
        }
    }
}