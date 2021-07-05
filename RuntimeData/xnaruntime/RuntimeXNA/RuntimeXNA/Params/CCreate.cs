//----------------------------------------------------------------------------------
//
// CCREATE: parametre position donnes creation
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Params
{	
	public abstract class CCreate:CPosition
	{
		public short cdpHFII; // FrameItemInstance number
		public short cdpOi; // OI of the object to create
		
		public CCreate()
		{
		}
		public abstract override void  load(CRunApp app);
	}
}