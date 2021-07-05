/* Copyright (c) 1996-2013 Clickteam
 *
 * This source code is part of the Android exporter for Clickteam Multimedia Fusion 2.
 * 
 * Permission is hereby granted to any person obtaining a legal copy 
 * of Clickteam Multimedia Fusion 2 to use or modify this source code for 
 * debugging, optimizing, or customizing applications created with 
 * Clickteam Multimedia Fusion 2.  Any other use of this source code is prohibited.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */
//----------------------------------------------------------------------------------
//
// FLOAT TO STRING
//
//----------------------------------------------------------------------------------
package Expressions;


import java.text.DecimalFormat;
import java.util.Formatter;
import java.util.Locale;

import RunLoop.*;
import Runtime.Log;

public class EXP_FLOATTOSTRING extends CExp
{
	@Override
	public void evaluate(CRun rhPtr)
	{ 
		rhPtr.rh4CurToken++;
		double value=rhPtr.get_ExpressionDouble();

		rhPtr.rh4CurToken++;
		int nDigits=rhPtr.get_ExpressionInt();

		rhPtr.rh4CurToken++;
		int nDecimals=rhPtr.get_ExpressionInt();
		
		
    	StringBuilder s = new StringBuilder();
     	Formatter formatter = new Formatter(s, Locale.US);
		// Set the sign
		s.append((value >=0 ? "": "-"));
		
		// Handle all as positive values
		DecimalFormat df = new DecimalFormat("#");
        df.setMaximumFractionDigits(12);
        String szDf = df.format(Math.abs(value));
        int length = szDf.length();
		int pPos = szDf.indexOf(".");
		
		//FloatToString$(>Value<, >Number of digits<, >Number of digits after decimal point or -1<)

		if(value > -1.0f && value < 1.0f && nDecimals == 0)
			value = Math.round(value);

		value = Math.abs(value);
		
		if (nDecimals == 0)
		{
			s.append(Long.toString((long)value));			
		}
		else if(nDecimals > 0)
		{
			s.append(Long.toString((long)value));
			long frac = Math.round((value - (long)value)*Math.pow(10, nDecimals));
			s.append(".");
			String f = "%0"+nDecimals+"d";
			s.append(String.format(Locale.US, f, frac));
		}
		else if (nDecimals == -1)
		{
			String adouble = Double.toString(value).toLowerCase();
			String DecNotation = "";			
			String NotScientific = "";
			int ExpNotation=0;
			int ExpCnt=0;
			double new_double;

			if(adouble.indexOf("e") != -1)
			{
				DecNotation = adouble.substring(0, adouble.indexOf("e"));	

				ExpNotation += Integer.parseInt(adouble.substring(adouble.indexOf("e")+1));
				new_double = Double.valueOf(DecNotation);
				while(new_double > 1.0)
				{
					new_double /=10.0;
					ExpCnt++;
				}
			}
			else 
			{
				DecNotation = adouble;
				new_double = Double.valueOf(DecNotation);
				while(new_double > 1.0)
				{
					new_double /=10.0;
					ExpCnt++;
				}
			}
			
			if(ExpCnt > 0)
			{
				ExpNotation += (ExpCnt-1);
				ExpCnt = 1;
			}
			
			if(nDigits < ExpNotation+ExpCnt || value < 1.0)
			{
				if(nDigits >= 0)
				{
					double frac = (Math.round(new_double *Math.pow(10, (nDigits))))/Math.pow(10, (nDigits-ExpCnt));
					if(frac - (long)frac > 0)
						s.append(Double.toString(frac));
					else if(frac >0)
						s.append(Long.toString((long)frac));
				}
				if(ExpNotation != 0)
				{
					NotScientific = String.format(Locale.US, "e%c%03d", (ExpNotation >=0 ? '+':'-'),Math.abs(ExpNotation));
					s.append(NotScientific);
				}
			}
			else if (nDigits == ExpNotation+ExpCnt)
			{
				s.append(Long.toString((long)(Math.round(new_double *Math.pow(10, (nDigits)))/Math.pow(10, (nDigits-ExpCnt))*Math.pow(10, (nDigits-ExpCnt)))));				
			}
			else
			{
				s.append(Long.toString((long)value));
				if(nDigits - ExpNotation > 0)
				{
					long frac = Math.round((value - (long)value)*Math.pow(10, nDigits-ExpNotation-ExpCnt));
					if(frac > 0)
					{
						s.append(".");
						s.append(Long.toString(frac));
					}
				}
			}
		
		}
		else if(nDecimals < -1)
		{
			s.append(Long.toString(Math.round(value*Math.pow(10, nDecimals))));
		}
		
		if(nDecimals == 0 && s.indexOf(".") != -1)
		{
			for (int i = s.length()-1;  i >= 0; i--) {
	
			    if (s.charAt(i) == '0')
			        s.deleteCharAt(i);
			    else
			    	break;
			}
		}

		if (s.length() > 0 && s.charAt(s.length()-1) == '.')
			s.deleteCharAt(s.length()-1);

		rhPtr.rh4Results[rhPtr.rh4PosPile].forceString(s.toString());
	}  

}
