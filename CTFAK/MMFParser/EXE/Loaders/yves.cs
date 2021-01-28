using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class AppIcon : ChunkLoader
    {
        byte[] _points;


        public AppIcon(ByteReader reader) : base(reader)
        {
        }

        public AppIcon(Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            return;
            Reader.ReadBytes(Reader.ReadInt32() - 4);
            List<byte> colorIndexes = new List<byte>();
            for (int i = 0; i < 16 * 16; i++)
            {
                var b = Reader.ReadSByte();
                var g = Reader.ReadSByte();
                var r = Reader.ReadSByte();
                Reader.ReadByte();
                colorIndexes.Add((byte) r);
                colorIndexes.Add((byte) g);
                colorIndexes.Add((byte) b);
            }

            var bitmap = new Bitmap(16, 16);
            for (int y = 0; y < 16; y++)
            {
                var xList = new List<byte>();
                for (int x = 0; x < 16; x++)
                {
                    var value = Reader.ReadByte();
                    bitmap.SetPixel(x, (16 - 1) - y,Color.Brown);
                }
            }
            bitmap.Save("penis.png");

            List<byte> alpha = new List<byte>();
            for (int i = 0; i < 16*16/8; i++)
            {
                List<bool> newAlphas = new List<bool>();
                var val = Reader.ReadByte();
                for (int j = 0; j < 8; j++)
                {
                    
                    newAlphas.Add(ByteFlag.GetFlag(val,i));
                }

                foreach (bool b in newAlphas)
                {
                    if (b) _points[i + 3] = 0;
                    else _points[i + 3] = 255;
                }
            }

            
            

            
        }


        public override void Print(bool ext)
        {
        }

        public override string[] GetReadableData()
        {
            return Array.Empty<string>();
        }
    }
}