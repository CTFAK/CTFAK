// -----------------------------------------------------------------------------
//
// DEACTIVATE GROUP
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Events;
namespace RuntimeXNA.Actions
{
	
	public class ACT_GRPDEACTIVATE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			PARAM_GROUPOINTER p = (PARAM_GROUPOINTER) evtParams[0];
			int evg = p.pointer;
			CEventGroup evgPtr = rhPtr.rhEvtProg.events[evg];
			CEvent evtPtr = evgPtr.evgEvents[0];
			
			PARAM_GROUP grpPtr = (PARAM_GROUP) evtPtr.evtParams[0];
			bool bFlag = (grpPtr.grpFlags & PARAM_GROUP.GRPFLAGS_GROUPINACTIVE) == 0;
			grpPtr.grpFlags |= PARAM_GROUP.GRPFLAGS_GROUPINACTIVE;
			
			if (bFlag == true && (grpPtr.grpFlags & PARAM_GROUP.GRPFLAGS_PARENTINACTIVE) == 0)
			{
				grpDeactivate(rhPtr, evg);
			}
		}
		internal virtual int grpDeactivate(CRun rhPtr, int evg)
		{
			CEventGroup evgPtr = rhPtr.rhEvtProg.events[evg];
			CEvent evtPtr = evgPtr.evgEvents[0];
			PARAM_GROUP grpPtr = (PARAM_GROUP) evtPtr.evtParams[0];
			
			evgPtr.evgFlags |= CEventGroup.EVGFLAGS_INACTIVE;
			
			int cpt;
			bool bQuit, bFlag;
			
			for (evg++, bQuit = false, cpt = 1; ; )
			{
				evgPtr = rhPtr.rhEvtProg.events[evg];
				evtPtr = evgPtr.evgEvents[0];
				switch (evtPtr.evtCode)
				{
					
					case ((- 10 << 16) | 65535):  // CNDL_GROUP:
						grpPtr = (PARAM_GROUP) evtPtr.evtParams[0];
						bFlag = (grpPtr.grpFlags & PARAM_GROUP.GRPFLAGS_PARENTINACTIVE) == 0;
						if (cpt == 1)
						{
							grpPtr.grpFlags |= PARAM_GROUP.GRPFLAGS_PARENTINACTIVE;
						}
						if (bFlag != false && (grpPtr.grpFlags & PARAM_GROUP.GRPFLAGS_GROUPINACTIVE) == 0)
						{
							evg = grpDeactivate(rhPtr, evg);
							continue;
						}
						else
						{
							cpt++;
						}
						break;
					
					case ((- 11 << 16) | 65535):  // CNDL_ENDGROUP:
						cpt--;
						if (cpt == 0)
						{
							evgPtr.evgFlags |= CEventGroup.EVGFLAGS_INACTIVE;
							bQuit = true;
							evg++;
						}
						break;
					
					default: 
						if (cpt == 1)
						{
							evgPtr.evgFlags |= CEventGroup.EVGFLAGS_INACTIVE;
						}
						break;
					
				}
				if (bQuit)
					break;
				
				evg++;
			}
			return evg;
		}
	}
}