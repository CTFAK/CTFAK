// ------------------------------------------------------------------------------
// 
// NO SPECIFIC SAMPLE PLAYING?
// 
// ------------------------------------------------------------------------------
package Conditions;

import Objects.CObject;
import Params.CParam;
import Params.CParamExpression;
import Params.PARAM_SAMPLE;
import RunLoop.CRun;

public class CND_NOSPSAMPLAYING extends CCnd
{
    public boolean eva1(CRun rhPtr, CObject hoPtr)
    {
		return eva2(rhPtr);
    }
    public boolean eva2(CRun rhPtr)
    {
		CParam p = evtParams[0];
		boolean bPlaying = false;
		int nSound = -1;
	    // PARAM_EXPSTRING?
		if (p.code == 45) {
		    String name = rhPtr.get_EventExpressionString((CParamExpression)p);
		    nSound = rhPtr.rhApp.soundBank.getSoundHandleFromName(name);
		}
		else {
		    nSound = ((PARAM_SAMPLE)p).sndHandle;
		}
		if ( nSound >= 0 )
			bPlaying = rhPtr.rhApp.soundPlayer.isSamplePlaying((short)nSound);
		if (!bPlaying)
		{
			return negaTRUE();
		}
		return negaFALSE();
    }
}
