using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web.UI.Design.WebControls.WebParts;
using System.Windows.Forms;
using CTFAK.GUI.GUIComponents;
using CTFAK.MMFParser.Attributes;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Events.Parameters;
using CTFAK.MMFParser.EXE.Loaders.Objects;
using CTFAK.MMFParser.MFA.Loaders;
using ChunkList = CTFAK.MMFParser.EXE.ChunkList;
using Extension = CTFAK.MMFParser.EXE.Loaders.Events.Parameters.Extension;

namespace CTFAK.Utils
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

        public static byte[] GetBuffer(this ByteWriter writer)
        {
            var buf = ((MemoryStream) writer.BaseStream).GetBuffer();
            Array.Resize(ref buf,(int) writer.Size());
            return buf;
        }

        public static string GetCurrentTime()
        {
            var date = DateTime.Now;
            return $"[{date.Hour,2}:{date.Minute,2}:{date.Second,2}:{date.Millisecond,3}]";
        }
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static bool ContainsItem(this List<FrameItem> items, int handle)
        { 
            foreach (var item in items)
            {
                if (item.Handle == handle) return true;
            }
            return false;
        }

        public static string AutoReadUnicode(this ByteReader reader)
        {
            var len = reader.ReadInt16();
            short check = reader.ReadInt16();
            Debug.Assert(check == -32768);
            return reader.ReadWideString(len);
        }

        public static void AutoWriteUnicode(this ByteWriter writer, string value)
        {
            writer.WriteInt16((short) value.Length);
            writer.Skip(1);
            writer.WriteInt8(0x80);
            writer.WriteUnicode(value, false);
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
            if (code == 3 || code==4 ||code == 10 || code == 11 || code == 12 || code == 17 || code == 26 || code == 31 ||
                code == 43 || code == 57 || code == 58 || code == 60 || code == 61)
            {
                item = new Short(reader);
            }
            if (code == 5 || code == 25 || code == 29 || code == 34 || code == 48 || code == 56)
            {
                item = new Int(reader);
            }
            if (code == 6 || code == 7 || code == 35 || code == 36)
            {
                item = new Sample(reader);
            }
            if (code == 9 || code == 21)
            {
                item = new Create(reader);
            }
            if (code == 13)
            {
                item = new Every(reader);
            }
            if (code == 14 || code == 44)
            {
                item = new KeyParameter(reader);
            }
            if (code == 15||code == 22||code == 23||code == 27||code == 28||code == 45||code == 46||code==52||code==53||code==54||code == 59||code == 62)
            {
                item = new ExpressionParameter(reader);
            }
            if (code == 16)
            {
                item=new Position(reader);
            }
            if (code == 18)
            {
                item=new Shoot(reader);
            }
            if (code == 19)
            {
                item = new Zone(reader);
            }
            if (code == 24)
            {
                item = new Colour(reader);
            }
            

            if (code == 50)
            {
                item = new AlterableValue(reader);
            }

            if (code == 32)
            {
                item = new Click(reader);
            }

            if (code == 33)
            {
                item = new MMFParser.EXE.Loaders.Events.Parameters.Program(reader);
            }

            if (code == 55)
            {
                item = new Extension(reader);
            }

            if (code == 38)
            {
                item = new CTFAK.MMFParser.EXE.Loaders.Events.Parameters.Group(reader);
            }

            if (code == 39)
            {
                item = new GroupPointer(reader);
            }

            if (code == 49)
            {
                item = new GlobalValue(reader);
            }

            if (code == 41 || code == 64)
            {
                item = new StringParam(reader);
            }

            if (code == 47 || code == 51)
            {
                item = new TwoShorts(reader);
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
        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
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

        

        public static ChunkNode GetChunkNode(ChunkList.Chunk chunk)
        {
            
            List<TreeNode> newNodes = new List<TreeNode>();
            string newName = chunk.Name;
            
            if (chunk.Loader == null) return new ChunkNode(chunk.Name,chunk);
            if (chunk.Loader.GetType().GetCustomAttribute(typeof(SubChunkListAttribute)) != null)
            {
                var customAttribute = chunk.Loader.GetType()
                    .GetCustomAttribute(typeof(SubChunkListAttribute)) as SubChunkListAttribute;
                var chunkList = chunk.Loader.GetType().GetField(customAttribute.FieldName).GetValue(chunk.Loader) as ChunkList;
                foreach (var subChunk in chunkList.Chunks)
                {
                    var subNode = new ChunkNode(subChunk.Name,subChunk);
                    if (subChunk?.Loader?.HasAttribute<SubListAttribute>() ?? false)
                    {
                        subNode.Nodes.Add(GetChunkNode(subChunk));
                    }
                    newNodes.Add(subNode);
                }
            }
            if (chunk.Loader.GetType().GetCustomAttribute(typeof(SubListAttribute)) != null)
            {
                var customAttribute = chunk.Loader.GetType()
                    .GetCustomAttribute(typeof(SubListAttribute)) as SubListAttribute;
                var chunkList = chunk.Loader.GetType().GetField(customAttribute.FieldName).GetValue(chunk.Loader) as List<ChunkLoader>;
                if (chunkList != null)
                {
                    foreach (var subChunk in chunkList)
                    {
                        ChunkNode subnode;
                        if (subChunk.HasAttribute<SubListAttribute>()) subnode = GetChunkNode(subChunk.Chunk);
                        else subnode = new ChunkNode(subChunk.Chunk.Name,subChunk);
                        newNodes.Add(subnode);
                    } 
                }
                
            }
            if (chunk.Loader.GetType().GetCustomAttribute(typeof(CustomVisualNameAttribute)) != null)
            {
                var attr = chunk.Loader.GetType().GetCustomAttribute(typeof(CustomVisualNameAttribute)) as CustomVisualNameAttribute;
                string name = "";
                if (attr.IsProp) name = chunk.Loader.GetType().GetProperty(attr.CustomName).GetValue(chunk.Loader) as string;
                else name = chunk.Loader.GetType().GetField(attr.CustomName).GetValue(chunk.Loader) as string;
                
                newName = chunk.Name + " "+ name;
            }
            ChunkNode node = new ChunkNode(newName,chunk);
            node.Nodes.AddRange(newNodes.ToArray());
            return node;
        }

        public static bool HasAttribute<T>(this object obj) where T : Attribute
        {
            var attr = obj.GetType().GetCustomAttribute(typeof(T));
            return attr != null;
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