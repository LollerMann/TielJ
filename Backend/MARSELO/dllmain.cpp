// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "VaderHook.h";
#include <iostream>

#define dllexport extern "C" __declspec(dllexport)


dllexport void MoveVader(int x, int y) {
    VaderHook vaderHook;
    vaderHook.movemouse(x,y);
}

dllexport bool GetMouseAccel() {
    //https://github.com/jan-glx/accelSwitch/blob/master/accelSwitch/accelSwitch.cpp
    int pvParam[3];
    if (!SystemParametersInfoW(SPI_GETMOUSE, 0, pvParam, 0)) std::cout << "GetParamInfo Failed" << std::endl;
    return pvParam[2];
}

dllexport void ToggleMouseAccel() {
    int pvParam[3];
    if (!SystemParametersInfoW(SPI_GETMOUSE, 0, pvParam, 0)) std::cout << "GetParamInfo Failed" << std::endl;
    pvParam[2] = !pvParam[2];
    if (!SystemParametersInfoW(SPI_SETMOUSE, 0, pvParam, SPIF_SENDCHANGE)) std::cout << "SetMouseAccel off failed" << std::endl;
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

