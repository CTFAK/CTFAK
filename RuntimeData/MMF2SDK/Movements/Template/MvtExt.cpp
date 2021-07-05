
// MvtExt.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "Resource.h"
#include "MvtExt.h"
#include "Mvt1.h"
// If you have more than one movement, include its H file here
// #include "Mvt2.h" 

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
	// Change this to 2 or more if your have more than one movement in the extension
	return 1;
}

MVTEXT_API void CALLBACK GetMvtName(int nMvt, LPSTR pName, int cbSize)
{
	switch (nMvt) 
	{
	case 0:
		::LoadString(hInst, IDS_MVT1NAME, pName, cbSize);
		break;
	
/*	// To have more than one movement, remove the remarks
	case 1:
		::LoadString(hInst, IDS_MVT2NAME, pName, cbSize);
		break;
*/	
	}
}

MVTEXT_API DWORD CALLBACK GetMvtID(int nMvt)
{
	switch (nMvt) 
	{
	case 0:
		return DWORDSTR('T','P','L','1');
	
/*	To have more than one movement, remove the remark
	case 1:
		return DWORDSTR('T','P','L','2');
*/
	}
	return 0;
}

MVTEXT_API HICON CALLBACK GetMvtIcon(int nMvt)
{
	switch (nMvt) 
	{
	case 0:
		return LoadIcon(hInst, MAKEINTRESOURCE(IDI_ICON1));
	
/*	To have more than one movement, remove the remark
	case 1:
		return LoadIcon(hInst, MAKEINTRESOURCE(IDI_ICON2));
*/
	}
	return NULL;
}

MVTEXT_API CMvt* CALLBACK CreateMvt(DWORD dwMvtID)
{
	switch (dwMvtID) 
	{
	case DWORDSTR('T','P','L','1'):
		return new CMvt1;
	
/*	To have more than one movement, remove the remark
	case DWORDSTR('T','P','L','2'):
		return new CMvt2;
*/
	}
	return NULL;
}

