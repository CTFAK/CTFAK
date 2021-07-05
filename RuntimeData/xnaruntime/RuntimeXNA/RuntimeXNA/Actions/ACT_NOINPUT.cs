// -----------------------------------------------------------------------------
//
// DISABLE INPUT
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Actions
{
	
	public class ACT_NOINPUT:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			rhPtr.rh2InputMask[evtOi] = 0; // Plus d'entree
		}
	}
}