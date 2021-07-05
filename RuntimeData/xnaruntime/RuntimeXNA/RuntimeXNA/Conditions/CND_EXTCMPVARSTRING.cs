// ------------------------------------------------------------------------------
// 
// COMPARE TO ALTERABLE STRING
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Values;
using RuntimeXNA.Expressions;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTCMPVARSTRING:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			// Boucle d'exploration
			CObject pHo = rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
			if (pHo == null)
				return false;
			
			int cpt = rhPtr.rhEvtProg.evtNSelectedObjects;
			CValue value1 = new CValue();
			CValue value2;
			CParamExpression p = (CParamExpression) evtParams[1];
			do 
			{
				int num;
				if (evtParams[0].code == 62)
				// PARAM_ALTSTRING_EXP)
					num = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
				else
					num = ((PARAM_SHORT) evtParams[0]).value;
				
				if (num >= 0 && num < CRVal.STRINGS_NUMBEROF_ALTERABLE && pHo.rov != null)
				{
					value1.forceString(pHo.rov.getString(num));
					value2 = rhPtr.get_EventExpressionAny(p);
					
					if (CRun.compareTo(value1, value2, p.comparaison) == false)
					{
						cpt--;
						rhPtr.rhEvtProg.evt_DeleteCurrentObject();
					}
				}
				else
				{
					cpt--;
					rhPtr.rhEvtProg.evt_DeleteCurrentObject();
				}
				pHo = rhPtr.rhEvtProg.evt_NextObject();
			}
			while (pHo != null);
			return (cpt != 0);
		}
	}
}