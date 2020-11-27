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
        public static string AutoReadUnicode(ByteIO reader)
        {
            var len = reader.ReadInt16();
            reader.Skip(2);
            return reader.ReadWideString(len);
        }
        public static DataLoader LoadParameter(int code, ByteIO reader)
        {
            DataLoader item = null;
            if (code == 1)
            {
                item = new ParamObject(reader);
            }
            if (code == 2)
            {
                item = new Time(reader);
            }
            if (code==3|| code == 10|| code == 11 || code == 12 || code == 17 || code == 26 || code == 31 || code == 43 || code == 57 || code == 58 || code == 60 || code == 61)
            {
                item = new Short(reader);                
            }
            return item;

        }
        public static string GetHex(this byte[] data, int count=-1,int position=0)
        {
            var actualCount = count;
            if (actualCount == -1) actualCount = data.Length;
            string temp = "";
            for (int i = 0; i < actualCount; i++)
            {
                temp += data[i].ToString("X2");
                temp += " ";
            }
            return temp;
        }
        public static void PrintHex(this byte[] data)
        {
            var blockSize = 16;
            var blocks = data.Split<byte>(blockSize);
            foreach (var block in blocks)
            {
                string charAcc = "";
                foreach (var b in block)
                {
                    if (b < 128 && b > 32) charAcc += Convert.ToChar(b);
                    else charAcc += '.';
                }
                var b_len = block.Count();
                //var accLen=


            }
            

        }
        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }


    }
}
