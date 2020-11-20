using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher
{
    static class Helper
    {
        public static byte[] ToBytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
            

        }
        public static string GetString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);


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
