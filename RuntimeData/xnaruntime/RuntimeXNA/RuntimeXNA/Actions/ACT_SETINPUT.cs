// -----------------------------------------------------------------------------
//
// SET INPUT
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Params;
using RuntimeXNA.Application;
namespace RuntimeXNA.Actions
{
	
	public class ACT_SETINPUT:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			int input = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[0]);
			if (input > CRunApp.CTRLTYPE_KEYBOARD)
				return ;
			if (input == CRunApp.CTRLTYPE_MOUSE)
				input = CRunApp.CTRLTYPE_KEYBOARD;
			int joueur = evtOi;
			if (joueur >= 4)
			// MAX_PLAYER
				return ;
			rhPtr.rhApp.pcCtrlType[joueur] = (short) input;
			
			/*JOYSTICK
			// Ajout Yves build 242: initialize joystick if necessary
			if ( input >= CTRLTYPE_JOY1 && input <= CTRLTYPE_JOY4 )
			InitJoystick(joueur, input - CTRLTYPE_JOY1);*/
		}
	}
}