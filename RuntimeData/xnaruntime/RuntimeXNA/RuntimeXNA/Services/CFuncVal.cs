//----------------------------------------------------------------------------------
//
// CFUNCVAL petite classe pour parser une chaine
//
//----------------------------------------------------------------------------------
using System;
namespace RuntimeXNA.Services
{
	
	public class CFuncVal
	{
		public int intValue;
		public double doubleValue;

		public virtual int parse(System.String s)
		{
			// Un nombre hexadecimal?
			try
			{
				System.String ss;
				if (s.Length >= 3)
				{
					if (s[0] == '0' && (s[1] == 'x' || s[2] == 'X'))
					{
						ss = s.Substring(2);
                        try
                        {
						    intValue = System.Convert.ToInt32(ss, 16);
                        }
                        catch(FormatException e)
                        {
                            e.GetType();
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            e.GetType();
                        }
						return 0;
					}
					if (s[0] == '0' && (s[1] == 'b' || s[2] == 'B'))
					{
						ss = s.Substring(2);
                        try
                        {
    						intValue = System.Convert.ToInt32(ss, 2);
                        }
                        catch(FormatException e)
                        {
                            e.GetType();
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            e.GetType();
                        }
						return 0;
					}
				}
                double d = 0;
                if (s.Length > 0)
                {
                    try
                    {
                        d = System.Double.Parse(s);
                    }
                    catch (FormatException e)
                    {
                        e.GetType();
                    }
                    catch (OverflowException e)
                    {
                        e.GetType();
                    }
                }
                double frac = System.Math.Round((double)d);
				if (d - frac != 0)
				{
					doubleValue = d;
					return 1;
				}
				intValue = (int) d;
				return 0;
			}
			catch (System.FormatException e)
			{
                e.GetType();
			}
			intValue = 0;
			return 0;
		}
	}
}