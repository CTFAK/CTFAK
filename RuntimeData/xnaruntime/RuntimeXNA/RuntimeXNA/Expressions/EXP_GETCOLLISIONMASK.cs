//----------------------------------------------------------------------------------
//
// MASQUE DE COLLISIONS
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Sprites;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_GETCOLLISIONMASK:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			int x, y;
			
			rhPtr.rh4CurToken++;
			x = rhPtr.get_ExpressionInt();
			rhPtr.rh4CurToken++;
			y = rhPtr.get_ExpressionInt();
			
			int result = 0;
			if (rhPtr.y_GetLadderAt_Absolute(- 1, x, y) != null)
				result = 2;
			else
			{
				if (rhPtr.rhFrame.bkdCol_TestPoint(x - rhPtr.rhWindowX, y - rhPtr.rhWindowY, CSpriteGen.LAYER_ALL, CColMask.CM_TEST_OBSTACLE))
					result = 1;
			}
            rhPtr.getCurrentResult().forceInt(result);
		}
	}
}