//----------------------------------------------------------------------------------
//
// FLOAT TO STRING
//
//----------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
namespace RuntimeXNA.Expressions
{
	
	public class EXP_FLOATTOSTRING:CExp
	{
		public override void  evaluate(CRun rhPtr)
		{
			rhPtr.rh4CurToken++;
			double value = rhPtr.get_ExpressionDouble();
			
			rhPtr.rh4CurToken++;
			int nDigits = rhPtr.get_ExpressionInt();
			if (nDigits < 1)
			{
				nDigits = 1;
			}
			
			rhPtr.rh4CurToken++;
			int nDecimals = rhPtr.get_ExpressionInt();
			
			System.String temp = value.ToString();
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			
			int point = temp.IndexOf('.');
			
			// Regarde si vraiment des chiffres apres la virgule
			int cpt;
			if (point >= 0)
			{
				for (cpt = point + 1; cpt < temp.Length; cpt++)
				{
					if (temp[cpt] != '0')
					{
						break;
					}
				}
				if (cpt == temp.Length)
				{
					point = - 1;
				}
			}
			
			// Formattage
			int pos = 0;
			if (point >= 0)
			{
				// Le signe
				if (value < 0.0)
				{
					result.Append("-");
					pos++;
				}
				
				// La partie entiere
				while (pos < point)
				{
					result.Append(temp[pos]);
					pos++;
				}
				
				if (nDecimals > 0)
				{
					result.Append(".");
					pos++;
					
					// La partie decimale
					for (cpt = 0; cpt < nDecimals && cpt + pos < temp.Length; cpt++)
					{
						result.Append(temp[pos + cpt]);
					}
				}
				else if (nDecimals < 0)
				{
					result.Append(".");
					pos++;
					while (pos < temp.Length)
					{
						result.Append(temp[pos]);
						pos++;
					}
				}
			}
			else
			{
				while (pos < temp.Length && temp[pos] != '.')
				{
					result.Append(temp[pos]);
					pos++;
				}
				if (nDecimals > 0)
				{
					result.Append(".");
					for (cpt = 0; cpt < nDecimals; cpt++)
					{
						result.Append("0");
					}
				}
			}
            rhPtr.getCurrentResult().forceString(new System.String(result.ToString().ToCharArray()));
		}
	}
}