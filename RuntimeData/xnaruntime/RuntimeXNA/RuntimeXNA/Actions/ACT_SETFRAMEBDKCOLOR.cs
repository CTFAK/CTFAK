// -----------------------------------------------------------------------------
//
// SET FRAME BACKGROUND COLOR
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Services;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETFRAMEBDKCOLOR:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int color;
			if (evtParams[0].code == 24)
			// PARAM_COLOUR
				color = ((PARAM_COLOUR) evtParams[0]).color;
			else
			{
				color = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
				color = CServices.swapRGB(color);
			}
			rhPtr.rhFrame.leBackground = color;
			
			// Redraw frame
			rhPtr.ohRedrawLevel(false);
		}
	}
}