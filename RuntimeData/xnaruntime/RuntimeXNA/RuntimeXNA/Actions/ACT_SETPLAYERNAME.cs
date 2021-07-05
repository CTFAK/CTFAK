// -----------------------------------------------------------------------------
//
// SET PLAYER NAME
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Application;
using RuntimeXNA.Params;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETPLAYERNAME:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int joueur = evtOi;
			if (joueur >= CRunApp.MAX_PLAYER)
				return ;
			System.String pString = rhPtr.get_EventExpressionString((CParamExpression) evtParams[0]);
			rhPtr.rhApp.playerNames[joueur] = pString;
		}
	}
}