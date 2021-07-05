// -----------------------------------------------------------------------------
//
// SET FRAME WIDTH
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETFRAMEWIDTH:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int newWidth = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			
			// Set new width
			int nOldWidth = rhPtr.rhFrame.leWidth;
			rhPtr.rhFrame.leWidth = newWidth;
			
			// Set virtual width
			if (nOldWidth == rhPtr.rhFrame.leVirtualRect.right)
				rhPtr.rhFrame.leVirtualRect.right = rhPtr.rhLevelSx = newWidth;
			
			// Redraw frame
			rhPtr.ohRedrawLevel(true);
		}
	}
}