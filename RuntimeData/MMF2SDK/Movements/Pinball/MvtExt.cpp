
// MvtExt.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "Resource.h"
#include "MvtExt.h"
#include "Pinball.h"

HINSTANCE hInst;

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call) 
	{
	case DLL_PROCESS_ATTACH:
		hInst = (HINSTANCE)hModule;
		break;
	}
    return TRUE;
}

// Retrieves the number of movements
MVTEXT_API int CALLBACK GetNumberOfMvts()
{
	return 1;
}

MVTEXT_API void CALLBACK GetMvtName(int nMvt, LPSTR pName, int cbSize)
{
	switch (nMvt) 
	{
	case 0:
		::LoadString(hInst, IDS_PINBALLNAME, pName, cbSize);
		break;
	}
}

MVTEXT_API DWORD CALLBACK GetMvtID(int nMvt)
{
	switch (nMvt) 
	{
	case 0:
		return DWORDSTR('P','I','N','B');
	}
	return 0;
}

MVTEXT_API HICON CALLBACK GetMvtIcon(int nMvt)
{
	switch (nMvt) 
	{
	case 0:
		return LoadIcon(hInst, MAKEINTRESOURCE(IDI_ICON1));
	}
	return NULL;
}

MVTEXT_API CMvt* CALLBACK CreateMvt(DWORD dwMvtID)
{
	switch (dwMvtID) 
	{
	case DWORDSTR('P','I','N','B'):
		return new CPinball;
	}
	return NULL;
}

