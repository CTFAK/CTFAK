// -----------------------------------------------------------------------------
//
// DISPLAY STRING
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Actions
{
	
	public class ACT_STRDISPLAYSTRING:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo != null)
			{
				CText pText = (CText) pHo;
				if (pText.txtChange(- 1))
				{
					pHo.roc.rcChanged = true;
					pHo.display();
				}
			}
		}
	}
}