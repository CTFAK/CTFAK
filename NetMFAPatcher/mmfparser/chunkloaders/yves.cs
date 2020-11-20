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

namespace NetMFAPatcher.chunkloaders
{
    public class AppIcon:ChunkLoader
    {
        List<byte> points;


        public AppIcon(ByteIO reader) : base(reader)
        {
        }

        public AppIcon(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Logger.Log("dumpingIcon");
            reader.ReadBytes(reader.ReadInt32() - 4);
            List<byte> color_indexes = new List<byte>();
            for (int i = 0; i < 16*16; i++)
            {
                
                var b = reader.ReadByte();
                var g = reader.ReadByte();
                var r = reader.ReadByte();
                reader.ReadByte();
                color_indexes.Add(r);
                color_indexes.Add(g);
                color_indexes.Add(b);
            }
            points = new List<byte>();
            for (int y = 0; y < 16; y++)
            {
                var x_list = new List<byte>();
                for (int x = 0; x < 16; x++)
                {
                    x_list.Add(color_indexes[reader.ReadByte()]);
                }
                //x_list.AddRange(points);
                //points = x_list;
                x_list.AddRange(points);
                points = x_list;

                
               
            }
            File.WriteAllBytes("fatcock.raw", points.ToArray());
            




        }


        public override void Print()
        {
            
        }
    }
}
