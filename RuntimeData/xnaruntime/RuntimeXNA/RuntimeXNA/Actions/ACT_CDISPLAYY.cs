// -----------------------------------------------------------------------------
//
// CENTER DISPLAY Y
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_CDISPLAYY:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int y = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			rhPtr.setDisplay(0, y, - 1, 2);
		}
	}
}