using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetMFAPatcher
{
    static class Helper
    {
        public static byte[] ToBytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
            

        }
        public static int getPadding(int width, int pad=2)
        {
            int num = pad - width * 3 % pad;
            if(num==pad)
            {
                num = 0;
            }
            return (int)Math.Ceiling((double)((float)num/3f));
        }
        public static string CleanInput(string strIn)
        {
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }

            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
        public static string GetString(this byte[] bytes)
        {
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += Convert.ToChar(bytes[i]);

            }
            return str;


        }
        public static string Log(this byte[] bytes, bool log = true, string format = "")
        {
            string Temp = String.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                var item = bytes[i];
                if (i > 0)
                {
                    Temp += " " + item.ToString(format);

                }
                else
                {
                    Temp += item.ToString(format);
                }

            }
            if (log)
            {


                Console.WriteLine(Temp);
            }
            return Temp;


        }

    }
}
