#include <Windows.h>
#include "Loader.h"
#include <stdio.h>
BOOL WINAPI DllMain(HMODULE hMod, DWORD dwReason, LPVOID lpReserved)
{
	switch (dwReason)
	{
	case DLL_PROCESS_ATTACH:
		
		//DisableThreadLibraryCalls(hMod);
		Loader::CTFAK = hMod;
		Loader::Entry();
		break;
	case DLL_PROCESS_DETACH:
		//kiero::shutdown();
		break;
	}
	return TRUE;
}
