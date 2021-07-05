// ============================================================================
//
// This file are where the Conditions/Actions/Expressions are defined.
// You can manually enter these, or use CICK (recommended)
// See the Extension FAQ in this SDK for more info and where to download it
//
// ============================================================================

// Common Include
#include	"common.h"

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
		IDMN_ACT_SETTEXT, M_ACT_SETTEXT,	ACT_SETTEXT,	0, 1, PARAM_EXPSTRING, IDS_SETTEXT_TITLE,
		};

// Definitions of parameters for each expression
short expressionsInfos[]=
		{
		IDMN_EXP_GETTEXT, M_EXP_GETTEXT, EXP_GETTEXT, EXPFLAG_STRING, 0,
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
short WINAPI DLLExport ActSetText(LPRDATA rdPtr, long param1, long param2)
{
	// Parameter
	LPCTSTR pParamStr = (LPCTSTR)param1;

	// Free old string
	if ( rdPtr->pText != NULL )
		free(rdPtr->pText);

	// Copy new string
	if ( pParamStr != NULL )
	{
		rdPtr->pText = (LPTSTR)malloc((_tcslen(pParamStr) + 1)*sizeof(TCHAR));
		if ( rdPtr->pText != NULL )
			_tcscpy(rdPtr->pText, pParamStr);
	}

	return 0;
}


// ============================================================================
//
// EXPRESSIONS ROUTINES
// 
// ============================================================================

// -----------------
// Get Text
// -----------------

long WINAPI DLLExport ExpGetText(LPRDATA rdPtr,long param1)
{
	if ( rdPtr->pText == NULL )
		return (long)_T("");

	// Setting the HOF_STRING flag lets MMF know that you return a string.
	rdPtr->rHo.hoFlags |= HOF_STRING;

	return (long)rdPtr->pText;
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
			ActSetText,
			0
			};

long (WINAPI * ExpressionJumps[])(LPRDATA rdPtr, long param) = 
			{     
			ExpGetText,
			0
		};