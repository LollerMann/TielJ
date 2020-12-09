#pragma once

#define WIN32_LEAN_AND_MEAN
#define _WIN32_WINNT 0x0500

#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <conio.h>
#include <string.h>
#include <windows.h>
#include <chrono>
#include <thread>



#include <iostream>

class VaderHook	{
public:
    void movemouse(int x, int y) {
        INPUT buffer[1];
        MouseSetup(buffer);
        MouseMoveAbsolute(buffer, x, y);
        //SetCursorPos(x, y);
    }
	VaderHook() {
        INPUT buffer[1];
        MouseSetup(buffer);
	}
    INPUT buffer[1];
private:
    double Sensitivity = 0.0;

    void MouseSetup(INPUT* buffer) {
        buffer->type = INPUT_MOUSE;
        buffer->mi.dx = 0;
        buffer->mi.dy = 0;
        buffer->mi.mouseData = 0;
        buffer->mi.dwFlags = MOUSEEVENTF_ABSOLUTE;
        buffer->mi.time = 0;
        buffer->mi.dwExtraInfo = 0;
    }
    void MouseMoveAbsolute(INPUT* buffer, int x, int y) {
        
        buffer->mi.dx = x ;
        buffer->mi.dy = y ;
        buffer->mi.dwFlags = MOUSEEVENTF_MOVE;;
        SendInput(1, buffer, sizeof(INPUT));
        //std::cout << std::endl << "moving " << buffer->mi.dx << "," << buffer->mi.dy << " relative to current position"  << std::endl;
    }
    void MouseClick(INPUT* buffer) {
        buffer->mi.dwFlags = (MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN);
        SendInput(1, buffer, sizeof(INPUT));
        Sleep(10);
        buffer->mi.dwFlags = (MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP);
        SendInput(1, buffer, sizeof(INPUT));
    }

    enum MouseButtons {
        MouseLeft = VK_LBUTTON,
        MouseRight = VK_RBUTTON,
        MouseMiddle = VK_MBUTTON
    };


    bool GetMouseButton(MouseButtons btn) { return (GetKeyState(btn) & 0x100) != 0; }
    bool GetKeyPress(int btn) { return (GetKeyState(btn) & 0x100) != 0; }
    bool GetKeyboardKey(char c) { return (GetKeyState(c) & 0x8000); }

};

