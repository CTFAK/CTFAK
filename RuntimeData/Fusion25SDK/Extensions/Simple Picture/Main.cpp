// ============================================================================
//
// This file are where the Conditions/Actions/Expressions are defined.
// You can manually enter these, or use CICK (recommended)
// See the Extension FAQ in this SDK for more info and where to download it
//
// ============================================================================

// Common Include
#include	"common.h"

// Prototypes
extern void LoadImageFile(LPRDATA rdPtr, LPCTSTR pFileName);

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
		IDMN_ACT_LOADFILE, M_ACT_LOADFILE,	ACT_LOADFILE,	0, 1, PARAM_FILENAME, 0,
		};

// Definitions of parameters for each expression
short expressionsInfos[]=
		{
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
// Load File
// -----------------
// 
short WINAPI DLLExport ActLoadFile(LPRDATA rdPtr, long param1, long param2)
{
	LPCTSTR pFilename = (LPCTSTR)param1;

	// Different file?
	if ( pFilename != NULL && _tcsicmp (rdPtr->sFilename, pFilename) != 0 )
	{
		// Load new file
		LoadImageFile(rdPtr, pFilename);

		// Redraw object
		callRunTimeFunction(rdPtr, RFUNCTION_REDRAW, 0, 0);
	}

	return 0;
}


// ============================================================================
//
// EXPRESSIONS ROUTINES
// 
// ============================================================================

// -----------------
// Get Filename
// -----------------

long WINAPI DLLExport ExpGetFilename(LPRDATA rdPtr,long param1)
{
	if ( rdPtr->sFilename == NULL )
		return (long)_T("");

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
			ActLoadFile,
			0
			};

long (WINAPI * ExpressionJumps[])(LPRDATA rdPtr, long param) = 
			{     
			ExpGetFilename,
			0
		};