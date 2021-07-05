// -----------------------------------------------------------------------------
//
// BRANCH TO NODE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Movements;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTBRANCHNODE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			System.String pName = rhPtr.get_EventExpressionString((CParamExpression) evtParams[0]);
			
			if (pHo.roc.rcMovementType == CMoveDef.MVTYPE_TAPED)
			{
				CMovePath pPath = (CMovePath) pHo.rom.rmMovement;
				pPath.mtBranchNode(pName);
			}
		}
	}
}