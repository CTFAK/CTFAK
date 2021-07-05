// -----------------------------------------------------------------------------
//
// SET TEXT COLOR
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Services;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETTEXTCOLOR:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int rgb;
			if (evtParams[0].code == 22)
			// PARAM_EXPRESSION
			{
				rgb = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
				rgb = CServices.swapRGB(rgb);
			}
			else
				rgb = ((PARAM_COLOUR) evtParams[0]).color;
			
			CRun.setObjectTextColor(pHo, rgb);
		}
	}
}