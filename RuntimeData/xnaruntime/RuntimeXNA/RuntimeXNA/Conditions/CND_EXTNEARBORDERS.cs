// ------------------------------------------------------------------------------
// 
// NEAR BORDERS?
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTNEARBORDERS:CCnd, IEvaExpObject
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			return evaExpObject(rhPtr, this);
		}
		public override bool eva2(CRun rhPtr)
		{
			return evaExpObject(rhPtr, this);
		}
		public virtual bool evaExpRoutine(CObject hoPtr, int bord, short comp)
		{
			int xw = hoPtr.hoAdRunHeader.rhWindowX + bord; // Compare en X
			int x = hoPtr.hoX - hoPtr.hoImgXSpot;
			if (x <= xw)
				return negaTRUE();
			
			xw = hoPtr.hoAdRunHeader.rhWindowX + hoPtr.hoAdRunHeader.rh3WindowSx - bord;
			x += hoPtr.hoImgWidth;
			if (x >= xw)
				return negaTRUE();
			
			int yw = hoPtr.hoAdRunHeader.rhWindowY + bord; // Compare en Y
			int y = hoPtr.hoY - hoPtr.hoImgYSpot;
			if (y <= yw)
				return negaTRUE();
			
			yw = hoPtr.hoAdRunHeader.rhWindowY + hoPtr.hoAdRunHeader.rh3WindowSy - bord;
			y += hoPtr.hoImgHeight;
			if (y >= yw)
				return negaTRUE();
			
			return negaFALSE();
		}
	}
}