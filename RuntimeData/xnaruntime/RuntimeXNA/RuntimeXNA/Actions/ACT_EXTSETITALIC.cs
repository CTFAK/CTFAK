// -----------------------------------------------------------------------------
//
// SET ITALIC
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Services;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETITALIC:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int bFlag = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			
			CFontInfo info = CRun.getObjectFont(pHo);
			info.lfItalic = (byte) bFlag;
			CRun.setObjectFont(pHo, info, null);
		}
	}
}