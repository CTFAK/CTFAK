//----------------------------------------------------------------------------------
//
// PARAM_COLOUR : une valeur RGB
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
	public class PARAM_COLOUR:CParam
	{
		public int color;
		
		public override void  load(CRunApp app)
		{
			color = app.file.readAColor();
		}
	}
}