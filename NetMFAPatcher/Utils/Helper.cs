using mmfparser;
using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.mmfparser.mfaloaders;
using NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters;
using NetMFAPatcher.Utils;
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
        public static DataLoader LoadParameter(int code, ByteIO reader)
        {
            DataLoader item = null;
            if(code==13)
            {
                item = new Every(reader);
                
            }
            if (code == 2)
            {
                item = new Time(reader);

            }
            if(item!=null) item.Read();

            return item;



        }
        

    }
}
