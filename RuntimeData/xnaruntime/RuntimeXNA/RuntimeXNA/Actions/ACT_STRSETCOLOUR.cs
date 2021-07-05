// -----------------------------------------------------------------------------
//
// SET STRING COLOR
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;
namespace RuntimeXNA.Actions
{
	
	public class ACT_STRSETCOLOUR:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo != null)
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
				CText pText = (CText) pHo;
				pText.rsTextColor = color;
				pHo.roc.rcChanged = true;
				pHo.display();
			}
		}
	}
}