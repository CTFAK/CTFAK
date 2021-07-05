// -----------------------------------------------------------------------------
//
// ACTIVATE GROUP
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Events;
namespace RuntimeXNA.Actions
{
	
	public class ACT_GRPACTIVATE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			PARAM_GROUPOINTER p = (PARAM_GROUPOINTER) evtParams[0];
			int evg = p.pointer;
			CEventGroup evgPtr = rhPtr.rhEvtProg.events[evg];
			CEvent evtPtr = evgPtr.evgEvents[0];
			
			PARAM_GROUP grpPtr = (PARAM_GROUP) evtPtr.evtParams[0];
			bool bFlag = (grpPtr.grpFlags & PARAM_GROUP.GRPFLAGS_GROUPINACTIVE) != 0;
			grpPtr.grpFlags &= ~ PARAM_GROUP.GRPFLAGS_GROUPINACTIVE;
			
			if (bFlag)
			{
				grpActivate(rhPtr, evg);
			}
		}
		internal virtual int grpActivate(CRun rhPtr, int evg)
		{
			CEventGroup evgPtr = rhPtr.rhEvtProg.events[evg];
			CEvent evtPtr = evgPtr.evgEvents[0];
			PARAM_GROUP grpPtr = (PARAM_GROUP) evtPtr.evtParams[0];
			int cpt;
			bool bQuit = false;
			
			if ((grpPtr.grpFlags & PARAM_GROUP.GRPFLAGS_PARENTINACTIVE) == 0)
			{
				evgPtr.evgFlags &= unchecked((ushort)~CEventGroup.EVGFLAGS_INACTIVE);
				
				for (evg++, bQuit = false, cpt = 1; ; )
				{
					evgPtr = rhPtr.rhEvtProg.events[evg];
					evtPtr = evgPtr.evgEvents[0];
					switch (evtPtr.evtCode)
					{
						
						case ((- 10 << 16) | 65535):  // CNDL_GROUP:
							grpPtr = (PARAM_GROUP) evtPtr.evtParams[0];
							if (cpt == 1)
							{
								grpPtr.grpFlags &= ~ PARAM_GROUP.GRPFLAGS_PARENTINACTIVE;
							}
							if ((grpPtr.grpFlags & PARAM_GROUP.GRPFLAGS_GROUPINACTIVE) == 0)
							{
								evg = grpActivate(rhPtr, evg);
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
								evgPtr.evgFlags &= unchecked((ushort)~ CEventGroup.EVGFLAGS_INACTIVE);
								bQuit = true;
								evg++;
							}
							break;
						
						case ((- 23 << 16) | 65535):  // CNDL_GROUPSTART:
							if (cpt == 1)
							{
								evgPtr.evgFlags &= unchecked((ushort)~ CEventGroup.EVGFLAGS_INACTIVE);
								evgPtr.evgFlags &= unchecked((ushort)~ CEventGroup.EVGFLAGS_ONCE);
							}
							break;
						
						default: 
							if (cpt == 1)
							{
								evgPtr.evgFlags &= unchecked((ushort)~ CEventGroup.EVGFLAGS_INACTIVE);
							}
							break;
						
					}
					if (bQuit)
						break;
					evg++;
				}
			}
			else
			{
				// Saute le groupe et les sous-groupes
				for (evg++, bQuit = false, cpt = 1; ; evg++)
				{
					evgPtr = rhPtr.rhEvtProg.events[evg];
					evtPtr = evgPtr.evgEvents[0];
					switch (evtPtr.evtCode)
					{
						
						case ((- 10 << 16) | 65535):  // CNDL_GROUP:
							cpt++;
							break;
						
						case ((- 11 << 16) | 65535):  // CNDL_ENDGROUP:
							cpt--;
							if (cpt == 0)
							{
								bQuit = true;
								evg++;
							}
							break;
						}
					if (bQuit)
						break;
				}
			}
			return evg;
		}
	}
}