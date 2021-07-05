// -----------------------------------------------------------------------------
//
// SET FRAME HEIGHT
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETFRAMEHEIGHT:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int newHeight = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			
			// Set new width
			int nOldHeight = rhPtr.rhFrame.leHeight;
			rhPtr.rhFrame.leHeight = newHeight;
			
			// Set virtual width
			if (nOldHeight == rhPtr.rhFrame.leVirtualRect.bottom)
				rhPtr.rhFrame.leVirtualRect.bottom = rhPtr.rhLevelSy = newHeight;
			
			// Redraw frame
			rhPtr.ohRedrawLevel(true);
		}
	}
}