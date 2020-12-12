using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DotNetCTFDumper.GUI;
using DotNetCTFDumper.MMFParser;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Events.Parameters;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Objects;

namespace DotNetCTFDumper.Utils
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
            string temp = String.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                var item = bytes[i];
                if (i > 0)
                {
                    temp += " " + item.ToString(format);
                }
                else
                {
                    temp += item.ToString(format);
                }
            }

            if (log)
            {
                Console.WriteLine(temp);
            }

            return temp;
        }

        public static string AutoReadUnicode(this ByteReader reader)
        {
            var len = reader.ReadInt16();
            reader.Skip(2);
            return reader.ReadWideString(len);
        }

        public static void AutoWriteUnicode(this ByteWriter writer, string value)
        {
            writer.WriteInt16((short) value.Length);
            writer.Skip(1);
            writer.WriteInt8(0x80);
            writer.WriteUnicode(value,false);
        }
       
        public static DataLoader LoadParameter(int code, ByteReader reader)
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

            if (code == 3 || code == 10 || code == 11 || code == 12 || code == 17 || code == 26 || code == 31 ||
                code == 43 || code == 57 || code == 58 || code == 60 || code == 61)
            {
                item = new Short(reader);
            }

            return item;
        }

        public static string GetHex(this byte[] data, int count = -1, int position = 0)
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

                var bLen = block.Count();
                //var accLen=
            }
        }

        public static byte[] GetContents(this ByteWriter wrt)
        {
            var buff = new byte[wrt.BaseStream.Length];
            for (int i = 0; i < wrt.BaseStream.Length; i++)
            {
                buff.Append<byte>((byte) wrt.BaseStream.ReadByte());
            }

            return buff;
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
            for (var i = 0; i < (float) array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static List<Color> GetColors(this byte[] bytes)
        {
            List<Color> colors = new List<Color>();
            for (int i = 0; i < bytes.Length; i += 4)
            {
                var color = Color.FromArgb(bytes[i], bytes[i + 1], bytes[i + 2], bytes[i + 3]);
                colors.Add(color);
            }

            return colors;
        }

        public static void CheckPattern(object source, object pattern)
        {
            if (source.GetType() != pattern.GetType())
                throw new InvalidDataException("Data is not valid: types are different");
            if (source is string)
            {
                if ((string) source != (string) pattern)
                {
                    throw new InvalidDataException($"Data is not valid: {source} != {pattern}");
                }
            }
            else
            {
                if (source != pattern)
                {
                    throw new InvalidDataException($"Data is not valid: {source} != {pattern}");
                }
            }
        }

        

        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;

        public static string ToPrettySize(this int value, int decimalPlaces = 0)
        {
            return ((long) value).ToPrettySize(decimalPlaces);
        }

        public static string ToPrettySize(this long value, int decimalPlaces = 0)
        {
            var asTb = Math.Round((double) value / OneTb, decimalPlaces);
            var asGb = Math.Round((double) value / OneGb, decimalPlaces);
            var asMb = Math.Round((double) value / OneMb, decimalPlaces);
            var asKb = Math.Round((double) value / OneKb, decimalPlaces);
            string chosenValue = asTb > 1 ? string.Format("{0} TB", asTb)
                : asGb > 1 ? string.Format("{0} GB", asGb)
                : asMb > 1 ? string.Format("{0} MB", asMb)
                : asKb > 1 ? string.Format("{0} KB", asKb)
                : string.Format("{0} B", Math.Round((double) value, decimalPlaces));
            return chosenValue;
        }

        public static string ActualName(this ChunkList.Chunk chunk)
        {
            var constName = ((Constants.ChunkNames) chunk.Id).ToString();
            int tempId = 0;
            int.TryParse(constName, out tempId);
            if (tempId != chunk.Id) return constName;
            else return $"Unknown-{chunk.Id}";
        }

        public static ChunkNode GetChunkNode(ChunkList.Chunk chunk, string customName = "[DEFAULT-NAME]")
        {
            ChunkNode node = null;
            if (chunk.Loader != null)
            {
                node = new ChunkNode(chunk.Name, chunk.Loader);
            }
            else
            {
                node = new ChunkNode(chunk.Name, chunk);
            }

            if (customName != "[DEFAULT-NAME]")
            {
                node.Text = customName;
            }

            return node;
        }

        public static Animation GetClosestAnimation(int index, Dictionary<int, Animation> animDict, int count)
        {
            try
            {
                return animDict[index];
            }
            catch
            {
            }

            return null;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;


        public static T[] To1DArray<T>(T[,] input)
        {
            // Step 1: get total size of 2D array, and allocate 1D array.
            int size = input.Length;
            T[] result = new T[size];

            // Step 2: copy 2D array elements into a 1D array.
            int write = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                for (int z = 0; z <= input.GetUpperBound(1); z++)
                {
                    result[write++] = input[i, z];
                }
            }

            // Step 3: return the new array.
            return result;
        }
        
        
    }
    public static class StringExtensionMethods
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}