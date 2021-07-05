// ------------------------------------------------------------------------------
// 
// MOUSE ON OBJECT
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Events;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_MONOBJECT:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			bool flag = (evtFlags2 & CEvent.EVFLAG2_NOT) != 0;
			PARAM_OBJECT po = (PARAM_OBJECT) evtParams[0];
			return rhPtr.getMouseOnObjectsEDX(po.oiList, flag);
		}
	}
}