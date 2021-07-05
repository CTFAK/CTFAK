// -----------------------------------------------------------------------------
//
// DISPLAY STRING
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_STRDISPLAY:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			PARAM_SHORT p = (PARAM_SHORT) evtParams[1];
			rhPtr.txtDoDisplay(this, p.value); // trouve le numero du texte        
		}
	}
}