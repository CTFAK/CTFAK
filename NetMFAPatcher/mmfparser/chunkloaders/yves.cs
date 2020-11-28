using NetMFAPatcher.mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public class AppIcon : ChunkLoader
    {
        List<byte> _points;


        public AppIcon(ByteIO reader) : base(reader)
        {
        }

        public AppIcon(Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            return;
            Logger.Log("dumpingIcon");
            Reader.ReadBytes(Reader.ReadInt32() - 4);
            List<byte> colorIndexes = new List<byte>();
            for (int i = 0; i < 16 * 16; i++)
            {
                var b = Reader.ReadByte();
                var g = Reader.ReadByte();
                var r = Reader.ReadByte();
                Reader.ReadByte();
                colorIndexes.Add(r);
                colorIndexes.Add(g);
                colorIndexes.Add(b);
            }

            _points = new List<byte>();
            for (int y = 0; y < 16; y++)
            {
                var xList = new List<byte>();
                for (int x = 0; x < 16; x++)
                {
                    xList.Add(colorIndexes[Reader.ReadByte()]);
                }

                //x_list.AddRange(points);
                //points = x_list;
                xList.AddRange(_points);
                _points = xList;
            }

            File.WriteAllBytes("fatcock.raw", _points.ToArray());
        }


        public override void Print(bool ext)
        {
        }
    }
}