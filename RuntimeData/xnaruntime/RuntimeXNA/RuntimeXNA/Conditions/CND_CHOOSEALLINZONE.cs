// ------------------------------------------------------------------------------
// 
// PICK OBJECTS IN ZONE
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Conditions
{
	
	public class CND_CHOOSEALLINZONE:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return eva2(rhPtr);
		}
		public override bool eva2(CRun rhPtr)
		{
			PARAM_ZONE p = (PARAM_ZONE) evtParams[0];
			if (rhPtr.rhEvtProg.select_ZoneTypeObjects(p, (short) 0) != 0)
				return true;
			return false;
		}
	}
}