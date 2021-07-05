// ============================================================================
//
// This file are where the Conditions/Actions/Expressions are defined.
// You can manually enter these, or use CICK (recommended)
// See the Extension FAQ in this SDK for more info and where to download it
//
// ============================================================================

// Common Include
#include	"common.h"
#include	"CfcFile.h"

// Quick memo: content of the eventInformations arrays
// ---------------------------------------------------
// Menu ID
// String ID
// Code
// Flags
// Number_of_parameters
// Parameter_type [Number_of_parameters]
// Parameter_TitleString [Number_of_parameters]

// Definitions of parameters for each condition
short conditionsInfos[]=
		{
//		IDMN_CONDITION, M_CONDITION, CND_CONDITION, EVFLAGS_ALWAYS, 3, PARAM_EXPRESSION, PARAM_EXPRESSION, PARAM_EXPRESSION, M_CND_P1, M_CND_P2, M_CND_P3,
		0, 0, 0, 0, 0
		};

// Definitions of parameters for each action
short actionsInfos[]=
		{
		IDMN_ACT_SETAT, M_ACT_SETAT,	ACT_SETAT,	0, 2, PARAM_EXPRESSION, PARAM_EXPRESSION, IDS_SETAT_OFFSET, IDS_SETAT_VALUE,
		IDMN_ACT_LOADFILE, M_ACT_LOADFILE,	ACT_LOADFILE,	0, 1, PARAM_FILENAME, 0,
		IDMN_ACT_SAVEFILE, M_ACT_SAVEFILE,	ACT_SAVEFILE,	0, 1, PARAM_FILENAME, 0,
		};

// Definitions of parameters for each expression
short expressionsInfos[]=
		{
		IDMN_EXP_GETAT,		M_EXP_GETAT,	EXP_GETAT,		0, 1, EXPPARAM_LONG,	IDS_GETAT_OFFSET,
		IDMN_EXP_GETSIZE,	M_EXP_GETSIZE,	EXP_GETSIZE,	0, 0,
		IDMN_EXP_GETFILENAME, M_EXP_GETFILENAME, EXP_GETFILENAME, EXPFLAG_STRING, 0,
		};



// ============================================================================
//
// CONDITION ROUTINES
// 
// ============================================================================

/*
// -----------------
// Sample Condition
// -----------------
// Returns TRUE when the two values are equal!
// 

long WINAPI DLLExport Condition(LPRDATA rdPtr, long param1, long param2)
{

//  **** Still use this method for 1 or 2 parameters ****	
//	if (param1==param2)	
//		return TRUE;

	long p1 = CNC_GetParameter(rdPtr);
	long p2 = CNC_GetParameter(rdPtr);
	long p3 = CNC_GetParameter(rdPtr);

	if ((p1 + p2)==p3)
		return TRUE;
		 
	return FALSE;
}
*/


// ============================================================================
//
// ACTIONS ROUTINES
// 
// ============================================================================

// -----------------
// Set Text
// -----------------
// 
short WINAPI DLLExport ActSetAt(LPRDATA rdPtr, long param1, long param2)
{
	// Parameters
	DWORD dwOffset = (DWORD)param1;
	BYTE bValue = (BYTE)param2;

	// Alloc / Realloc
	if ( dwOffset >= rdPtr->dwBufferSize )
	{
		// Allocate
		if ( rdPtr->pBuffer == NULL )
		{
			rdPtr->pBuffer = (LPBYTE)calloc(dwOffset+1, 1);
			if ( rdPtr->pBuffer != NULL )
				rdPtr->dwBufferSize = dwOffset+1;
		}

		// Reallocate
		else
		{
			LPBYTE pb = (LPBYTE)realloc(rdPtr->pBuffer, dwOffset+1);
			if ( pb != NULL )
			{
				rdPtr->pBuffer = pb;

				// Initialize new values
				memset(&rdPtr->pBuffer[rdPtr->dwBufferSize], 0, (dwOffset+1 - rdPtr->dwBufferSize));

				rdPtr->dwBufferSize = dwOffset+1;
			}
		}
	}

	// Store value
	if ( dwOffset < rdPtr->dwBufferSize )
		rdPtr->pBuffer[dwOffset] = bValue;

	return 0;
}

// -----------------
// Load File
// -----------------
// 

void LoadDataFile (LPRDATA rdPtr, LPCSTR pFilename)
{
	LPRH	rhPtr = rdPtr->rHo.hoAdRunHeader;
	HANDLE	hf = INVALID_HANDLE_VALUE;

	do {	
		if ( pFilename == NULL || *pFilename == 0 )
			break;

		// Copy filename to temp buffer
		char fname[MAX_PATH];
		strcpy(fname, pFilename);

		// Ask MMF2 to open the file (opens external file and embedded files, and downloads files in Vitalize mode)
		DWORD dwSize;
		hf = rhPtr->rh4.rh4Mv->mvOpenHFile(fname, &dwSize, 0);
		if ( hf == INVALID_HANDLE_VALUE )
			break;

		// Free old buffer
		if ( rdPtr->pBuffer != NULL )
			free(rdPtr->pBuffer);
		rdPtr->dwBufferSize = 0;

		// Allocate new buffer
		rdPtr->pBuffer = (LPBYTE)calloc(dwSize, 1);
		if ( rdPtr->pBuffer != NULL )
		{
			rdPtr->dwBufferSize = dwSize;
			File_Read((HFILE)hf, rdPtr->pBuffer, dwSize);

			// Copy filename
			strcpy(rdPtr->sFilename, fname);
		}

	} while(FALSE);

	// Close file (opened with mvOpenHFile)
	if ( hf != INVALID_HANDLE_VALUE )
		rhPtr->rh4.rh4Mv->mvCloseHFile(hf);
}

short WINAPI DLLExport ActLoadFile(LPRDATA rdPtr, long param1, long param2)
{
	LPSTR pFilename = (LPSTR)param1;

	// Different file?
	if ( pFilename != NULL && stricmp (rdPtr->sFilename, pFilename) != 0 )
	{
		// Load new file
		LoadDataFile(rdPtr, pFilename);
	}

	return 0;
}

// -----------------
// Save File
// -----------------
// 

short WINAPI DLLExport ActSaveFile(LPRDATA rdPtr, long param1, long param2)
{
	LPSTR pFilename = (LPSTR)param1;

	// Different file?
	if ( pFilename != NULL && *pFilename != 0 && stricmp (rdPtr->sFilename, pFilename) != 0 )
	{
		// Create file
		HFILE hf = File_Create(pFilename);
		if ( hf != HFILE_ERROR )
		{
			// Write buffer
			if ( rdPtr->dwBufferSize != 0 )
				File_Write(hf, rdPtr->pBuffer, rdPtr->dwBufferSize);

			// Copy filename
			strcpy(rdPtr->sFilename, pFilename);

			// Close file
			File_Close(hf);
		}
	}

	return 0;
}


// ============================================================================
//
// EXPRESSIONS ROUTINES
// 
// ============================================================================

// -----------------
// Get At
// -----------------

long WINAPI DLLExport ExpGetAt(LPRDATA rdPtr,long param1)
{
	// Get first expression parameter (offset in data buffer)
	DWORD dwOffset = (DWORD)CNC_GetFirstExpressionParameter(rdPtr, param1, TYPE_INT);

	if ( dwOffset < rdPtr->dwBufferSize )
		return rdPtr->pBuffer[dwOffset];

	return 0L;
}

// -----------------
// Get Buffer Size
// -----------------

long WINAPI DLLExport ExpGetSize(LPRDATA rdPtr,long param1)
{
	return (long)rdPtr->dwBufferSize;
}

// -----------------
// Get Filename
// -----------------

long WINAPI DLLExport ExpGetFilename(LPRDATA rdPtr,long param1)
{
	if ( rdPtr->sFilename == NULL )
		return (long)"";

	// Setting the HOF_STRING flag lets MMF know that you return a string.
	rdPtr->rHo.hoFlags |= HOF_STRING;

	return (long)rdPtr->sFilename;
}


// ----------------------------------------------------------
// Condition / Action / Expression jump table
// ----------------------------------------------------------
// Contains the address inside the extension of the different
// routines that handle the action, conditions and expressions.
// Located at the end of the source for convinience
// Must finish with a 0
//
long (WINAPI * ConditionJumps[])(LPRDATA rdPtr, long param1, long param2) = 
			{ 
			0
			};
	
short (WINAPI * ActionJumps[])(LPRDATA rdPtr, long param1, long param2) =
			{
			ActSetAt,
			ActLoadFile,
			ActSaveFile,
			0
			};

long (WINAPI * ExpressionJumps[])(LPRDATA rdPtr, long param) = 
			{     
			ExpGetAt,
			ExpGetSize,
			ExpGetFilename,
			0
		};
