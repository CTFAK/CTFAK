
// Mvt1.cpp : movement example
//
// Simple square movement
//
// Parameters:
//
// - Movement Speed
// - Number of segements
// - Move At Start
// - Initial dir

#include "stdafx.h"
#include "Resource.h"
#include "Mvt1.h"


enum 
{
	PROPID_MVT1_SPEED = PROPID_MVTEXT_FIRST,
	PROPID_MVT1_NOFSEGMENTS,
	PROPID_MVT1_MOVEATSTART,
	PROPID_MVT1_INITIALDIR
};

MinMaxParam	MinMaxSpeed = { 0, 100 };
MinMaxParam	MinMaxNOfSegments = { 0, 50 };
DirCtrlCreateParam	initDirParam32 = { TRUE, 32, DCS_3D | DCS_SETALL_BTNS };

PropData CMvt1::MvtProps[] = 
{
	PropData_SliderEdit(PROPID_MVT1_SPEED, IDS_MVT_SPEED, IDS_MVT_SPEED_INFO, &MinMaxSpeed),
	PropData_SliderEdit(PROPID_MVT1_NOFSEGMENTS, IDS_MVT_NOFSEGMENTS, IDS_MVT_NOFSEGMENTS_INFO, &MinMaxNOfSegments),
	PropData_CheckBox(PROPID_MVT1_MOVEATSTART, IDS_MVT_MOVEATSTART, IDS_MVT_MOVEATSTART_INFO),
	PropData_DirCtrl(PROPID_MVT1_INITIALDIR, IDS_INITDIR, IDS_INITDIR_INFO, &initDirParam32),
	PropData_End()
};

////////////////////////////////////////////////
//
// Editor & Runtime class
//

// Constructor
CMvt1::CMvt1()
{
	m_dwSpeed = 50;
	m_dwFlags = MFLAG1_MOVEATSTART;
	m_dwNOfSegments = 10;
	m_dwInitialDir=0xFFFFFFFF;
}

CMvt1::~CMvt1()
{
}

// Initialize
void CMvt1::Initialize()
{
}

// Clone
CMvt* CMvt1::Clone()
{
	CMvt1* pClone = new CMvt1;

	pClone->m_dwSpeed = m_dwSpeed;
	pClone->m_dwFlags = m_dwFlags;
	pClone->m_dwNOfSegments= m_dwNOfSegments;
	pClone->m_dwInitialDir=m_dwInitialDir;

	return pClone;
}

// Operator ==
//
// Note: the class of pMvt is always the same as the current class
//
BOOL CMvt1::IsEqual(CMvt* pMvt)
{
	CMvt1* pMvt1 = (CMvt1*)pMvt;

	if ( m_dwSpeed != pMvt1->m_dwSpeed )
		return FALSE;
	if ( m_dwFlags != pMvt1->m_dwFlags )
		return FALSE;
	if ( m_dwNOfSegments!= pMvt1->m_dwNOfSegments)
		return FALSE;
	if ( m_dwInitialDir!= pMvt1->m_dwInitialDir )
		return FALSE;

	return TRUE;
}

HINSTANCE CMvt1::GetModuleHandle()
{
	return hInst;
}

// Retrieves the player (MVCONT_COMPUTER = computer controlled movement, MVCONT_PLAYER1+ = player number)
// Note: the Player property is automatically added by MMF
int CMvt1::GetPlayer()
{
	return MVCONT_COMPUTER;
}

void CMvt1::SetPlayer(int nPlayer)
{
	// Nothing to do: computer controlled movement
}

// Returns TRUE if move at start
//
// Note: the Move At Start property is not automatically added by MMF. 
//       You must add it to your properties if you need it.

BOOL CMvt1::MoveAtStart()
{
	return (m_dwFlags&MFLAG1_MOVEATSTART)!=0;
}

// Returns the initial direction (32-bit mask)
//
// Note: the Initial Direction property is not automatically added by MMF. 
//       You must add it to your properties if you need it.
//
int CMvt1::DirAtStart()
{
	return m_dwInitialDir;
}


// Properties
void CMvt1::GetProperties(CMvtPropMgr* pMvtPropMgr, BOOL bMasterItem)
{
	pMvtPropMgr->AddProps(MvtProps);
}

void CMvt1::RemoveProperties(CMvtPropMgr* pMvtPropMgr)
{
	pMvtPropMgr->RemoveProps(MvtProps);
}

CPropValue* CMvt1::GetPropertyValue(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	LPARAM lValue = 0;

	switch (nPropID) 
	{
	case PROPID_MVT1_SPEED:
		lValue = m_dwSpeed;
		break;

	case PROPID_MVT1_NOFSEGMENTS:
		lValue = m_dwNOfSegments;
		break;
	
	case PROPID_MVT1_INITIALDIR:
		{
			PropDirValue pv;
			memset(&pv, 0, sizeof(PropDirValue));
			pv.selDir32 = m_dwInitialDir;
			pv.numDirs = 32;
			return new CPropDataValue(sizeof(PropDirValue), (LPBYTE)&pv);
		}
		break;
	}
	return new CPropDWordValue(lValue);
}

void CMvt1::SetPropertyValue(CMvtPropMgr* pMvtPropMgr, UINT nPropID, CPropValue* pValue)
{
	if ( pValue == NULL )
		return;

	switch (nPropID) 
	{
	case PROPID_MVT1_SPEED:
		m_dwSpeed = ((CPropDWordValue*)pValue)->m_dwValue;
		break;
	case PROPID_MVT1_NOFSEGMENTS:
		m_dwNOfSegments= ((CPropDWordValue*)pValue)->m_dwValue;
		break;
	case PROPID_MVT1_INITIALDIR:
		{
			PropDirValue* pv = (PropDirValue*)((CPropDataValue*)pValue)->m_pData;
			if ( pv != NULL )
				m_dwInitialDir = pv->selDir32;
		}
		break;
	}
}

int CMvt1::GetPropertyCheck(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	switch (nPropID) 
	{
	case PROPID_MVT1_MOVEATSTART:
		return ((m_dwFlags&MFLAG1_MOVEATSTART)!=0);
	}
	return FALSE;
}

void CMvt1::SetPropertyCheck(CMvtPropMgr* pMvtPropMgr, UINT nPropID, int nCheck)
{
	switch (nPropID) 
	{
	case PROPID_MVT1_MOVEATSTART:
		m_dwFlags&=~MFLAG1_MOVEATSTART;
		if (nCheck)
			m_dwFlags|=MFLAG1_MOVEATSTART;
		break;
	}
}

BOOL CMvt1::IsPropertyEnabled(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	return TRUE;
}

BOOL CMvt1::EditProperty(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	return FALSE;
}

LPARAM CMvt1::GetPropertyCreateParam(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	return NULL;
}

void CMvt1::ReleasePropertyCreateParam(CMvtPropMgr* pMvtPropMgr, UINT nPropID, LPARAM lCreateParam)
{
}

BOOL CMvt1::Load(LPBYTE pBuf, DWORD dwSize, BOOL bRuntime)
{
	BYTE b;
	LPBYTE pb = pBuf;

	// Check size
	if ( dwSize != sizeof(BYTE) + sizeof(DWORD) * 4 )
		return FALSE;

	// Version number
	b = *pb++;
	if ( b > 0 )
		return FALSE;

	// Flags
	m_dwFlags = *((LPDWORD)pb);
	pb += sizeof(DWORD);

	// Speed
	m_dwSpeed = *((LPDWORD)pb);
	pb += sizeof(DWORD);

	// Segment Size
	m_dwNOfSegments = *((LPDWORD)pb);
	pb += sizeof(DWORD);

	// Initial direction
	m_dwInitialDir = *((LPDWORD)pb);
	pb += sizeof(DWORD);

	return TRUE;
}

BOOL CMvt1::Save(LPBYTE pBuf, LPDWORD pDWSize, BOOL bRuntime)
{
	LPBYTE pb = pBuf;
	BYTE b;

	if ( pBuf != NULL )
	{
		// Version number
		b = 0;
		*pb++ = b;

		// Flags
		*((LPDWORD)pb) = m_dwFlags;
		pb += sizeof(DWORD);

		// Speed
		*((LPDWORD)pb) = m_dwSpeed;
		pb += sizeof(DWORD);

		// Segment Size
		*((LPDWORD)pb) = m_dwNOfSegments;
		pb += sizeof(DWORD);

		// Initial Direction
		*((LPDWORD)pb) = m_dwInitialDir;
		pb += sizeof(DWORD);
	}
	*pDWSize = sizeof(BYTE) + sizeof(DWORD) * 4;

	return TRUE;
}

BOOL CMvt1::IsActionEnabled(short action)
{
	switch(action)
	{
	case ACT_EXTSTOP:
	case ACT_EXTSTART:
	case ACT_EXTBOUNCE:
	case ACT_EXTREVERSE:
		return TRUE;	

	case ACT_EXTSETGRAVITY:
	case ACT_EXTSPEED:
	case ACT_EXTWRAP:
	case ACT_EXTMAXSPEED:
	case ACT_EXTSETACCELERATION:
	case ACT_EXTSETDECELERATION:
	case ACT_EXTSETROTATINGSPEED:
	case ACT_EXTSETDIRECTIONS:
		break;
	}
	return FALSE;
}


CRunMvt* CMvt1::CreateRunMvt()
{
	CRunMvt1* prm = new CRunMvt1;
	return prm;
}

////////////////////////////////////////////////
//
// Runtime only class
//

CRunMvt1::CRunMvt1()
{
}

CRunMvt1::~CRunMvt1()
{
}

// Initialize the movement
// -----------------------
void CRunMvt1::Initialize(CMvt* pMvt, LPHO pHo)
{
	// Store pointer to edit data
	m_pMvt = (CMvt1*)pMvt;

	// Initialisations
	LPRO pRo=(LPRO)pHo;
	m_segmentCount=m_pMvt->m_dwNOfSegments;
	pRo->roc.rcSpeed=m_pMvt->m_dwSpeed;
	if ((m_pMvt->m_dwFlags&MFLAG1_MOVEATSTART)==0)
		pRo->roc.rcSpeed=0;

	// Finds the initial direction
	pRo->roc.rcDir=callDirAtStart(pHo, m_pMvt->m_dwInitialDir);
	m_dir=8;
}

// Moves the object
// ----------------
BOOL CRunMvt1::Move(LPHO pHo)
{
	LPRO pRo=(LPRO)pHo;

	// Makes the move
	if (pRo->roc.rcSpeed!=0)
	{
		m_segmentCount--;
		if (m_segmentCount<=0)
		{
			pRo->roc.rcDir+=m_dir;
			if (pRo->roc.rcDir>=32)
				pRo->roc.rcDir-=32;
			if (pRo->roc.rcDir<0)
				pRo->roc.rcDir+=32;
			m_segmentCount=m_pMvt->m_dwNOfSegments;
		}
	}
	callAnimations(pHo, ANIMID_WALK);
	callMoveIt(pHo);

	// The object has been moved
	return TRUE;	
}

// Changes both X and Y position
// -----------------------------
void CRunMvt1::SetPosition(LPHO pHo, int x, int y)
{
	pHo->hoX=(short)x;
	pHo->hoY=(short)y;
}

// Changes X position
// ------------------
void CRunMvt1::SetXPosition(LPHO pHo, int x)
{
	pHo->hoX=(short)x;
}

// Changes Y position
// ------------------
void CRunMvt1::SetYPosition(LPHO pHo, int y)
{
	pHo->hoY=(short)y;
}

// Stops the object
// ----------------
void CRunMvt1::Stop(LPHO pHo, BOOL bCurrent)
{
	LPRO pRo=(LPRO)pHo;
	if (!bCurrent)
	{
		pRo->roc.rcSpeed=0;
		return;
	}
	int x=0, y=0;
	callApproachObject(x, y, pHo, pHo->hoX, pHo->hoY, pRo->roc.rcOldX, pRo->roc.rcOldY, 0, CM_TEST_PLATFORM);
	pHo->hoX=x;
	pHo->hoY=y;
	pRo->roc.rcSpeed=0; 
}

// Bounces the object
// ------------------
void CRunMvt1::Bounce(LPHO pHo, BOOL bCurrent)
{
	LPRO pRo=(LPRO)pHo;
	if (!bCurrent)
	{
		pRo->roc.rcDir+=16;
		if (pRo->roc.rcDir>=32)
			pRo->roc.rcDir-=32;
		m_dir=-m_dir;
		return;
	}
	int x=0, y=0;
	callApproachObject(x, y, pHo, pHo->hoX, pHo->hoY, pRo->roc.rcOldX, pRo->roc.rcOldY, 0, CM_TEST_PLATFORM);
	pHo->hoX=x;
	pHo->hoY=y;
	pRo->roc.rcDir+=16;
	if (pRo->roc.rcDir>=32)
		pRo->roc.rcDir-=32;
	m_dir=-m_dir;
}

// Go in reverse
// -------------
void CRunMvt1::Reverse(LPHO pHo)
{
	LPRO pRo=(LPRO)pHo;

	pRo->roc.rcDir+=16;
	if (pRo->roc.rcDir>=32)
		pRo->roc.rcDir-=32;
	m_dir=-m_dir;
}

// Restart the movement
// --------------------
void CRunMvt1::Start(LPHO pHo)
{
	LPRO pRo=(LPRO)pHo;

	pRo->roc.rcSpeed=m_pMvt->m_dwSpeed;
}

// Changes the speed
// -----------------
void CRunMvt1::SetSpeed(LPHO pHo, int speed)
{
}

// Changes the maximum speed
// -------------------------
void CRunMvt1::SetMaxSpeed(LPHO pHo, int speed)
{
}

// Changes the direction
// ---------------------
void CRunMvt1::SetDir(LPHO pHo, int speed)
{
}

// Changes the acceleration
// ------------------------
void CRunMvt1::SetAcc(LPHO pHo, int acc)
{
}

// Changes the deceleration
// ------------------------
void CRunMvt1::SetDec(LPHO pHo, int dec)
{
}

// Changes the rotation speed
// --------------------------
void CRunMvt1::SetRotSpeed(LPHO pHo, int speed)
{
}

// Changes the authorised directions out of 8
// ------------------------------------------
void CRunMvt1::Set8Dirs(LPHO pHo, int dirs)
{
}

// Changes the gravity
// -------------------
void CRunMvt1::SetGravity(LPHO pHo, int gravity)
{
}

// Extension Actions entry
// -----------------------
DWORD CRunMvt1::ActionEntry(LPHO pHo, int action, DWORD param)
{
	return 0;
}

// Returns the speed
// -----------------
int CRunMvt1::GetSpeed(LPHO pHo)
{
	return ((LPRO)pHo)->roc.rcSpeed;
}

// Returns the acceleration
// ------------------------
int CRunMvt1::GetAcceleration(LPHO pHo)
{
	return 0;
}

// Returns the deceleration
// ------------------------
int CRunMvt1::GetDeceleration(LPHO pHo)
{
	return 0;
}

// Returns the gravity
// -------------------
int CRunMvt1::GetGravity(LPHO pHo)
{
	return 0;
}


// Reserved for future extension
// -----------------------------
DWORD CRunMvt1::Extension(LPHO pHo, int function, DWORD param)
{
	return 0;
}



