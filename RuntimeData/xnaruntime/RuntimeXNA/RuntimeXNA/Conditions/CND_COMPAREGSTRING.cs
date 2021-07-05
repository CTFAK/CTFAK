//----------------------------------------------------------------------------------
//
// COMPARE TO GLOBAL STRING
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Expressions;
namespace RuntimeXNA.Conditions
{
	
	public class CND_COMPAREGSTRING:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			int num;
			if (evtParams[0].code == 52)
			// PARAM_VARGLOBAL_EXP 
				num = (rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]) - 1);
			// &15; YVES: enlev
			else
				num = ((PARAM_SHORT) evtParams[0]).value;
			
			System.String gString = rhPtr.rhApp.getGlobalStringAt(num);
			CValue gValue = new CValue(gString);
			CValue value_Renamed = rhPtr.get_EventExpressionAny((CParamExpression) evtParams[1]);
			short comp = ((CParamExpression) evtParams[1]).comparaison;
			return CRun.compareTo(gValue, value_Renamed, comp);
		}
	}
}