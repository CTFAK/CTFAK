// dllmain.cpp : Определяет точку входа для приложения DLL.
#include <windows.h>
#include <iostream>
#include <psapi.h>
#include <TlHelp32.h>
using namespace std;
void injEntry();
void* GetModuleBaseAddress(DWORD procId);
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        injEntry();
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
void injEntry()
{
    FILE* pFile = nullptr;
    AllocConsole();
    freopen_s(&pFile, "CONOUT$", "w", stdout);
    auto proc = GetCurrentProcessId();
    cout << "CTFAK data collector injected" << endl;
    cout << GetModuleBaseAddress(proc) << endl;
}
void* GetModuleBaseAddress(DWORD procId)
{
    uintptr_t modBaseAddr = 0;
    HANDLE hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE | TH32CS_SNAPMODULE32, procId);
    if (hSnap != INVALID_HANDLE_VALUE)
    {
        MODULEENTRY32 modEntry;
        modEntry.dwSize = sizeof(modEntry);
        if (Module32First(hSnap, &modEntry))
        {
            do
            {
                    modBaseAddr = (uintptr_t)modEntry.modBaseAddr;
                    break;
            } while (Module32Next(hSnap, &modEntry));
        }
    }
    CloseHandle(hSnap);
    return (void*)modBaseAddr;
}
