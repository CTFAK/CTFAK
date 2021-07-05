// SetupDialog.cpp : implementation file
//

#include "stdafx.h"
#include "SetupDialog.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

extern LPCSTR WINAPI GetHelpFileName();

#ifndef RUN_ONLY

/////////////////////////////////////////////////////////////////////////////
// CSetupDialog dialog


CSetupDialog::CSetupDialog(LPMV mV, LPEDATA edPtr, CWnd* pParent /*=NULL*/)
	: CDialog(CSetupDialog::IDD, pParent)
{
	//{{AFX_DATA_INIT(CSetupDialog)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT

	m_mV = mV;
	m_edPtr = edPtr;
}


void CSetupDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CSetupDialog)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CSetupDialog, CDialog)
	//{{AFX_MSG_MAP(CSetupDialog)
	ON_BN_CLICKED(IDHELP, OnHelp)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSetupDialog message handlers

void CSetupDialog::OnHelp() 
{
	m_mV->mvHelp(GetHelpFileName(), 0 /*HH_DISPLAY_TOPIC*/, NULL /*(LPARAM)"index.html"*/);
}

#endif // RUN_ONLY
