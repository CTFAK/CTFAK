#if !defined(AFX_SETUPDIALOG_H__53D6F2B4_270E_42B3_BFC7_E13C5AA68434__INCLUDED_)
#define AFX_SETUPDIALOG_H__53D6F2B4_270E_42B3_BFC7_E13C5AA68434__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SetupDialog.h : header file
//

#ifndef RUN_ONLY

/////////////////////////////////////////////////////////////////////////////
// CSetupDialog dialog

class CSetupDialog : public CDialog
{
// Construction
public:
	CSetupDialog(LPMV mV, LPEDATA edPtr, CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CSetupDialog)
	enum { IDD = DB_SETUP };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CSetupDialog)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	LPMV		m_mV;			// MMF2 variables
	LPEDATA		m_edPtr;		// object data

	// Generated message map functions
	//{{AFX_MSG(CSetupDialog)
	afx_msg void OnHelp();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // RUN_ONLY

#endif // !defined(AFX_SETUPDIALOG_H__53D6F2B4_270E_42B3_BFC7_E13C5AA68434__INCLUDED_)
