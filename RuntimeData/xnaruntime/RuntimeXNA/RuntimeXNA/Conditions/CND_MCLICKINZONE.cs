// ------------------------------------------------------------------------------
// 
// CLICK IN ZONE
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_MCLICKINZONE:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			short key = (short) rhPtr.rhEvtProg.rhCurParam0;
			if (((PARAM_SHORT) evtParams[0]).value == key)
			{
				PARAM_ZONE p = (PARAM_ZONE) evtParams[1];
				if (rhPtr.rh2MouseX >= p.x1 && rhPtr.rh2MouseX < p.x2 && rhPtr.rh2MouseY >= p.y1 && rhPtr.rh2MouseY < p.y2)
				{
					return true;
				}
			}
			return false;
		}
		public override bool eva2(CRun rhPtr)
		{
			if (((PARAM_SHORT) evtParams[0]).value == rhPtr.rhEvtProg.rh2CurrentClick)
			{
				PARAM_ZONE p = (PARAM_ZONE) evtParams[1];
				if (rhPtr.rh2MouseX >= p.x1 && rhPtr.rh2MouseX < p.x2 && rhPtr.rh2MouseY >= p.y1 && rhPtr.rh2MouseY < p.y2)
				{
					return true;
				}
			}
			return false;
		}
	}
}