// -----------------------------------------------------------------------------
//
// SET SCALE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SPRSETSCALE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			// Recupere parametres
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			float fScale = (float) rhPtr.get_EventExpressionDouble((CParamExpression) evtParams[0]);
			bool bResample = false;
			if (rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]) != 0)
				bResample = true;
			pHo.setScale(fScale, fScale, bResample);
		}
	}
}