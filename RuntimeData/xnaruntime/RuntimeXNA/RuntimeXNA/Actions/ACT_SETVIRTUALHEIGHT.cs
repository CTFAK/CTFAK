// -----------------------------------------------------------------------------
//
// SET VIRTUAL HEIGHT
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETVIRTUALHEIGHT:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int newHeight = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			
			if (newHeight < rhPtr.rhFrame.leHeight)
				newHeight = rhPtr.rhFrame.leHeight;
			if (newHeight > 0x7FFFF000)
				newHeight = 0x7FFFF000;
			
			if (rhPtr.rhFrame.leVirtualRect.bottom != newHeight)
				rhPtr.rhFrame.leVirtualRect.bottom = rhPtr.rhLevelSy = newHeight;
		}
	}
}