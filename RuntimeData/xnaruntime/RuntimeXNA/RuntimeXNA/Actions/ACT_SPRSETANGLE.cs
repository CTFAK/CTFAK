// -----------------------------------------------------------------------------
//
// SET ANGLE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Sprites;
using RuntimeXNA.Banks;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SPRSETANGLE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
			{
				return ;
			}
			
			// Recupere parametres
			int nAngle = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			bool bAntiA = false;
			if (rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]) != 0)
			{
				bAntiA = true;
			}
			
			nAngle %= 360;
			if (nAngle < 0)
			{
				nAngle += 360;
			}
			
			bool bOldAntiA = false;
			if ((pHo.ros.rsFlags & CRSpr.RSFLAG_ROTATE_ANTIA) != 0)
			{
				bOldAntiA = true;
			}
			if (pHo.roc.rcAngle != nAngle || bOldAntiA != bAntiA)
			{
				pHo.roc.rcAngle = nAngle;
				pHo.ros.rsFlags &= ~ CRSpr.RSFLAG_ROTATE_ANTIA;
				if (bAntiA)
				{
					pHo.ros.rsFlags |= CRSpr.RSFLAG_ROTATE_ANTIA;
				}
				pHo.roc.rcChanged = true;
				
				CImage ifo = pHo.hoAdRunHeader.rhApp.imageBank.getImageInfoEx(pHo.roc.rcImage, pHo.roc.rcAngle, pHo.roc.rcScaleX, pHo.roc.rcScaleY);
				pHo.hoImgWidth = ifo.width;
				pHo.hoImgHeight = ifo.height;
				pHo.hoImgXSpot = ifo.xSpot;
				pHo.hoImgYSpot = ifo.ySpot;
			}
		}
	}
}