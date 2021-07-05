// -----------------------------------------------------------------------------
//
// SET STRING
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_STRSETSTRING:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo != null)
			{
				System.String text = rhPtr.get_EventExpressionString((CParamExpression) evtParams[0]);
				CText pText = (CText) pHo;
				if (pText.rsTextBuffer == null || (pText.rsTextBuffer != null && String.CompareOrdinal(text, pText.rsTextBuffer) != 0))
				{
					pText.txtSetString(text);
					pText.txtChange(- 1);
					if ((pHo.ros.rsFlags & CRSpr.RSFLAG_HIDDEN) == 0)
					{
						pHo.roc.rcChanged = true;
						pHo.display();
					}
				}
			}
		}
	}
}