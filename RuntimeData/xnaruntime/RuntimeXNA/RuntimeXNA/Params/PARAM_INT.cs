//----------------------------------------------------------------------------------
//
// PARAM_INT : un parametre sur un int
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
	public class PARAM_INT:CParam
	{
		public int value_Renamed;
        public int value2;
		
		public override void  load(CRunApp app)
		{
			value_Renamed = app.file.readAInt();
            value2 = 0;
		}
	}
}