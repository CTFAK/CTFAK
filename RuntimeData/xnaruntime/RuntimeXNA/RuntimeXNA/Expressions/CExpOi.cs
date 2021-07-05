//----------------------------------------------------------------------------------
//
// TOKEN EXPRESSION RELIE A UN OBJECT
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions
{	
	public abstract class CExpOi:CExp
	{
		public short oi;
		public short oiList;
		
		public CExpOi()
		{
		}
		public abstract override void  evaluate(CRun rhPtr);
	}
}