// ============================================================================
//
// This file contains routines that are handled during the Runtime.
//
// Including creating, display, and handling your object.
// 
// ============================================================================

// Common Include
#include	"common.h"
#include	"CfcFile.h"
#include	"ImageFlt.h"
#include	"ImgFlt.h"

// DEBUGGER /////////////////////////////////////////////////////////////////

#if !defined(RUN_ONLY)
// Identifiers of items displayed in the debugger
enum
{
	DB_FILENAME,
};

// Items displayed in the debugger
WORD DebugTree[]=
{
	DB_FILENAME,

	// End of table (required)
	DB_END
};

#endif // !defined(RUN_ONLY)

// -------------------
// Free collision mask
// -------------------

void FreeColMask(LPRDATA rdPtr)
{
	if ( rdPtr->pColMask != NULL )
	{
		free(rdPtr->pColMask);
		rdPtr->pColMask = NULL;
	}
}

// --------------------
// Load image from file
// --------------------

void LoadImageFile(LPRDATA rdPtr, LPCTSTR pFileName)
{
	LPRH	rhPtr = rdPtr->rHo.hoAdRunHeader;
	HANDLE	hf = INVALID_HANDLE_VALUE;

	do {	
		if ( pFileName == NULL || *pFileName == 0 )
			break;

		// Copy filename to temp buffer
		TCHAR fname[MAX_PATH];
		_tcscpy(fname, pFileName);

		// Get surface prototype
		LPSURFACE wSurf = WinGetSurface((int)rhPtr->rhIdEditWin);
		LPSURFACE proto;
		GetSurfacePrototype(&proto, (wSurf != NULL) ? wSurf->GetDepth() : 24, ST_MEMORYWITHDC, SD_DIB);
		if ( proto == NULL )
			break;

		// Ask MMF2 to open the file (opens external file and embedded files, and downloads files in Vitalize mode)
		DWORD dwSize;
		hf = rhPtr->rh4.rh4Mv->mvOpenHFile(fname, &dwSize, 0);
		if ( hf == INVALID_HANDLE_VALUE )
			break;

		// Create CInpuBufFile object associated with the file handle
		DWORD dwOff = File_GetPosition((HFILE)hf);
		CInputBufFile bf;
		if ( bf.Create((HFILE)hf, dwOff, dwSize) != 0 )
			break;

		// Create surface
		cSurface* psf = NewSurface();
		if ( psf == NULL )
			break;
		psf->Create(4, 4, proto);
		psf->Fill(RGB(0,0,0));
		if ( psf->GetDepth() == 8 )
			psf->SetPalette (*wSurf);

		// Load picture
		CImageFilterMgr* pImgMgr = rhPtr->rh4.rh4Mv->mvImgFilterMgr;
		if ( ImportImageFromInputFile(pImgMgr, &bf, psf, NULL, IMPORT_IMAGE_USESURFACEDEPTH | IMPORT_IMAGE_USESURFACEPALETTE) )
		{
			// Copy filename if file successfully loaded
			_tcscpy(rdPtr->sFilename, fname);
			rdPtr->nWidth = rdPtr->rHo.hoImgWidth = psf->GetWidth();
			rdPtr->nHeight = rdPtr->rHo.hoImgHeight = psf->GetHeight();

			// Replace picture surface
			if ( rdPtr->pSf != NULL )
				DeleteSurface(rdPtr->pSf);
			rdPtr->pSf = psf;

			// Free collision mask
			FreeColMask(rdPtr);

			// Set transparent color
			if ( psf->GetTransparentColor() == RGB(0,0,0) )
			{
				COLORREF trspColor = rdPtr->dwTranspColor;
				if ( (rdPtr->dwOptions & SPICTFLAGS_TRANSP_FIRSTPIXEL) != 0 )
					psf->GetPixel(0,0,trspColor);
				psf->SetTransparentColor(trspColor);
			}
		}
		else
			DeleteSurface(psf);

	} while(FALSE);

	// Close picture file (opened with mvOpenHFile)
	if ( hf != INVALID_HANDLE_VALUE )
		rhPtr->rh4.rh4Mv->mvCloseHFile(hf);
}

// --------------------
// GetRunObjectDataSize
// --------------------
// Returns the size of the runtime datazone of the object
// 
ushort WINAPI DLLExport GetRunObjectDataSize(fprh rhPtr, LPEDATA edPtr)
{
	return(sizeof(RUNDATA));
}


// ---------------
// CreateRunObject
// ---------------
// The routine where the object is actually created
// 
short WINAPI DLLExport CreateRunObject(LPRDATA rdPtr, LPEDATA edPtr, fpcob cobPtr)
{
/*
   This routine runs when your object is created, as you might have guessed.
   It is here that you must transfer any data you need in rdPtr from edPtr,
   because after this has finished you cannot access it again!
   Also, if you have anything to initialise (e.g. dynamic arrays, surface objects)
   you should do it here, and free your resources in DestroyRunObject.
*/
	rdPtr->rHo.hoX = cobPtr->cobX;
	rdPtr->rHo.hoY = cobPtr->cobY;
	rdPtr->rHo.hoImgWidth = edPtr->nWidth;
	rdPtr->rHo.hoImgHeight = edPtr->nHeight;

	// Initialize RUNDATA members
	rdPtr->dwOptions = edPtr->dwOptions;
	rdPtr->dwTranspColor = edPtr->dwTranspColor;
	_tcscpy(rdPtr->sFilename, edPtr->sFilename);
	rdPtr->pColMask = NULL;

	// Load image
	LoadImageFile(rdPtr, rdPtr->sFilename);

	// No errors
	return 0;
}


// ----------------
// DestroyRunObject
// ----------------
// Destroys the run-time object
// 
short WINAPI DLLExport DestroyRunObject(LPRDATA rdPtr, long fast)
{
/*
   When your object is destroyed (either with a Destroy action or at the end of
   the frame) this routine is called. You must free any resources you have allocated!
*/

	// Free collision mask
	FreeColMask(rdPtr);

	// Delete picture surface
	if ( rdPtr->pSf != NULL )
	{
		DeleteSurface(rdPtr->pSf);
		rdPtr->pSf = NULL;
	}

	// No errors
	return 0;
}


// ----------------
// HandleRunObject
// ----------------
// Called (if you want) each loop, this routine makes the object live
// 
short WINAPI DLLExport HandleRunObject(LPRDATA rdPtr)
{
	if (rdPtr->rc.rcChanged)
		return REFLAG_DISPLAY;
	else
		return 0;
}

// ----------------
// DisplayRunObject
// ----------------
// Draw the object in the application screen.
// 
short WINAPI DLLExport DisplayRunObject(LPRDATA rdPtr)
{
	// Nothing to do, as GetRunObjectSurface returns the surface to display
	return 0;
}

// -------------------
// GetRunObjectSurface
// -------------------
// Implement this function instead of DisplayRunObject if your extension
// supports ink effects and transitions. Note: you can support ink effects
// in DisplayRunObject too, but this is automatically done if you implement
// GetRunObjectSurface (MMF applies the ink effect to the surface).
//
// Note: do not forget to enable the function in the .def file 
// if you remove the comments below.

cSurface* WINAPI DLLExport GetRunObjectSurface(LPRDATA rdPtr)
{
	return rdPtr->pSf;
}


// -------------------------
// GetRunObjectCollisionMask
// -------------------------
// Implement this function if your extension supports fine collision mode (OEPREFS_FINECOLLISIONS),
// Or if it's a background object and you want Obstacle properties for this object.
//
// Should return NULL if the object is not transparent.
//
// Note: do not forget to enable the function in the .def file 
// if you remove the comments below.
//

LPSMASK WINAPI DLLExport GetRunObjectCollisionMask(LPRDATA rdPtr, LPARAM lParam)
{
	// Opaque? collide with box => returns NULL
	if ( (rdPtr->rs.rsEffect & EFFECTFLAG_TRANSPARENT) == 0 )	// Note: only if your object has the OEPREFS_INKEFFECTS option
		return NULL;

	// Transparent? Create mask
	LPSMASK pMask = rdPtr->pColMask;
	if ( pMask == NULL )
	{
		if ( rdPtr->pSf != NULL )
		{
			DWORD dwMaskSize = rdPtr->pSf->CreateMask(NULL, lParam);
			if ( dwMaskSize != 0 )
			{
				pMask = (LPSMASK)calloc(dwMaskSize, 1);
				if ( pMask != NULL )
				{
					rdPtr->pSf->CreateMask(pMask, lParam);
					rdPtr->pColMask = pMask;
				}
			}
		}
	}

	// Note: for active objects, lParam is always the same.
	// For background objects (OEFLAG_BACKGROUND), lParam maybe be different if the user uses your object
	// as obstacle and as platform. In this case, you should store 2 collision masks
	// in your data: one if lParam is 0 and another one if lParam is different from 0.

	return pMask;
}


// ----------------
// PauseRunObject
// ----------------
// Enters the pause mode
// 
short WINAPI DLLExport PauseRunObject(LPRDATA rdPtr)
{
	// Ok
	return 0;
}


// -----------------
// ContinueRunObject
// -----------------
// Quits the pause mode
//
short WINAPI DLLExport ContinueRunObject(LPRDATA rdPtr)
{
	// Ok
	return 0;
}


// ============================================================================
//
// START APP / END APP / START FRAME / END FRAME routines
// 
// ============================================================================

// -------------------
// StartApp
// -------------------
// Called when the application starts or restarts.
// Useful for storing global data
// 
void WINAPI DLLExport StartApp(mv _far *mV, CRunApp* pApp)
{
	// Example
	// -------
	// Delete global data (if restarts application)
//	CMyData* pData = (CMyData*)mV->mvGetExtUserData(pApp, hInstLib);
//	if ( pData != NULL )
//	{
//		delete pData;
//		mV->mvSetExtUserData(pApp, hInstLib, NULL);
//	}
}

// -------------------
// EndApp
// -------------------
// Called when the application ends.
// 
void WINAPI DLLExport EndApp(mv _far *mV, CRunApp* pApp)
{
	// Example
	// -------
	// Delete global data
//	CMyData* pData = (CMyData*)mV->mvGetExtUserData(pApp, hInstLib);
//	if ( pData != NULL )
//	{
//		delete pData;
//		mV->mvSetExtUserData(pApp, hInstLib, NULL);
//	}
}

// -------------------
// StartFrame
// -------------------
// Called when the frame starts or restarts.
// 
void WINAPI DLLExport StartFrame(mv _far *mV, DWORD dwReserved, int nFrameIndex)
{
}

// -------------------
// EndFrame
// -------------------
// Called when the frame ends.
// 
void WINAPI DLLExport EndFrame(mv _far *mV, DWORD dwReserved, int nFrameIndex)
{
}


// ============================================================================
//
// DEBUGGER ROUTINES
// 
// ============================================================================

// -----------------
// GetDebugTree
// -----------------
// This routine returns the address of the debugger tree
//
LPWORD WINAPI DLLExport GetDebugTree(LPRDATA rdPtr)
{
#if !defined(RUN_ONLY)
	return DebugTree;
#else
	return NULL;
#endif // !defined(RUN_ONLY)
}

// -----------------
// GetDebugItem
// -----------------
// This routine returns the text of a given item.
//
void WINAPI DLLExport GetDebugItem(LPTSTR pBuffer, LPRDATA rdPtr, int id)
{
#if !defined(RUN_ONLY)

	TCHAR pTemp[DB_BUFFERSIZE];

	switch (id)
	{
	case DB_FILENAME:
		LoadString(hInstLib, IDS_FILENAME, pTemp, DB_BUFFERSIZE);
		wsprintf(pBuffer, pTemp, rdPtr->sFilename);
		break;
	}

#endif // !defined(RUN_ONLY)
}

// -----------------
// EditDebugItem
// -----------------
// This routine allows to edit editable items.
//
void WINAPI DLLExport EditDebugItem(LPRDATA rdPtr, int id)
{
#if !defined(RUN_ONLY)

#endif // !defined(RUN_ONLY)
}

