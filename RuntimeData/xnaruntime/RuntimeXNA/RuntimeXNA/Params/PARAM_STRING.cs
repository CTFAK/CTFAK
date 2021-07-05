//----------------------------------------------------------------------------------
//
// PARAM_STRING : une chaine
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
	public class PARAM_STRING:CParam
	{
		public System.String pString;
		
		public override void  load(CRunApp app)
		{
			pString = app.file.readAString();
		}
	}
}