// -----------------------------------------------------------------------------
//
// MOVE TO LAYER
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Sprites;
using RuntimeXNA.Frame;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTMOVETOLAYER:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject hoPtr = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (hoPtr == null)
				return ;
			
			if (hoPtr.ros != null)
			{
				int nLayer = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
				
				if (nLayer > 0 && nLayer <= rhPtr.rhFrame.nLayers)
				{
					nLayer -= 1;
					
					// Set new layer
					hoPtr.hoLayer = (short) nLayer;
					
					// Show / hide sprite and update z-order index
					if (hoPtr.ros != null)
					{
						CSprite pSpr = hoPtr.roc.rcSprite;
						if (pSpr != null)
						{
							rhPtr.rhApp.spriteGen.setSpriteLayer(pSpr, nLayer);
							
							// Increments the maximum z-order value and use it
							CLayer pLayer = rhPtr.rhFrame.layers[nLayer];
							pLayer.nZOrderMax++;
							pSpr.sprZOrder = pLayer.nZOrderMax;
							
							// Update z-order
							// Update the zorder value in the runtime structure (not mandatory, done before DeleteSprite)
							hoPtr.ros.rsZOrder = pSpr.sprZOrder;
							
							// Hide object if new layer is hidden
							if ((pLayer.dwOptions & (CLayer.FLOPT_TOHIDE | CLayer.FLOPT_VISIBLE)) != CLayer.FLOPT_VISIBLE)
							{
								rhPtr.rhApp.spriteGen.activeSprite(pSpr, CSpriteGen.AS_REDRAW, null);
								hoPtr.ros.obHide();
							}
							else
							{
								// Show object if new layer is visible
								if ((hoPtr.ros.rsFlags & CRSpr.RSFLAG_VISIBLE) != 0 && (hoPtr.ros.rsFlags & CRSpr.RSFLAG_HIDDEN) != 0 && (pLayer.dwOptions & (CLayer.FLOPT_TOHIDE | CLayer.FLOPT_VISIBLE)) == CLayer.FLOPT_VISIBLE)
								{
									hoPtr.ros.obShow();
								}
							}
						}
					}
				}
			}
		}
	}
}