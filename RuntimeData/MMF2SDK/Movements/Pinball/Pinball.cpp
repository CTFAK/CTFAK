///////////////////////////////////////////////////////////////////////////////
// 
// MMF MOVEMENTS EXTENSION KIT : PINBALL MOVEMENT
// 
///////////////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "Resource.h"
#include "Pinball.h"

enum 
{
	PROPID_INITIALSPEED = PROPID_MVTEXT_FIRST,
	PROPID_INITIALDIR,
	PROPID_GRAVITY,
	PROPID_MOVEATSTART,
	PROPID_DECELERATION
};

MinMaxParam	MinMax = { 0, 100 };
DirCtrlCreateParam	InitialDirParam32 = { TRUE, 32, DCS_3D | DCS_SETALL_BTNS };

PropData CPinball::MvtProps[] = 
{
	PropData_SliderEdit(PROPID_GRAVITY, IDS_GRAVITY, IDS_GRAVITY_INFO, &MinMax),
	PropData_SliderEdit(PROPID_DECELERATION, IDS_DECELERATION, IDS_DECELERATION_INFO, &MinMax),
	PropData_CheckBox(PROPID_MOVEATSTART, IDS_MOVEATSTART, IDS_MOVEATSTART_INFO),
	PropData_SliderEdit(PROPID_INITIALSPEED, IDS_INITIALSPEED, IDS_INITIALSPEED_INFO, &MinMax),
	PropData_DirCtrl(PROPID_INITIALDIR, IDS_INITIALDIR, IDS_INITIALDIR_INFO, &InitialDirParam32),
	PropData_End()
};

////////////////////////////////////////////////
//
// Editor & Runtime class
//

// Constructor
CPinball::CPinball()
{
	m_dwInitialSpeed = 50;
	m_dwInitialDir=0xFFFFFFFF;
	m_dwGravity = 25;
	m_dwDeceleration=0;
	m_dwFlags=EFLAG_MOVEATSTART;
}

CPinball::~CPinball()
{
}

// Initialize
void CPinball::Initialize()
{
}

// Clone
CMvt* CPinball::Clone()
{
	CPinball* pClone = new CPinball;

	pClone->m_dwInitialSpeed = m_dwInitialSpeed;
	pClone->m_dwInitialDir=m_dwInitialDir;
	pClone->m_dwGravity=m_dwGravity;
	pClone->m_dwDeceleration=m_dwDeceleration;
	pClone->m_dwFlags=m_dwFlags;

	return pClone;
}

// Operator ==
//
// Note: the class of pMvt is always the same as the current class
//
BOOL CPinball::IsEqual(CMvt* pMvt)
{
	CPinball* pMvt1 = (CPinball*)pMvt;

	if ( m_dwInitialSpeed != pMvt1->m_dwInitialSpeed )
		return FALSE;
	if ( m_dwInitialDir != pMvt1->m_dwInitialDir )
		return FALSE;
	if ( m_dwGravity != pMvt1->m_dwGravity )
		return FALSE;
	if ( m_dwDeceleration != pMvt1->m_dwDeceleration )
		return FALSE;
	if ( m_dwFlags != pMvt1->m_dwFlags )
		return FALSE;

	return TRUE;
}

HINSTANCE CPinball::GetModuleHandle()
{
	return hInst;
}

// Retrieves the player (MVCONT_COMPUTER = computer controlled movement, MVCONT_PLAYER1+ = player number)
// Note: the Player property is automatically added by MMF
int CPinball::GetPlayer()
{
	return MVCONT_COMPUTER;
}

void CPinball::SetPlayer(int nPlayer)
{
	// Nothing to do: computer controlled movement
}

// Returns TRUE if move at start
//
// Note: the Move At Start property is not automatically added by MMF. 
//       You must add it to your properties if you need it.

BOOL CPinball::MoveAtStart()
{
	return ((m_dwFlags & EFLAG_MOVEATSTART) != 0);
}

// Returns the initial direction (32-bit mask)
//
// Note: the Initial Direction property is not automatically added by MMF. 
//       You must add it to your properties if you need it.
//
int CPinball::DirAtStart()
{
	return m_dwInitialDir;
}


// Properties
void CPinball::GetProperties(CMvtPropMgr* pMvtPropMgr, BOOL bMasterItem)
{
	pMvtPropMgr->AddProps(MvtProps);
}

void CPinball::RemoveProperties(CMvtPropMgr* pMvtPropMgr)
{
	pMvtPropMgr->RemoveProps(MvtProps);
}

CPropValue* CPinball::GetPropertyValue(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	LPARAM lValue = 0;

	switch (nPropID) 
	{
	case PROPID_INITIALSPEED:
		lValue = m_dwInitialSpeed;
		break;

	case PROPID_DECELERATION:
		lValue = m_dwDeceleration;
		break;

	case PROPID_GRAVITY:
		lValue = m_dwGravity;
		break;

	case PROPID_INITIALDIR:
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

void CPinball::SetPropertyValue(CMvtPropMgr* pMvtPropMgr, UINT nPropID, CPropValue* pValue)
{
	if ( pValue == NULL )
		return;

	switch (nPropID) 
	{
	case PROPID_INITIALSPEED:
		m_dwInitialSpeed = ((CPropDWordValue*)pValue)->m_dwValue;
		break;

	case PROPID_DECELERATION:
		m_dwDeceleration = ((CPropDWordValue*)pValue)->m_dwValue;
		break;

	case PROPID_GRAVITY:
		m_dwGravity = ((CPropDWordValue*)pValue)->m_dwValue;
		break;

	case PROPID_INITIALDIR:
		{
			PropDirValue* pv = (PropDirValue*)((CPropDataValue*)pValue)->m_pData;
			if ( pv != NULL )
				m_dwInitialDir = pv->selDir32;
		}
		break;
	}
}

int CPinball::GetPropertyCheck(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	switch (nPropID)
	{
	case PROPID_MOVEATSTART:
		return ((m_dwFlags&EFLAG_MOVEATSTART) != 0);
	}
	return FALSE;
}

void CPinball::SetPropertyCheck(CMvtPropMgr* pMvtPropMgr, UINT nPropID, int nCheck)
{
	switch (nPropID)
	{
	case PROPID_MOVEATSTART:
		m_dwFlags&=~EFLAG_MOVEATSTART;
		if (nCheck)
			m_dwFlags|=EFLAG_MOVEATSTART;
		break;
	}
}

BOOL CPinball::IsPropertyEnabled(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	return TRUE;
}

BOOL CPinball::EditProperty(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	return FALSE;
}

LPARAM CPinball::GetPropertyCreateParam(CMvtPropMgr* pMvtPropMgr, UINT nPropID)
{
	return NULL;
}

void CPinball::ReleasePropertyCreateParam(CMvtPropMgr* pMvtPropMgr, UINT nPropID, LPARAM lCreateParam)
{
}

BOOL CPinball::Load(LPBYTE pBuf, DWORD dwSize, BOOL bRuntime)
{
	BYTE b;
	LPBYTE pb = pBuf;

	// Check size
	if ( dwSize != sizeof(BYTE) + sizeof(DWORD) * 5 )
		return FALSE;

	// Version number
	b = *pb++;
	if ( b > 0 )
		return FALSE;

	// Speed
	m_dwInitialSpeed = *((LPDWORD)pb);
	pb += sizeof(DWORD);

	// Deceleration
	m_dwDeceleration = *((LPDWORD)pb);
	pb += sizeof(DWORD);

	// Segment Size
	m_dwGravity = *((LPDWORD)pb);
	pb += sizeof(DWORD);

	// Initial direction
	m_dwInitialDir = *((LPDWORD)pb);
	pb += sizeof(DWORD);

	// Flags
	m_dwFlags = *((LPDWORD)pb);
	pb += sizeof(DWORD);

	return TRUE;
}

BOOL CPinball::Save(LPBYTE pBuf, LPDWORD pDWSize, BOOL bRuntime)
{
	LPBYTE pb = pBuf;
	BYTE b;

	if ( pBuf != NULL )
	{
		// Version number
		b = 0;
		*pb++ = b;

		// Speed
		*((LPDWORD)pb) = m_dwInitialSpeed;
		pb += sizeof(DWORD);

		// Deceleration
		*((LPDWORD)pb) = m_dwDeceleration;
		pb += sizeof(DWORD);

		// Segment Size
		*((LPDWORD)pb) = m_dwGravity;
		pb += sizeof(DWORD);

		// Initial Direction
		*((LPDWORD)pb) = m_dwInitialDir;
		pb += sizeof(DWORD);

		// Flags
		*((LPDWORD)pb) = m_dwFlags;
		pb += sizeof(DWORD);
	}
	*pDWSize = sizeof(BYTE) + sizeof(DWORD) * 5;

	return TRUE;
}

BOOL CPinball::IsActionEnabled(short action)
{
	switch(action)
	{
	case ACT_EXTSTOP:
	case ACT_EXTSTART:
	case ACT_EXTBOUNCE:
	case ACT_EXTREVERSE:
	case ACT_EXTSETGRAVITY:
	case ACT_EXTSPEED:
	case ACT_EXTSETDECELERATION:
		return TRUE;	
	}
	return FALSE;
}


CRunMvt* CPinball::CreateRunMvt()
{
	CRunPinball* prm = new CRunPinball;
	return prm;
}

////////////////////////////////////////////////
//
// Runtime only class
//

#define _PI 3.14159265359

CRunPinball::CRunPinball()
{
}

CRunPinball::~CRunPinball()
{
}

// Initialize the movement
// -----------------------
void CRunPinball::Initialize(CMvt* pMvt, LPHO pHo)
{
	LPRO pRo=(LPRO)pHo;

	// Store pointer to edit data
	m_pPinball = (CPinball*)pMvt;

	// Initialisations
	m_X=pHo->hoX;
	m_Y=pHo->hoY;
	pRo->roc.rcSpeed=m_pPinball->m_dwInitialSpeed;

	// Finds the initial direction
	pRo->roc.rcDir=callDirAtStart(pHo, m_pPinball->m_dwInitialDir);
	double angle=(pRo->roc.rcDir*2*_PI)/32.0;

	// Calculates the vectors
	m_gravity=m_pPinball->m_dwGravity;
	m_deceleration=m_pPinball->m_dwDeceleration;
	m_xVector=pRo->roc.rcSpeed*cos(angle);
	m_yVector=-pRo->roc.rcSpeed*sin(angle);

	// Move at start
	m_flags=0;
	if ((m_pPinball->m_dwFlags&EFLAG_MOVEATSTART)==0)
		m_flags|=MFLAG_STOPPED;
}

// Moves the object
// ----------------

void CRunPinball::GetAngle(double vX, double vY, double& angle, double& vector)
{
	vector=sqrt(vX*vX+vY*vY);
	if (vector==0.0)
	{
		angle=0.0;
		return;
	}

	angle=acos(vX/vector);

	if (vY>0.0)
	{
		angle=2.0*PI-angle;
	}
}

BOOL CRunPinball::Move(LPHO pHo)
{
	LPRO pRo=(LPRO)pHo;

	// Stopped?
	if (m_flags&MFLAG_STOPPED)
	{
		callAnimations(pHo, ANIMID_STOP);
		callCollisions(pHo);
		return FALSE;
	}

	// Increase Y speed
	m_yVector+=m_gravity/10.0;

	// Get the current vector of the ball
	double angle, vector;
	GetAngle(m_xVector, m_yVector, angle, vector);	// Get the angle and vector
	vector-=m_deceleration/50.0;
	if (vector<0.0)
		vector=0.0;
	m_xVector=vector*cos(angle);					// Restores X and Y speeds
	m_yVector=-vector*sin(angle);
	
	// Calculate the new position
	m_X=m_X+(m_xVector/10.0);
	m_Y=m_Y+(m_yVector/10.0);

	// Performs the animation
	pRo->roc.rcSpeed=(int)vector;
	if (pRo->roc.rcSpeed>100)
		pRo->roc.rcSpeed=100;
	pRo->roc.rcDir=(int)((angle*32)/(2.0*_PI));
	callAnimations(pHo, ANIMID_WALK);

	// detects the collisions
	pHo->hoX=(short)m_X;
	pHo->hoY=(short)m_Y;
	callCollisions(pHo);

	// The object has been moved
	return TRUE;	
}

// Changes both X and Y position
// -----------------------------
void CRunPinball::SetPosition(LPHO pHo, int x, int y)
{
	pHo->hoX=(short)x;
	pHo->hoY=(short)y;
	m_X=x;
	m_Y=y;
}

// Changes X position
// ------------------
void CRunPinball::SetXPosition(LPHO pHo, int x)
{
	pHo->hoX=(short)x;
	m_X=x;
}

// Changes Y position
// ------------------
void CRunPinball::SetYPosition(LPHO pHo, int y)
{
	pHo->hoY=(short)y;
	m_Y=y;
}

// Stops the object
// ----------------
void CRunPinball::Stop(LPHO pHo, BOOL bCurrent)
{
	m_flags|=MFLAG_STOPPED;
}

// Bounces the object
// ------------------
void CRunPinball::Bounce(LPHO pHo, BOOL bCurrent)
{
	LPRO pRo=(LPRO)pHo;
	if (!bCurrent)
	{
		m_xVector=-m_xVector;
		m_yVector=-m_yVector;
		return;
	}

	// Takes the object against the obstacle
	int x=0, y=0;
	callApproachObject(x, y, pHo, pHo->hoX, pHo->hoY, pRo->roc.rcOldX, pRo->roc.rcOldY, 0, CM_TEST_PLATFORM);
	pHo->hoX=x;
	pHo->hoY=y;
	m_X=x;
	m_Y=y;

	// Get the current vector of the ball
	double angle, vector;
	GetAngle(m_xVector, m_yVector, angle, vector);

	// Finds the shape of the obstacle
	double a;
	double aFound=-1000;
	for (a=0.0; a<2.0*_PI; a+=_PI/32.0)
	{
		double xVector=16*cos(angle+a);
		double yVector=-16*sin(angle+a);
		double x=m_X+xVector;
		double y=m_Y+yVector;

		if (callTestPosition(pHo, (int)x, (int)y, 0, CM_TEST_PLATFORM))
		{
			aFound=a;
			break;
		}
	}
	
	// If nothing is found, simply go backward
	if (aFound==-1000)
	{
		m_xVector=-m_xVector;
		m_yVector=-m_yVector;
	}
	else
	{
		// The angle is found, proceed with the bounce
		angle+=aFound*2;
		if (angle>2.0*_PI) 
			angle-=2.0*_PI;

		// Restores the speed vectors
		m_xVector=vector*cos(angle);
		m_yVector=-vector*sin(angle);
	}
}	

// Go in reverse
// -------------
void CRunPinball::Reverse(LPHO pHo)
{
	m_xVector=-m_xVector;
	m_yVector=-m_yVector;
}

// Restart the movement
// --------------------
void CRunPinball::Start(LPHO pHo)
{
	m_flags&= ~MFLAG_STOPPED;
}

// Changes the speed
// -----------------
void CRunPinball::SetSpeed(LPHO pHo, int speed)
{
	LPRO pRo=(LPRO)pHo;
	pRo->roc.rcSpeed=speed;

	// Gets the current speed vector
	double vector, angle;
	GetAngle(m_xVector, m_yVector, angle, vector);

	// Changes the current x and y vectors
	m_xVector=speed*cos(angle);
	m_yVector=-speed*sin(angle);
}

// Changes the maximum speed
// -------------------------
void CRunPinball::SetMaxSpeed(LPHO pHo, int speed)
{
}

// Changes the direction
// ---------------------
void CRunPinball::SetDir(LPHO pHo, int dir)
{
	LPRO pRo=(LPRO)pHo;
	pRo->roc.rcDir=dir;

	// Get the current speed vector
	double vector, angle;
	GetAngle(m_xVector, m_yVector, angle, vector);

	// Converts the angle in 32 directions to a angle in radian
	angle=dir*2.0*_PI/32.0;

	// Changes the speeds
	m_xVector=vector*cos(angle);
	m_yVector=-vector*sin(angle);
}

// Changes the acceleration
// ------------------------
void CRunPinball::SetAcc(LPHO pHo, int acc)
{
}

// Changes the deceleration
// ------------------------
void CRunPinball::SetDec(LPHO pHo, int dec)
{
	m_deceleration=dec;
}

// Changes the rotation speed
// --------------------------
void CRunPinball::SetRotSpeed(LPHO pHo, int speed)
{
}

// Changes the authorised directions out of 8
// ------------------------------------------
void CRunPinball::Set8Dirs(LPHO pHo, int dirs)
{
}

// Changes the gravity
// -------------------
void CRunPinball::SetGravity(LPHO pHo, int gravity)
{
	m_gravity=gravity;
}

// Returns the speed
// -----------------
int CRunPinball::GetSpeed(LPHO pHo)
{
	return ((LPRO)pHo)->roc.rcSpeed;
}

// Returns the acceleration
// ------------------------
int CRunPinball::GetAcceleration(LPHO pHo)
{
	return 0;
}

// Returns the deceleration
// ------------------------
int CRunPinball::GetDeceleration(LPHO pHo)
{
	return (int)m_deceleration;
}

// Returns the gravity
// -------------------
int CRunPinball::GetGravity(LPHO pHo)
{
	return (int)m_gravity;
}


// Extension Actions entry
// -----------------------
DWORD CRunPinball::ActionEntry(LPHO pHo, int action, DWORD param)
{
	m_gravity=param;

	return 0;
}

// Reserved for future extension
// -----------------------------
DWORD CRunPinball::Extension(LPHO pHo, int function, DWORD param)
{
	return 0;
}

