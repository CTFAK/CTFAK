using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.EXE.ChunkList;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders
{
    public class AppIcon : ChunkLoader
    {
        List<byte> _points;


        public AppIcon(ByteReader reader) : base(reader)
        {
        }

        public AppIcon(Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            
            Logger.Log("dumpingIcon");
            Reader.ReadBytes(Reader.ReadInt32() - 4);
            List<byte> colorIndexes = new List<byte>();
            for (int i = 0; i < 16 * 16; i++)
            {
                var r = Reader.ReadSByte();
                var g = Reader.ReadSByte();
                var b = Reader.ReadSByte();
                Reader.ReadByte();
                colorIndexes.Add((byte) r);
                colorIndexes.Add((byte) g);
                colorIndexes.Add((byte) b);
            }

            _points = new List<byte>();
            for (int y = 0; y < 16; y++)
            {
                var xList = new List<byte>();
                for (int x = 0; x < 16; x++)
                {
                    xList.Add(colorIndexes[Reader.ReadByte()]);
                }

                xList.AddRange(_points);
                _points = xList;                
            }

            for (int i = 0; i < 16*16/8; i++)
            {
                
            }
            using (var bmp = new Bitmap(16, 16, PixelFormat.Format32bppArgb))
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0,
                        bmp.Width,
                        bmp.Height),
                    ImageLockMode.WriteOnly,
                    bmp.PixelFormat);

                IntPtr pNative = bmpData.Scan0;
                Marshal.Copy(_points.ToArray(), 0, pNative, _points.Count);

                bmp.UnlockBits(bmpData);

                bmp.Save("icon.png");
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