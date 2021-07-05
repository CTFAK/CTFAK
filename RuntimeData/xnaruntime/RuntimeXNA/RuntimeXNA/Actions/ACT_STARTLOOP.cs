// -----------------------------------------------------------------------------
//
// START LOOP
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Events;
namespace RuntimeXNA.Actions
{
	
	public class ACT_STARTLOOP:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			System.String name = rhPtr.get_EventExpressionString((CParamExpression) evtParams[0]);
			if (name.Length == 0)
				return ;
			int number = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
			
			CLoop pLoop;
			bool bInfinite = false;
			int index;
			for (index = 0; index < rhPtr.rh4FastLoops.size(); index++)
			{
				pLoop = (CLoop)rhPtr.rh4FastLoops.get(index);
                if (System.String.Compare(pLoop.name, name, StringComparison.OrdinalIgnoreCase) == 0)
					break;
			}
			if (index == rhPtr.rh4FastLoops.size())
			{
				CLoop loop = new CLoop();
				rhPtr.rh4FastLoops.add(loop);
				index = rhPtr.rh4FastLoops.size() - 1;
				loop.name = name;
				loop.flags = 0;
			}
			pLoop = (CLoop)rhPtr.rh4FastLoops.get(index);
			pLoop.flags &= ~ CLoop.FLFLAG_STOP;
			
			bInfinite = false;
			if (number < 0)
			{
				bInfinite = true;
				number = 10;
			}
			System.String save = rhPtr.rh4CurrentFastLoop;
			bool actionLoop = rhPtr.rhEvtProg.rh2ActionLoop; // Flag boucle
			int actionLoopCount = rhPtr.rhEvtProg.rh2ActionLoopCount; // Numero de boucle d'actions
			CEventGroup eventGroup = rhPtr.rhEvtProg.rhEventGroup;
			for (pLoop.index = 0; pLoop.index < number; pLoop.index++)
			{
				rhPtr.rh4CurrentFastLoop = pLoop.name;
				rhPtr.rhEvtProg.rh2ActionOn = false;
				rhPtr.rhEvtProg.handle_GlobalEvents(((- 16 << 16) | 65535)); // CNDL_ONLOOP;
				if ((pLoop.flags & CLoop.FLFLAG_STOP) != 0)
					break;
				if (bInfinite)
					number = pLoop.index + 10;
			}
			rhPtr.rhEvtProg.rhEventGroup = eventGroup;
			rhPtr.rhEvtProg.rh2ActionLoopCount = actionLoopCount; // Numero de boucle d'actions
			rhPtr.rhEvtProg.rh2ActionLoop = actionLoop; // Flag boucle
			rhPtr.rh4CurrentFastLoop = save;
			rhPtr.rhEvtProg.rh2ActionOn = true;
			
			rhPtr.rh4FastLoops.remove(index);
		}
	}
}