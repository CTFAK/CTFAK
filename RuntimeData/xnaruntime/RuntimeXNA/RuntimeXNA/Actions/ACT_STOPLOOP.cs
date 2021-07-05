// -----------------------------------------------------------------------------
//
// STOP LOOP
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_STOPLOOP:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			System.String name = rhPtr.get_EventExpressionString((CParamExpression) evtParams[0]);
			if (name.Length == 0)
				return ;
			
			CLoop pLoop;
			int n;
			for (n = 0; n < rhPtr.rh4FastLoops.size(); n++)
			{
				pLoop = (CLoop)rhPtr.rh4FastLoops.get(n);
				if (System.String.Compare(pLoop.name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					pLoop.flags |= CLoop.FLFLAG_STOP;
				}
			}
		}
	}
}