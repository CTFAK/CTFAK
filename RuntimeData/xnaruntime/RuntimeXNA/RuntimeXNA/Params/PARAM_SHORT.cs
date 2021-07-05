//----------------------------------------------------------------------------------
//
// PARAM_SHORT : un parametre sur un short
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
	public class PARAM_SHORT:CParam
	{
		public short value;
		
		public override void  load(CRunApp app)
		{
			value = app.file.readAShort();
		}
	}
}