// -----------------------------------------------------------------------------
//
// SPRITE PASTE SPRITE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Sprites;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SPRPASTE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			// Un cran d'animation sans effet
			if (pHo.roa != null)
				pHo.roa.animIn(0);
			
			if (pHo.hoLayer == 0)
			{
				if (pHo.roc.rcSprite != null)
				{
					rhPtr.rhApp.spriteGen.activeSprite(pHo.roc.rcSprite, CSpriteGen.AS_REDRAW, null);
				}
			}
			
			// Layer0 ? Stocke dans une table
			rhPtr.activeToBackdrop(pHo, ((PARAM_SHORT) evtParams[0]).value, false);
		}
	}
}