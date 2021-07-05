//----------------------------------------------------------------------------------
//
// PARAM_EXPRESSION : une expression
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
	public class PARAM_EXPRESSION:CParamExpression
	{
		public override void  load(CRunApp app)
		{
			comparaison = app.file.readAShort();
			load(app.file);
		}
	}
}