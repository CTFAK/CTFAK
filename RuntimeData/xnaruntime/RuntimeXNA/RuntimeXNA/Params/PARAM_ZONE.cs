//----------------------------------------------------------------------------------
//
// PARAM_ZONE: zone a l'ecran
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
	public class PARAM_ZONE:CParam
	{
		public short x1;
		public short y1;
		public short x2;
		public short y2;
		
		public override void  load(CRunApp app)
		{
			x1 = app.file.readAShort();
			y1 = app.file.readAShort();
			x2 = app.file.readAShort();
			y2 = app.file.readAShort();
		}
	}
}