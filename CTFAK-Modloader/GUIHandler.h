#pragma once
#include <d3d9.h>


long __stdcall hkEndScene(LPDIRECT3DDEVICE9 pDevice);
DWORD WINAPI HookDirect3D9(LPVOID lpParam);
