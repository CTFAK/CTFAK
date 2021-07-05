// -----------------------------------------------------------------------------
//
// CENTER X DISPLAY
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_CDISPLAYX:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int x = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			rhPtr.setDisplay(x, 0, - 1, 1);
		}
	}
}