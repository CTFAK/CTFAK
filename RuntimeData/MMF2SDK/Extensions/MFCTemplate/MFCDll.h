// MFCDll.h : main header file for the MFCDLL DLL
//

#if !defined(AFX_MFCDLL_H__04688E92_2DFF_4405_ABFA_337C2FF026CA__INCLUDED_)
#define AFX_MFCDLL_H__04688E92_2DFF_4405_ABFA_337C2FF026CA__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

/////////////////////////////////////////////////////////////////////////////
// CMFCDllApp
// See MFCDll.cpp for the implementation of this class
//

class CMFCDllApp : public CWinApp
{
public:
	CMFCDllApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMFCDllApp)
	virtual BOOL InitInstance();
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

	//{{AFX_MSG(CMFCDllApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

extern CMFCDllApp theApp;

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MFCDLL_H__04688E92_2DFF_4405_ABFA_337C2FF026CA__INCLUDED_)
