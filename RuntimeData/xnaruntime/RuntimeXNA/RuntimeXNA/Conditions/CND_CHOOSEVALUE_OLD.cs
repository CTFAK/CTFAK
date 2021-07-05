// ------------------------------------------------------------------------------
// 
// CND_CHOOSEVALUE_OLD
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.OI;
using RuntimeXNA.Params;
using RuntimeXNA.Expressions;
namespace RuntimeXNA.Conditions
{
	
	public class CND_CHOOSEVALUE_OLD:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			int cpt = 0;
			
			// Boucle d'exploration
			CObject pHo = rhPtr.rhEvtProg.evt_FirstObjectFromType(COI.OBJ_SPR); //!!! QUE FAIRE?
			while (pHo != null)
			{
				cpt++;
				
				int number;
				if (evtParams[0].code == 53)
				// pEvp->evpCode==PARAM_ALTVALUE_EXP
					number = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
				else
					number = ((PARAM_SHORT) evtParams[0]).value;
				CValue value2 = rhPtr.get_EventExpressionAny((CParamExpression) evtParams[1]);
				
				if (pHo.rov != null)
				{
					CValue value_Renamed = new CValue(pHo.rov.getValue(number));
					short comp = ((CParamExpression) evtParams[1]).comparaison;
					if (CRun.compareTo(value_Renamed, value2, comp) == false)
					{
						rhPtr.rhEvtProg.evt_DeleteCurrentObject();
						cpt--;
					}
				}
				pHo = rhPtr.rhEvtProg.evt_NextObjectFromType();
			} ;
			// Vrai / Faux?
			if (cpt != 0)
				return true;
			return false;
		}
	}
}