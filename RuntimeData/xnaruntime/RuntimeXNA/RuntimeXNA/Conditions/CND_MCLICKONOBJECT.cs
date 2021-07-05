// ------------------------------------------------------------------------------
// 
// CLICK ON OBJECT
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Events;
namespace RuntimeXNA.Conditions
{
	
	public class CND_MCLICKONOBJECT:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			PARAM_SHORT p = (PARAM_SHORT) evtParams[0];
			if (rhPtr.rhEvtProg.rhCurParam0 != p.value)
				return false; // La touche
			
			short oi = (short) rhPtr.rhEvtProg.rhCurParam1; //; L'objet qui clique
			PARAM_OBJECT po = (PARAM_OBJECT) evtParams[1];
			if (oi == po.oi)
			//; L'oi sur lequel on clique
			{
				rhPtr.rhEvtProg.evt_AddCurrentObject(rhPtr.rhEvtProg.rh4_2ndObject);
				return true;
			}
			
			short oil = po.oiList;
			if (oil >= 0)
				return false; // Un Qualifier?
			CQualToOiList qoil = rhPtr.rhEvtProg.qualToOiList[oil & 0x7FFF];
			int qoi;
			for (qoi = 0; qoi < qoil.qoiList.Length; qoi += 2)
			{
				if (qoil.qoiList[qoi] == oi)
				{
					rhPtr.rhEvtProg.evt_AddCurrentQualifier(oil);
					rhPtr.rhEvtProg.evt_AddCurrentObject(rhPtr.rhEvtProg.rh4_2ndObject);
					return true;
				}
			}
			;
			return false;
		}
		public override bool eva2(CRun rhPtr)
		{
			PARAM_SHORT p = (PARAM_SHORT) evtParams[0];
			if (rhPtr.rhEvtProg.rh2CurrentClick != p.value)
				return false; // La touche
			
			PARAM_OBJECT po = (PARAM_OBJECT) evtParams[1];
			return rhPtr.getMouseOnObjectsEDX(po.oiList, false);
		}
	}
}