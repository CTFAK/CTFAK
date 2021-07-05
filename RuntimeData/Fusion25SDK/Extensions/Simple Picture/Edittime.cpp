
// ============================================================================
//
// This file contains routines that are handled only during the Edittime,
// under the Frame and Event editors.
//
// Including creating, display, and setting up your object.
// 
// ============================================================================

// Common
#include	"common.h"
#include	"ImageFlt.h"
#include	"ImgFlt.h"

#if !defined(RUN_ONLY)

// PROPERTIES /////////////////////////////////////////////////////////////////

// Property identifiers
enum {
	PROPID_FILENAME = PROPID_EXTITEM_CUSTOM_FIRST,		// Filename
	PROPID_TRANSPCOLORGROUP,
	PROPID_TRANSPFIRSTPIXEL,							// Option "transparent color = first pixel"
	PROPID_TRANSPCOLORCUSTOM,							// Custom transparent color
};

// Property definitions
//
// Type, ID, Text, Text of Info box [, Options, Init Param]
//

// General Tab
PropData Properties[] = {

	// Filename
	PropData_PictureFilename	(PROPID_FILENAME,	IDS_PROP_FILENAME,	IDS_PROP_FILENAME_INFO,	FALSE),

	// End of table (required)
	PropData_End()
};

// Displa Tav
PropData TranspProps[] = {
	PropData_Group(PROPID_TRANSPCOLORGROUP, IDS_PROP_TRANSPCOLORGROUP, IDS_PROP_TRANSPCOLORGROUP_INFO),
		PropData_CheckBox(PROPID_TRANSPFIRSTPIXEL, IDS_PROP_TRANSPFIRSTPIXEL, IDS_PROP_TRANSPFIRSTPIXEL_INFO),
		PropData_Color_Check(PROPID_TRANSPCOLORCUSTOM, IDS_PROP_TRANSPCOLORCUSTOM, IDS_PROP_TRANSPCOLORCUSTOM_INFO),
	PropData_End()
};

#endif // !defined(RUN_ONLY)


// -----------------
// GetObjInfos
// -----------------
// Return object info
//
// Info displayed in the object's About properties
// Note: ObjComment is also displayed in the Quick Description box in the Insert Object dialog box
//
void WINAPI	DLLExport GetObjInfos (mv _far *mV, LPEDATA edPtr, LPTSTR ObjName, LPTSTR ObjAuthor, LPTSTR ObjCopyright, LPTSTR ObjComment, LPTSTR ObjHttp)
{
#ifndef RUN_ONLY
	// Name
	LoadString(hInstLib, IDST_OBJNAME,ObjName, 255);

	// Author
	LoadString(hInstLib, IDST_AUTHOR,ObjAuthor,255);

	// Copyright
	LoadString(hInstLib, IDST_COPYRIGHT,ObjCopyright,255);

	// Comment
	LoadString(hInstLib, IDST_COMMENT,ObjComment,1024);

	// Internet address
	LoadString(hInstLib, IDST_HTTP,ObjHttp,255);
#endif // !defined(RUN_ONLY)
}

// -----------------
// GetHelpFileName
// -----------------
// Returns the help filename of the object.
//
LPCTSTR WINAPI GetHelpFileName()
{
#ifndef RUN_ONLY
	return _T("Help\\MyExt.chm");
#else
	return NULL;
#endif // !defined(RUN_ONLY)
}


// ============================================================================
//
// ROUTINES USED UNDER FRAME EDITOR
// 
// ============================================================================


// --------------------
// CreateObject
// --------------------
// Called when you choose "Create new object". It should display the setup box 
// and initialize everything in the datazone.

int WINAPI DLLExport CreateObject(mv _far *mV, fpLevObj loPtr, LPEDATA edPtr)
{
#ifndef RUN_ONLY
	// Check compatibility
	if ( IS_COMPATIBLE(mV) )
	{
		// Default size
		edPtr->nWidth = 100;
		edPtr->nHeight = 20;

		// Default options
		edPtr->dwOptions = 0;

		// Default transparent color
		edPtr->dwTranspColor = RGB(255,0,255);

		// Initialize file selector parameters
		OPENFILENAME	ofn;

		LPTSTR szFileTitle = (LPTSTR)calloc(_MAX_PATH, sizeof(TCHAR));
		memset((LPVOID)&ofn, 0, sizeof(OPENFILENAME));
		ofn.lStructSize = sizeof(OPENFILENAME);
		ofn.hwndOwner = mV->mvHEditWin;
		ofn.nFilterIndex = 1;
		ofn.lpstrFile = edPtr->sFilename;
		ofn.nMaxFile = _MAX_PATH;
		ofn.lpstrFileTitle = szFileTitle;
		ofn.nMaxFileTitle = _MAX_PATH;
		ofn.lpstrInitialDir = NULL;
		ofn.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST | OFN_EXPLORER | OFN_HIDEREADONLY;

		// Open file selector
		CImageFilterMgr* pImgMgr = mV->mvImgFilterMgr;					// MMF image filter manager
		if ( ChoosePicture(&ofn, TRUE, pImgMgr, NULL, PICSEL_IMAGES) )
		{
			// User selected a file => open the filter associated with this type of picture file
			CImageFilter pFilter(pImgMgr);
			if ( pFilter.Open(edPtr->sFilename) == IF_OK )
			{
				// A filter was able to open the file => get file info
				edPtr->nWidth = pFilter.GetWidth();
				edPtr->nHeight = pFilter.GetHeight();

				// Close filter
				pFilter.Close();
			}
		}
		else
		{
			// User didn't select a file
			*edPtr->sFilename = 0;
		}

		free(szFileTitle);

		// No error
		return 0;
	}
#endif // !defined(RUN_ONLY)

	// Error
	return -1;
}

// --------------------
// EditObject
// --------------------
// Called when the user selects the Edit command in the object's popup menu
//
BOOL WINAPI EditObject (mv _far *mV, fpObjInfo oiPtr, fpLevObj loPtr, LPEDATA edPtr)
{
#ifndef RUN_ONLY

	BOOL bOK = FALSE;

	OPENFILENAME	ofn;

	// Ask for picture file
	LPTSTR szFileTitle = (LPTSTR)calloc(_MAX_PATH, sizeof(TCHAR));
	LPTSTR szFileName =  (LPTSTR)calloc(_MAX_PATH, sizeof(TCHAR));
	memset((LPVOID)&ofn, 0, sizeof(OPENFILENAME));
	ofn.lStructSize = sizeof(OPENFILENAME);
	ofn.hwndOwner = mV->mvHEditWin;
	ofn.nFilterIndex = 1;
	ofn.lpstrFile = szFileName;
	ofn.nMaxFile = _MAX_PATH;
	ofn.lpstrFileTitle = szFileTitle;
	ofn.nMaxFileTitle = _MAX_PATH;
	ofn.lpstrInitialDir = NULL;
	ofn.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST | OFN_EXPLORER | OFN_HIDEREADONLY;

	_tcscpy(szFileName, edPtr->sFilename);

	// Open dialog box
	CImageFilterMgr* pImgMgr = mV->mvImgFilterMgr;
	if ( ChoosePicture(&ofn, TRUE, pImgMgr, NULL, PICSEL_IMAGES) )
	{
		CImageFilter pFilter(pImgMgr);
		if ( pFilter.Open(szFileName) == IF_OK )
		{
			edPtr->nWidth = pFilter.GetWidth();
			edPtr->nHeight = pFilter.GetHeight();

			// Force Transparent option if alpha channel
			if ( pFilter.ContainsAlphaChannel() && mvGetPropCheck(mV, edPtr, PROPID_FITEM_TRANSPARENT) == 0 )
				mvSetPropCheck(mV, edPtr, PROPID_FITEM_TRANSPARENT, TRUE);

			// Close filter
			pFilter.Close();

			// Copy new filename
			_tcscpy(edPtr->sFilename, szFileName);

			// OK
			bOK = TRUE;
		}
	}
	free(szFileName);
	free(szFileTitle);

	return bOK;

#endif // !defined(RUN_ONLY)
	return FALSE;
}

// --------------------
// SetEditSize
// --------------------
// Called when the object has been resized
//
// Note: remove the comments if your object can be resized (and remove the comments in the .def file)

/*

BOOL WINAPI SetEditSize(LPMV mv, LPEDATA edPtr, int cx, int cy)
{
#ifndef RUN_ONLY

	edPtr->nWidth = cx;
	edPtr->nHeight = cy;

#endif // !defined(RUN_ONLY)
	return TRUE;	// OK
}

*/

// --------------------
// PutObject
// --------------------
// Called when each individual object is dropped in the frame.
//
void WINAPI	DLLExport PutObject(mv _far *mV, fpLevObj loPtr, LPEDATA edPtr, ushort cpt)
{
#ifndef RUN_ONLY
#endif // !defined(RUN_ONLY)
}

// --------------------
// RemoveObject
// --------------------
// Called when each individual object is removed from the frame.
//
void WINAPI	DLLExport RemoveObject(mv _far *mV, fpLevObj loPtr, LPEDATA edPtr, ushort cpt)
{
#ifndef RUN_ONLY
	// Is the last object removed?
    if (0 == cpt)
	{
		// Do whatever necessary to remove our data
	}
#endif // !defined(RUN_ONLY)
}

// --------------------
// DuplicateObject
// --------------------
// Called when an object is created from another one (note: should be called CloneObject instead...)
//
void WINAPI DLLExport DuplicateObject(mv __far *mV, fpObjInfo oiPtr, LPEDATA edPtr)
{
#ifndef RUN_ONLY
#endif // !defined(RUN_ONLY)
}

// --------------------
// GetObjectRect
// --------------------
// Returns the size of the rectangle of the object in the frame editor.
//
void WINAPI DLLExport GetObjectRect(mv _far *mV, RECT FAR *rc, fpLevObj loPtr, LPEDATA edPtr)
{
#ifndef RUN_ONLY

	rc->right = rc->left + edPtr->nWidth;
	rc->bottom = rc->top + edPtr->nHeight;

#endif // !defined(RUN_ONLY)
	return;
}


// --------------------
// EditorDisplay
// --------------------
// Displays the object under the frame editor
//
// Note: this function is optional. If it's not defined in your extension,
// MMF2 will load and display the EXO_IMAGE bitmap if it's defined in your resource file.
//
// If you need to draw the icon manually, remove the comments around this function and in the .def file.
//

void WINAPI DLLExport EditorDisplay(mv _far *mV, fpObjInfo oiPtr, fpLevObj loPtr, LPEDATA edPtr, RECT FAR *rc)
{
#ifndef RUN_ONLY
/*
	// This is a simple case of drawing an image onto MMF's frame editor window
	// First, we must get a pointer to the surface used by the frame editor

	LPSURFACE ps = WinGetSurface((int)mV->mvIdEditWin);
	if ( ps != NULL )		// Do the following if this surface exists
	{
		// Create font
		HFONT hFnt = CreateFontIndirect(&edPtr->textFont);

		// Ink effects
		BOOL bTransp = ((oiPtr->oiHdr.oiInkEffect & EFFECTFLAG_TRANSPARENT) != 0);
		BlitMode bm = (bTransp) ? BMODE_TRANSP : BMODE_OPAQUE;
		BOOL bAntiA = (oiPtr->oiHdr.oiInkEffect & EFFECTFLAG_ANTIALIAS) ? TRUE : FALSE;
		BlitOp bo = (BlitOp)(oiPtr->oiHdr.oiInkEffect & EFFECT_MASK);
		LPARAM boParam = oiPtr->oiHdr.oiInkEffectParam;

		// Draw text
		DWORD dwDTFlags = edPtr->dwAlignFlags | DT_VCENTER | DT_NOPREFIX | DT_SINGLELINE | DT_END_ELLIPSIS;
		ps->DrawText(edPtr->sText, _tcslen(edPtr->sText), rc, dwDTFlags, edPtr->textColor, hFnt,
					 bm, bo, boParam, bAntiA);

		// Delete font
		if ( hFnt != NULL )
			DeleteObject(hFnt);
	}
*/

	// Get frame editor surface & MMF image filter
	LPSURFACE wSurf = WinGetSurface((int)mV->mvIdEditWin);
	CImageFilterMgr* pImgMgr = mV->mvImgFilterMgr;

	// Get surface prototype for picture file
	LPSURFACE proto = NULL;
	if ( wSurf != NULL )
		GetSurfacePrototype(&proto, wSurf->GetDepth(), ST_MEMORYWITHDC, SD_DIB);

	// Create surface used to load the picture file
	cSurface	sf;
	sf.Create(4, 4, proto);

	// Copy palette if 8-bit surface
	if ( sf.GetDepth() == 8 )
		sf.SetPalette (*wSurf);

	// Import picture file
	if ( edPtr == NULL || *edPtr->sFilename == 0 || proto == NULL || pImgMgr == NULL ||
		!ImportImage(pImgMgr, edPtr->sFilename, &sf, NULL, IMPORT_IMAGE_USESURFACEDEPTH | IMPORT_IMAGE_USESURFACEPALETTE) )
	{
		// Error => draw rectangle
		wSurf->Rectangle(rc->left, rc->top, rc->right, rc->bottom, RGB(200,200,200), 1, BLACK);
		wSurf->Line(rc->left, rc->top, rc->right-1, rc->bottom-1);
		wSurf->Line(rc->right-1, rc->top+1, rc->left+1, rc->bottom-1);
	}

	// Draw picture
	else
	{
		// Set transparent color
		if ( (oiPtr->oiHdr.oiInkEffect & EFFECTFLAG_TRANSPARENT) != 0 )
		{
			if ( sf.GetTransparentColor() == RGB(0,0,0) )		// check if the transparent color was set by the ImportImage function (for example if GIF image with transparent color)
			{
				COLORREF trspColor = edPtr->dwTranspColor;
				if ( (edPtr->dwOptions & SPICTFLAGS_TRANSP_FIRSTPIXEL) != 0 )
					sf.GetPixel(0,0,trspColor);
				sf.SetTransparentColor(trspColor);
			}
		}

		BlitMode bm = (oiPtr->oiHdr.oiInkEffect & EFFECTFLAG_TRANSPARENT) ? BMODE_TRANSP : BMODE_OPAQUE;
		BOOL bAntiA = (oiPtr->oiHdr.oiInkEffect & EFFECTFLAG_ANTIALIAS) ? TRUE : FALSE;
		BlitOp bo = (BlitOp)(oiPtr->oiHdr.oiInkEffect & EFFECT_MASK);
		int effectParam = oiPtr->oiHdr.oiInkEffectParam;

		sf.Blit(*wSurf, rc->left, rc->top, bm, bo, effectParam, bAntiA ? BLTF_ANTIA : 0);
	}

#endif // !defined(RUN_ONLY)
}


// --------------------
// IsTransparent
// --------------------
// This routine tells CC&C if the mouse pointer is over a transparent zone of the object.
// 

extern "C" BOOL WINAPI DLLExport IsTransparent(mv _far *mV, fpLevObj loPtr, LPEDATA edPtr, int dx, int dy)
{
#ifndef RUN_ONLY
	// Write your code here
#endif // !defined(RUN_ONLY)
	return FALSE;
}

// --------------------
// PrepareToWriteObject
// --------------------
// Just before writing the datazone when saving the application, CC&C calls this routine.
// 
void WINAPI	DLLExport PrepareToWriteObject(mv _far *mV, LPEDATA edPtr, fpObjInfo adoi)
{
#ifndef RUN_ONLY
	// Write your code here
#endif // !defined(RUN_ONLY)
}

// --------------------
// GetFilters
// --------------------

BOOL WINAPI GetFilters(LPMV mV, LPEDATA edPtr, DWORD dwFlags, LPVOID pReserved)
{
#ifndef RUN_ONLY
	// If your extension uses image filters
	if ( (dwFlags & GETFILTERS_IMAGES) != 0 )
		return TRUE;

	// If your extension uses sound filters
//	if ( (dwFlags & GETFILTERS_SOUNDS) != 0 )
//		return TRUE;
#endif // RUN_ONLY
	return FALSE;
}

// --------------------
// UsesFile
// --------------------
// Triggers when a file is dropped onto the frame
// Return TRUE if you can create an object from the given file
//
BOOL WINAPI	DLLExport UsesFile (LPMV mV, LPTSTR fileName)
{
	BOOL r = FALSE;

#ifndef RUN_ONLY

	if ( fileName != NULL )
	{
		CImageFilterMgr*	pImgMgr = mV->mvImgFilterMgr;
		if ( CanImportImage(pImgMgr, fileName) )
			r = TRUE;
	}

#endif // !defined(RUN_ONLY)

	return r;
}


// --------------------
// CreateFromFile
// --------------------
// Creates a new object from file
//
void WINAPI	DLLExport CreateFromFile (LPMV mV, LPTSTR fileName, LPEDATA edPtr)
{
#ifndef RUN_ONLY

	// Set default object flags
	edPtr->dwOptions = 0;
	_tcscpy (edPtr->sFilename, fileName);
	edPtr->nWidth = 32;
	edPtr->nHeight = 32;
	edPtr->dwTranspColor = 0;

	CImageFilterMgr* pImgMgr = mV->mvImgFilterMgr;
	CImageFilter pFilter(pImgMgr);
	if ( pFilter.Open(fileName) == IF_OK )
	{
		edPtr->nWidth = pFilter.GetWidth();
		edPtr->nHeight = pFilter.GetHeight();

		pFilter.Close();
	}

#endif // !defined(RUN_ONLY)
}

// ============================================================================
//
// PROPERTIES
// 
// ============================================================================

// --------------------
// GetProperties
// --------------------
// Inserts properties into the properties of the object.
//
BOOL WINAPI DLLExport GetProperties(LPMV mV, LPEDATA edPtr, BOOL bMasterItem)
{
#ifndef RUN_ONLY
	mvInsertProps(mV, edPtr, Properties, PROPID_TAB_GENERAL, TRUE);
	mvInsertProps(mV, edPtr, TranspProps, PROPID_TAB_DISPLAY, TRUE);
#endif // !defined(RUN_ONLY)

	// OK
	return TRUE;
}

// --------------------
// ReleaseProperties
// --------------------
// Called when the properties are removed from the property window.
//
void WINAPI DLLExport ReleaseProperties(LPMV mV, LPEDATA edPtr, BOOL bMasterItem)
{
#ifndef RUN_ONLY
	// Write your code here
#endif // !defined(RUN_ONLY)
}

// --------------------
// GetPropCreateParam
// --------------------
// Called when a property is initialized and its creation parameter is NULL (in the PropData).
// Allows you, for example, to change the content of a combobox property according to specific settings in the EDITDATA structure.
//
LPARAM WINAPI DLLExport GetPropCreateParam(LPMV mV, LPEDATA edPtr, UINT nPropID)
{
#ifndef RUN_ONLY
	// Example
	// -------
//	if ( nPropID == PROPID_COMBO )
//	{
//		switch (edPtr->sType)
//		{
//		case TYPE1:
//			return (LPARAM)ComboList1;
//		case TYPE2:
//			return (LPARAM)ComboList2;
//		}
//	}
#endif // !defined(RUN_ONLY)
	return NULL;
}

// ----------------------
// ReleasePropCreateParam
// ----------------------
// Called after a property has been initialized.
// Allows you, for example, to free memory allocated in GetPropCreateParam.
//
void WINAPI DLLExport ReleasePropCreateParam(LPMV mV, LPEDATA edPtr, UINT nPropID, LPARAM lParam)
{
#ifndef RUN_ONLY
#endif // !defined(RUN_ONLY)
}

// --------------------
// GetPropValue
// --------------------
// Returns the value of properties that have a value.
// Note: see GetPropCheck for checkbox properties
//
LPVOID WINAPI DLLExport GetPropValue(LPMV mV, LPEDATA edPtr, UINT nPropID)
{
#ifndef RUN_ONLY
	switch (nPropID) {

	// Filename
	case PROPID_FILENAME:
		return new CPropStringValue(edPtr->sFilename);

	// Custom transparent color
	case PROPID_TRANSPCOLORCUSTOM:
		return new CPropDWordValue(edPtr->dwTranspColor);

	}
#endif // !defined(RUN_ONLY)
	return NULL;
}

// --------------------
// GetPropCheck
// --------------------
// Returns the checked state of properties that have a check box.
//
BOOL WINAPI DLLExport GetPropCheck(LPMV mV, LPEDATA edPtr, UINT nPropID)
{
#ifndef RUN_ONLY

	switch (nPropID) {

	case PROPID_TRANSPFIRSTPIXEL:
		return ((edPtr->dwOptions & SPICTFLAGS_TRANSP_FIRSTPIXEL) != 0);

	case PROPID_TRANSPCOLORCUSTOM:
		return ((edPtr->dwOptions & SPICTFLAGS_TRANSP_FIRSTPIXEL) == 0);
	}

#endif // !defined(RUN_ONLY)
	return 0;		// Unchecked
}

// Update size when picture filename changes
void UpdateSize(LPMV mV, LPEDATA edPtr, BOOL bUpdateTransp)
{
	CImageFilterMgr* pImgMgr = mV->mvImgFilterMgr;
	CImageFilter pFilter(pImgMgr);
	if ( pFilter.Open(edPtr->sFilename) == IF_OK )
	{
		edPtr->nWidth = pFilter.GetWidth();
		edPtr->nHeight = pFilter.GetHeight();

		if ( bUpdateTransp && pFilter.ContainsAlphaChannel() && mvGetPropCheck(mV, edPtr, PROPID_FITEM_TRANSPARENT) == 0 )
			mvSetPropCheck(mV, edPtr, PROPID_FITEM_TRANSPARENT, TRUE);

		pFilter.Close();
	}
}

// --------------------
// SetPropValue
// --------------------
// This routine is called by MMF after a property has been modified.
//
void WINAPI DLLExport SetPropValue(LPMV mV, LPEDATA edPtr, UINT nPropID, LPVOID lParam)
{
#ifndef RUN_ONLY

	// Gets the pointer to the CPropValue structure
	CPropValue* pValue = (CPropValue*)lParam;
	BOOL bRefresh = TRUE;

	switch (nPropID) 
	{
	case PROPID_FILENAME:
		if ( ((CPropStringValue*)pValue)->GetString() != NULL )
		{
			// Set filename
			_tcscpy (edPtr->sFilename, ((CPropStringValue*)pValue)->GetString());
			UpdateSize(mV, edPtr, TRUE);
		}
		break;

	case PROPID_TRANSPCOLORCUSTOM:
		edPtr->dwTranspColor = ((CPropDWordValue*)pValue)->m_dwValue;
		break;

	default:
		bRefresh = FALSE;
		break;
	}
	if ( bRefresh )
		mvInvalidateObject(mV, edPtr);

#endif // !defined(RUN_ONLY)
}

// --------------------
// SetPropCheck
// --------------------
// This routine is called by MMF when the user modifies a checkbox in the properties.
//
void WINAPI DLLExport SetPropCheck(LPMV mV, LPEDATA edPtr, UINT nPropID, BOOL nCheck)
{
#ifndef RUN_ONLY

	BOOL bRefresh = TRUE;
	switch (nPropID) 
	{
	case PROPID_FITEM_TRANSPARENT:
		mvRefreshProp(mV, edPtr, PROPID_TRANSPFIRSTPIXEL, FALSE);
		mvRefreshProp(mV, edPtr, PROPID_TRANSPCOLORCUSTOM, FALSE);
		break;

	case PROPID_TRANSPFIRSTPIXEL:
		edPtr->dwOptions |= SPICTFLAGS_TRANSP_FIRSTPIXEL;
		mvRefreshProp(mV, edPtr, PROPID_TRANSPFIRSTPIXEL, FALSE);
		mvRefreshProp(mV, edPtr, PROPID_TRANSPCOLORCUSTOM, FALSE);
		break;

	case PROPID_TRANSPCOLORCUSTOM:
		edPtr->dwOptions &= ~SPICTFLAGS_TRANSP_FIRSTPIXEL;
		mvRefreshProp(mV, edPtr, PROPID_TRANSPFIRSTPIXEL, FALSE);
		mvRefreshProp(mV, edPtr, PROPID_TRANSPCOLORCUSTOM, FALSE);
		break;
	default:
		bRefresh = FALSE;
		break;
	}
	if ( bRefresh )
		mvInvalidateObject(mV, edPtr);

#endif // !defined(RUN_ONLY)
}

// --------------------
// EditProp
// --------------------
// This routine is called when the user clicks the button of a Button or EditButton property.
//
BOOL WINAPI DLLExport EditProp(LPMV mV, LPEDATA edPtr, UINT nPropID)
{
#ifndef RUN_ONLY

#endif // !defined(RUN_ONLY)
	return FALSE;
}

// --------------------
// IsPropEnabled
// --------------------
// This routine returns the enabled state of a property.
//
BOOL WINAPI IsPropEnabled(LPMV mV, LPEDATA edPtr, UINT nPropID)
{
#ifndef RUN_ONLY

	switch (nPropID) {
	case PROPID_TRANSPFIRSTPIXEL:
	case PROPID_TRANSPCOLORCUSTOM:
		return (mvGetPropCheck(mV, edPtr, PROPID_FITEM_TRANSPARENT) != 0);
	}

#endif // !defined(RUN_ONLY)
	return TRUE;
}


// ============================================================================
//
// ROUTINES USED UNDER EVENT / TIME / STEP-THROUGH EDITOR
// You should not need to change these routines
// 
// ============================================================================

// -----------------
// menucpy
// -----------------
// Internal routine used later, copy one menu onto another
// 
#ifndef RUN_ONLY
void menucpy(HMENU hTargetMenu, HMENU hSourceMenu)
{
#ifndef RUN_ONLY
	int			n, id, nMn;
	LPTSTR		strBuf;
	HMENU		hSubMenu;
	
	nMn = GetMenuItemCount(hSourceMenu);
	strBuf = (LPTSTR)calloc(80, sizeof(TCHAR));
	for (n = 0; n < nMn; n++)
	{
		if (0 == (id = GetMenuItemID(hSourceMenu, n)))
			AppendMenu(hTargetMenu, MF_SEPARATOR, 0, 0L);
		else
		{
			GetMenuString(hSourceMenu, n, strBuf, 80, MF_BYPOSITION);
			if (id != -1)
				AppendMenu(hTargetMenu, GetMenuState(hSourceMenu, n, MF_BYPOSITION), id, strBuf);
			else
			{
				hSubMenu = CreatePopupMenu();
				AppendMenu(hTargetMenu, MF_POPUP | MF_STRING, (uint)hSubMenu, strBuf);
				menucpy(hSubMenu, GetSubMenu(hSourceMenu, n));
			}
		}
	}
	free(strBuf);
#endif
}

// -----------------
// GetPopupMenu
// -----------------
// Internal routine used later. Returns the first popup from a menu
// 
HMENU GetPopupMenu(short mn)
{
	HMENU	hMn, hSubMenu, hPopup = NULL;

	if ((hMn = LoadMenu(hInstLib, MAKEINTRESOURCE(mn))) != NULL)
	{
		if ((hSubMenu = GetSubMenu(hMn, 0)) != NULL)
		{
			if ((hPopup = CreatePopupMenu()) != NULL)
				menucpy(hPopup, hSubMenu);
		}
		DestroyMenu(hMn);
	}
	return hPopup;
}

// --------------------
// GetEventInformations
// --------------------
// Internal routine used later. Look for one event in one of the eventInfos array...
// No protection to go faster: you must properly enter the conditions/actions!
// 
static LPEVENTINFOS2 GetEventInformations(LPEVENTINFOS2 eiPtr, short code)

{
	while(eiPtr->infos.code != code)
		eiPtr = EVINFO2_NEXT(eiPtr);
	
	return eiPtr;
}
#endif // !defined(RUN_ONLY)


// ----------------------------------------------------
// GetConditionMenu / GetActionMenu / GetExpressionMenu
// ----------------------------------------------------
// Load the condition/action/expression menu from the resource, eventually
// enable or disable some options, and returns it to CC&C.
//
HMENU WINAPI DLLExport GetConditionMenu(mv _far *mV, fpObjInfo oiPtr, LPEDATA edPtr)
{
#ifndef RUN_ONLY
	// Check compatibility
	if ( IS_COMPATIBLE(mV) )
		return GetPopupMenu(MN_CONDITIONS);
#endif // !defined(RUN_ONLY)
	return NULL;
}

HMENU WINAPI DLLExport GetActionMenu(mv _far *mV, fpObjInfo oiPtr, LPEDATA edPtr)
{
#ifndef RUN_ONLY
	// Check compatibility
	if ( IS_COMPATIBLE(mV) )
		return GetPopupMenu(MN_ACTIONS);
#endif // !defined(RUN_ONLY)
	return NULL;
}

HMENU WINAPI DLLExport GetExpressionMenu(mv _far *mV, fpObjInfo oiPtr, LPEDATA edPtr)
{
#ifndef RUN_ONLY
	// Check compatibility
	if ( IS_COMPATIBLE(mV) )
		return GetPopupMenu(MN_EXPRESSIONS);
#endif // !defined(RUN_ONLY)
	return NULL;
}


// -------------------------------------------------------
// GetConditionTitle / GetActionTitle / GetExpressionTitle
// -------------------------------------------------------
// Returns the title of the dialog box displayed when entering
// parameters for the condition, action or expressions, if any.
// Here, we simply return the title of the menu option
//

#ifndef RUN_ONLY
void GetCodeTitle(LPEVENTINFOS2 eiPtr, short code, short param, short mn, LPTSTR strBuf, WORD maxLen)
{
	HMENU		hMn;

	// Finds event in array
	eiPtr=GetEventInformations(eiPtr, code);

	// If a special string is to be returned
	short strID = EVINFO2_PARAMTITLE(eiPtr, param);

	if ( strID != 0 )
		LoadString(hInstLib, strID, strBuf, maxLen);
	else
	{
		// Otherwise, returns the menu option 
		if ((hMn = LoadMenu(hInstLib, MAKEINTRESOURCE(mn))) != NULL )
		{
			GetMenuString(hMn, eiPtr->menu, strBuf, maxLen, MF_BYCOMMAND);
			DestroyMenu(hMn);
		}
	}
}
#else
#define GetCodeTitle(a,b,c,d,e,f)
#endif // !defined(RUN_ONLY)

void WINAPI DLLExport GetConditionTitle(mv _far *mV, short code, short param, LPTSTR strBuf, short maxLen)
{
	GetCodeTitle((LPEVENTINFOS2)conditionsInfos, code, param, MN_CONDITIONS, strBuf, maxLen);
}
void WINAPI DLLExport GetActionTitle(mv _far *mV, short code, short param, LPTSTR strBuf, short maxLen)
{
	GetCodeTitle((LPEVENTINFOS2)actionsInfos, code, param, MN_ACTIONS, strBuf, maxLen);
}
void WINAPI DLLExport GetExpressionTitle(mv _far *mV, short code, LPTSTR strBuf, short maxLen)
{
	GetCodeTitle((LPEVENTINFOS2)expressionsInfos, code, 0, MN_EXPRESSIONS, strBuf, maxLen);
}

// -------------------------------------------------------
// GetConditionTitle / GetActionTitle / GetExpressionTitle
// -------------------------------------------------------
// From a menu ID, these routines returns the code of the condition,
// action or expression, as defined in the .H file
//

short WINAPI DLLExport GetConditionCodeFromMenu(mv _far *mV, short menuId)
{
#ifndef RUN_ONLY
	LPEVENTINFOS2	eiPtr;
	int				n;

	for (n=CND_LAST, eiPtr=(LPEVENTINFOS2)conditionsInfos; n>0 && eiPtr->menu!=menuId; n--)
		eiPtr = EVINFO2_NEXT(eiPtr);
	if (n>0) 
		return eiPtr->infos.code;
#endif // !defined(RUN_ONLY)
	return -1;
}

short WINAPI DLLExport GetActionCodeFromMenu(mv _far *mV, short menuId)
{
#ifndef RUN_ONLY
	LPEVENTINFOS2	eiPtr;
	int				n;

	for (n=ACT_LAST, eiPtr=(LPEVENTINFOS2)actionsInfos; n>0 && eiPtr->menu!=menuId; n--)
		eiPtr = EVINFO2_NEXT(eiPtr);
	if (n>0) 
		return eiPtr->infos.code;
#endif // !defined(RUN_ONLY)
	return -1;
}

short WINAPI DLLExport GetExpressionCodeFromMenu(mv _far *mV, short menuId)
{
#ifndef RUN_ONLY
	LPEVENTINFOS2	eiPtr;
	int				n;

	for (n=EXP_LAST, eiPtr=(LPEVENTINFOS2)expressionsInfos; n>0 && eiPtr->menu!=menuId; n--)
		eiPtr = EVINFO2_NEXT(eiPtr);
	if (n>0) 
		return eiPtr->infos.code;
#endif // !defined(RUN_ONLY)
	return -1;
}


// -------------------------------------------------------
// GetConditionInfos / GetActionInfos / GetExpressionInfos
// -------------------------------------------------------
// From a action / condition / expression code, returns 
// an infosEvents structure. 
//

LPINFOEVENTSV2 WINAPI DLLExport GetConditionInfos(mv _far *mV, short code)
{
#ifndef RUN_ONLY
	return &GetEventInformations((LPEVENTINFOS2)conditionsInfos, code)->infos;
#else
	return NULL;
#endif // !defined(RUN_ONLY)
}

LPINFOEVENTSV2 WINAPI DLLExport GetActionInfos(mv _far *mV, short code)
{
#ifndef RUN_ONLY
	return &GetEventInformations((LPEVENTINFOS2)actionsInfos, code)->infos;
#else
	return NULL;
#endif // !defined(RUN_ONLY)
}

LPINFOEVENTSV2 WINAPI DLLExport GetExpressionInfos(mv _far *mV, short code)
{
#ifndef RUN_ONLY
	return &GetEventInformations((LPEVENTINFOS2)expressionsInfos, code)->infos;
#else
	return NULL;
#endif // !defined(RUN_ONLY)
}


// ----------------------------------------------------------
// GetConditionString / GetActionString / GetExpressionString
// ----------------------------------------------------------
// From a action / condition / expression code, returns 
// the string to use for displaying it under the event editor
//

void WINAPI DLLExport GetConditionString(mv _far *mV, short code, LPTSTR strPtr, short maxLen)
{
#ifndef RUN_ONLY
	// Check compatibility
	if ( IS_COMPATIBLE(mV) )
		LoadString(hInstLib, GetEventInformations((LPEVENTINFOS2)conditionsInfos, code)->string, strPtr, maxLen);
#endif // !defined(RUN_ONLY)
}

void WINAPI DLLExport GetActionString(mv _far *mV, short code, LPTSTR strPtr, short maxLen)
{
#ifndef RUN_ONLY
	// Check compatibility
	if ( IS_COMPATIBLE(mV) )
		LoadString(hInstLib, GetEventInformations((LPEVENTINFOS2)actionsInfos, code)->string, strPtr, maxLen);
#endif // !defined(RUN_ONLY)
}

void WINAPI DLLExport GetExpressionString(mv _far *mV, short code, LPTSTR strPtr, short maxLen)
{
#ifndef RUN_ONLY
	// Check compatibility
	if ( IS_COMPATIBLE(mV) )
		LoadString(hInstLib, GetEventInformations((LPEVENTINFOS2)expressionsInfos, code)->string, strPtr, maxLen);
#endif // !defined(RUN_ONLY)
}

// ----------------------------------------------------------
// GetExpressionParam
// ----------------------------------------------------------
// Returns the parameter name to display in the expression editor
//
void WINAPI DLLExport GetExpressionParam(mv _far *mV, short code, short param, LPTSTR strBuf, short maxLen)
{
#if !defined(RUN_ONLY)
	short		strID;

	// Finds event in array
	LPEVENTINFOS2 eiPtr=GetEventInformations((LPEVENTINFOS2)expressionsInfos, code);

	// If a special string is to be returned
	strID = EVINFO2_PARAMTITLE(eiPtr, param);
	if ( strID != 0 )
		LoadString(hInstLib, strID, strBuf, maxLen);
	else
		*strBuf=0;
#endif // !defined(RUN_ONLY)
}

// ----------------------------------------------------------
// Custom Parameters
// ----------------------------------------------------------

// --------------------
// InitParameter
// --------------------
// Initialize the parameter.
//
void WINAPI InitParameter(mv _far *mV, short code, paramExt* pExt)
{
#if !defined(RUN_ONLY)
#endif // !defined(RUN_ONLY)
}


// --------------------
// EditParameter
// --------------------
// Edit the parameter.
//
void WINAPI EditParameter(mv _far *mV, short code, paramExt* pExt)
{
#if !defined(RUN_ONLY)
#endif // !defined(RUN_ONLY)
}

// --------------------
// GetParameterString
// --------------------
// Initialize the parameter.
//
void WINAPI GetParameterString(mv _far *mV, short code, paramExt* pExt, LPTSTR pDest, short size)
{
#if !defined(RUN_ONLY)
#endif // !defined(RUN_ONLY)
}

