//----------------------------------------------------------------------------------
//
// PLAYER NAME
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GETPLAYERNAME:CExpOi
	{
		public override void  evaluate(CRun rhPtr)
		{
            rhPtr.getCurrentResult().forceString(rhPtr.rhApp.playerNames[oi]);
		}
	}
}