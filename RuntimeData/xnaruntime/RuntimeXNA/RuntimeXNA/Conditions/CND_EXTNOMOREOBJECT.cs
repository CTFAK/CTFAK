// ------------------------------------------------------------------------------
// 
// NO MORE OBJECTS
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Events;
namespace RuntimeXNA.Conditions
{
	
	public class CND_EXTNOMOREOBJECT:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject hoPtr)
		{
			// Correction bug jeux K&P
			if (hoPtr == null)
			{
				return eva2(rhPtr);
			}
			if (evtOi >= 0)
			{
				if (hoPtr.hoOi != evtOi)
					return false;
				return true;
			}
			return evaNoMoreObject(rhPtr, 1);
		}
		public override bool eva2(CRun rhPtr)
		{
			return evaNoMoreObject(rhPtr, 0);
		}
		internal virtual bool evaNoMoreObject(CRun rhPtr, int sub)
		{
			short oil = evtOiList;
			
			CObjInfo poil;
			if (oil >= 0)
			{
				// Un objet normal
				poil = rhPtr.rhOiList[oil];
				if (poil.oilNObjects == 0)
					return true;
				return false;
			}
			
			// Un qualifier
			if (oil == - 1)
				return false;
			CQualToOiList pqoi = rhPtr.rhEvtProg.qualToOiList[oil & 0x7FFF];
			int count = 0;
			int qoi;
			for (qoi = 0; qoi < pqoi.qoiList.Length; qoi += 2)
			{
				poil = rhPtr.rhOiList[pqoi.qoiList[qoi + 1]];
				count += poil.oilNObjects;
			}
			count -= sub; //; Moins un si appel lors de killobject qualifier!
			if (count == 0)
				return true;
			return false;
		}
	}
}