// -----------------------------------------------------------------------------
//
// SET FONT NAME
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Services;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETFONTNAME:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			System.String name = rhPtr.get_EventExpressionString((CParamExpression) evtParams[0]);
			
			CFontInfo info = CRun.getObjectFont(pHo);
			
			info.lfFaceName = name;
			
			CRun.setObjectFont(pHo, info, null);
		}
	}
}