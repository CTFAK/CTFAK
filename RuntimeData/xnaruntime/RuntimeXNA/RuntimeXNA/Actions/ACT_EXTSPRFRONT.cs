// -----------------------------------------------------------------------------
//
// SET SPRITE FRONT
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSPRFRONT:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			if (pHo.roc.rcSprite != null)
			{
				rhPtr.rhApp.spriteGen.moveSpriteToFront(pHo.roc.rcSprite);
			}
		}
	}
}