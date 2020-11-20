using NetMFAPatcher.mmfparser;
using NetMFAPatcher.utils;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.chunkloaders
{
    class DebugImageBank : ChunkLoader
    {
        Dictionary<int, DebugImageItem> images = new Dictionary<int, DebugImageItem>();
        public DebugImageBank(ByteIO reader) : base(reader)
        {
        }

        public DebugImageBank(ChunkList.Chunk chunk) : base(chunk)
        {
        }
        public override void Print()
        {

        }

        public override void Read()
        {
            reader = new ByteIO(chunk.chunk_data);
            var number_of_items = reader.ReadUInt32();
            Console.WriteLine($"Found {number_of_items} images");
            for (int i = 0; i < number_of_items; i++)
            {
                var item = new DebugImageItem(reader);
                item.Read();
                item.handle -= 1;
                
                //images[item.handle] = item;
                


            }

        }
    }
    class DebugImageItem : ChunkLoader
    {

        public int handle;
        int position;
        int checksum;
        int references;
        int width;
        int height;
        int graphic_mode;
        int x_hotspot;
        int y_hotspot;
        int action_x;
        int action_y;
        public int flags;
        public int size;
        //tranparent,add later
        int indexed;
        byte[] image;
        byte[] alpha;
        ByteIO image_data;

        public bool isCompressed = true;

        public override void Read()
        {
            handle = reader.ReadInt32();
            position = (int)reader.Tell();
            if (!Program.DumpImages) return;
            Save($"{Program.DumpPath}\\ImageBank\\{handle}.png");

        }
        
        public void Save(string filename)
        {
            Bitmap result;
            var image_data = Decompressor.DecompressAsReader(reader);
            using (ByteIO binaryReader = image_data)
            {
                int num = 0;
                byte b = 0;
                short num2;
                short num3;
                if (true)
                {
                    binaryReader.ReadInt32();
                    binaryReader.ReadInt32();
                    num = (int)binaryReader.ReadUInt32();
                    num2 = binaryReader.ReadInt16();
                    num3 = binaryReader.ReadInt16();
                    graphic_mode = binaryReader.ReadByte();
                    b = (byte)binaryReader.ReadSByte();
                    binaryReader.BaseStream.Position += 2;
                    binaryReader.ReadInt16();
                    binaryReader.ReadInt16();
                    binaryReader.ReadInt16();
                    binaryReader.ReadInt16();
                    binaryReader.ReadByte();
                    binaryReader.ReadByte();
                    binaryReader.ReadByte();
                    binaryReader.ReadByte();

                }
                var colorSize = 3;
                Bitmap bitmap = new Bitmap((int)num2, (int)num3);
                Color[,] array = new Color[(int)num2, (int)num3];
                int num4 = Helper.getPadding((int)num2, 2);
                int num5 = 0;
                for (int i = 0; i < (int)num3; i++)
                {
                    for (int j = 0; j < (int)num2; j++)
                    {
                        byte[] colorData=null;
                        if(graphic_mode==4)
                        {
                            colorSize = 3;
                            colorData = binaryReader.ReadBytes(colorSize);
                            array[j, i] = ImageHelper.ReadPoint(colorData, 0);
                        }
                        else
                        {
                            colorSize = 2;
                            colorData = binaryReader.ReadBytes(colorSize);
                            array[j, i] = ImageHelper.ReadSixteen(colorData, 0);
                        }
                        
                        
                        

                        num5 += 3;
                    }
                    binaryReader.ReadBytes(num4 * 3);
                    num5 += num4 * 3;
                }
                int num6 = num - num5;
                if (b == 16)
                {
                    num4 = (num6 - (int)(num2 * num3)) / (int)num3;
                    for (int k = 0; k < (int)num3; k++)
                    {
                        for (int l = 0; l < (int)num2; l++)
                        {
                            byte b5 = binaryReader.ReadByte();
                            Color color = array[l, k];
                            array[l, k] = Color.FromArgb((int)b5, (int)color.R, (int)color.G, (int)color.B);
                        }
                        binaryReader.ReadBytes(num4);
                    }
                }
                for (int m = 0; m < (int)num3; m++)
                {
                    for (int n = 0; n < (int)num2; n++)
                    {
                        bitmap.SetPixel(n, m, array[n, m]);
                    }
                }
                result = bitmap;
            }
            result.Save(filename);
        }
        


        
        
            
   
        

        public override void Print()
        {

        }
        public DebugImageItem(ByteIO reader) : base(reader)
        {
        }

        public DebugImageItem(ChunkList.Chunk chunk) : base(chunk)
        {
        }


    }
}
