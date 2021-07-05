// -----------------------------------------------------------------------------
//
// SET DIRECTIONS
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_EXTSETDIRECTIONS:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
				return ;
			
			int dirs = ((PARAM_INT) evtParams[0]).value_Renamed;
			pHo.rom.rmMovement.set8Dirs(dirs);
		}
	}
}