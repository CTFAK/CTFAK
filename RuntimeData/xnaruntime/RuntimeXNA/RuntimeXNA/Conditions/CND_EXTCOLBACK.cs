// ------------------------------------------------------------------------------
// 
// COLLISION WITH BACKGROUND
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Events;
using RuntimeXNA.Sprites;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTCOLBACK:CCnd, IEvaObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			if (compute_NoRepeat(hoPtr))
			// One shot
			{
				rhPtr.rhEvtProg.evt_AddCurrentObject(hoPtr); //; Stocke l'objet courant
				return true;
			}
			
			// Si une action STOP dans le groupe, il faut la faire!!!
			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			CEventGroup pEvg = rhPtr.rhEvtProg.rhEventGroup;
			if ((pEvg.evgFlags & CEventGroup.EVGFLAGS_STOPINGROUP) == 0)
				return false;
			rhPtr.rhEvtProg.rh3DoStop = true;
			return true;
		}
		public override bool eva2(CRun rhPtr)
		{
			return evaObject(rhPtr, this);
		}
		public virtual bool evaObjectRoutine(CObject hoPtr)
		{
			if (hoPtr.hoAdRunHeader.colMask_TestObject_IXY(hoPtr, hoPtr.roc.rcImage, hoPtr.roc.rcAngle, hoPtr.roc.rcScaleX, hoPtr.roc.rcScaleY, hoPtr.hoX, hoPtr.hoY, 0, CColMask.CM_TEST_PLATFORM) != 0)
				return negaTRUE();
			return negaFALSE();
		}
	}
}