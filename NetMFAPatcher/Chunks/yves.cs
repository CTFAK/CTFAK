using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.Chunks
{
    public class AppIcon
    {
        ByteIO reader;
        List<byte> points;


        public AppIcon(ByteIO reader)
        {
            this.reader = reader;
        }
        public void Read()
        {
            reader.ReadBytes(reader.ReadInt32() - 4);
            var color_indexes = new byte[16 * 16 * 3];
            for (int i = 0; i < 16*16; i++)
            {
                
                var b = reader.ReadByte();
                var g = reader.ReadByte();
                var r = reader.ReadByte();
                reader.ReadSByte();
                color_indexes.Append<byte>(r);
                color_indexes.Append<byte>(g);
                color_indexes.Append<byte>(b);
            }
            points = new List<byte>();
            for (int y = 0; y < 16; y++)
            {
                var x_list = new List<byte>();
                for (int x = 0; x < 16; x++)
                {
                    x_list.Add(color_indexes[reader.ReadByte()]);
                }
                //var points = x_list.// this.points;
                    //how to add lists?
            }

            Dump();
            
            

        }
        public void Dump()
        {
           

        }
    }
}
