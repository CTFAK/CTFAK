// ------------------------------------------------------------------------------
// 
// COUNTER EQUALS
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Expressions;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_CCOUNTER:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
			int cpt = rhPtr.rhEvtProg.evtNSelectedObjects;
			CValue value1 = new CValue();
			while (pHo != null)
			{
				value1.forceValue(((CCounter) pHo).cpt_GetValue());
				CValue value2 = rhPtr.get_EventExpressionAny((CParamExpression) evtParams[0]);
				if (CRun.compareTo(value1, value2, ((CParamExpression) evtParams[0]).comparaison) == false)
				{
					cpt--;
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
				}
				pHo = rhPtr.rhEvtProg.evt_NextObject();
			} while (pHo != null)
				;
			return (cpt != 0);
		}
	}
}