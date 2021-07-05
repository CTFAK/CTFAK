// -----------------------------------------------------------------------------
//
// SET FONT SIZE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Services;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETFONTSIZE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int newSize = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			int bResize = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[1]);
			
			CFontInfo lf = CRun.getObjectFont(pHo);
			
			int oldSize = lf.lfHeight;
			lf.lfHeight = newSize;
			
			if (bResize == 0)
			{
				CRun.setObjectFont(pHo, lf, null);
			}
			else
			{
				CRect rc = new CRect();
				float coef = 1.0f;
				if (oldSize != 0)
				{
					//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
					coef = ((float) newSize) / ((float) oldSize);
				}
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				rc.right = (int) (pHo.hoImgWidth * coef);
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				rc.bottom = (int) (pHo.hoImgHeight * coef);
				rc.left = 0;
				rc.top = 0;
				CRun.setObjectFont(pHo, lf, rc);
			}
		}
	}
}