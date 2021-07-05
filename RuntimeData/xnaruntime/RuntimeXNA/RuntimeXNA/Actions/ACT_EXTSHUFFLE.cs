// -----------------------------------------------------------------------------
//
// SHUFFLE
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSHUFFLE:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			rhPtr.rhEvtProg.rh2ShuffleBuffer.add(pHo);
		}
	}
}