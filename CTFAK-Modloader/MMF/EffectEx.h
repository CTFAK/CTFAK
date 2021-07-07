//************************************************
// EffectEd.h
//************************************************

#ifndef _EffectEx_h
#define _EffectEx_h

//**************************
//**************************
// External headers
//**************************
//**************************

#include "SurfaceDefs.h"

//**************************
//**************************
// Forwards
//**************************
//**************************

// external
class FAR cSurface;
class FAR cSurfaceImplementation;

// Types
enum {
	EFFECTPARAM_INT,
	EFFECTPARAM_FLOAT,
	EFFECTPARAM_INTFLOAT4,
	EFFECTPARAM_SURFACE,
};

// Flags
#define EFFECTFLAG_INTERNALMASK		0xF0000000
#define EFFECTFLAG_HASTEXTUREPARAMS	0x10000000
#define EFFECTFLAG_WANTSPIXELSIZE	0x00000001
#define EFFECTFLAG_WANTSMATRIX		0x00000002

typedef struct EFFECTPARAM
{
	int		nValueType;
	union {
		int						nValue;
		float					fValue;
		cSurfaceImplementation*	pTextureSurface;
	};
} EFFECTPARAM;
typedef EFFECTPARAM* LPEFFECTPARAM;

//************************************************
// CEffectEx class

class SURFACES_API CEffectEx
{	
public:
	CEffectEx();
	virtual ~CEffectEx();

	virtual BOOL	Initialize(LPCSTR pFxData, LPARAM lUserParam, DWORD dwFlags, int nParams, LPCSTR pParamNames, LPBYTE pParamTypes);
	virtual BOOL	CreateCompiledEffect(cSurface* psf);
	virtual LPVOID	GetCompiledEffect();
	virtual void	ReleaseCompiledEffect(cSurface* psf);
	virtual BOOL	IsEffectValid(cSurface* psf, LPSTR pErrorTextBuffer, int nBufferSize);

	virtual int		GetParamType(int nParamIdx);
	virtual int		GetParamIndex(LPCSTR pParamName);

	virtual DWORD	GetRGBA();
	virtual DWORD	GetFlags();

	virtual int		GetNParams()	{ return m_nParams; }
	virtual int		GetParamIntValue(int nParamIdx);
	virtual float	GetParamFloatValue(int nParamIdx);
	virtual cSurfaceImplementation*	GetParamSurfaceValue(int nParamIdx);

	virtual void	SetRGBA(DWORD dwRGBA);
	virtual void	SetFlags(DWORD dwFlags);
	virtual void	SetParamIntValue(int nParamIdx, int nParamValue);
	virtual void	SetParamFloatValue(int nParamIdx, float fParamValue);
	virtual void	SetParamSurfaceValue(int nParamIdx, cSurfaceImplementation* pSurfaceValue);

	virtual void	SetBackgroundTexture(int nTexture) { m_nBackgroundTexture = nTexture; }
	virtual int		GetBackgroundTexture() { return m_nBackgroundTexture; }

	virtual LPARAM	GetUserParam();

public:
	DWORD			m_dwRGBA;
	DWORD			m_dwFlags;
	LPCSTR			m_pFxBuf;
	LPVOID			m_pHWAEffect;
	short			m_nBackgroundTexture;
	short			m_nParams;
	LPCSTR			m_pParamNames;
	LPEFFECTPARAM	m_pParams;
	cSurfaceImplementation* m_pSf;
	LPARAM			m_lUserParam;
};

SURFACES_API CEffectEx* WINAPI	NewEffect();
SURFACES_API void WINAPI		DeleteEffect(CEffectEx* pEffect);

#endif  // _EffectEx_h
