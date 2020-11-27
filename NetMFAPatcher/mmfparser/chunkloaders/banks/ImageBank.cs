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
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.chunkloaders
{
    public class ImageBank : ChunkLoader
    {
        Dictionary<int, ImageItem> images = new Dictionary<int, ImageItem>();
        public ImageBank(ByteIO reader) : base(reader)
        {
        }

        public ImageBank(Chunk chunk) : base(chunk)
        {
        }
        public override void Print(bool ext)
        {

        }

        public override void Read()
        {
            reader = new ByteIO(chunk.chunk_data);
            var number_of_items = reader.ReadUInt32();
            if (!Program.DumpImages) return;
            Console.WriteLine($"Found {number_of_items} images");
            for (int i = 0; i < number_of_items; i++)
            {
                var item = new ImageItem(reader);
                //item.isCompressed = false;
                

                item.Read();
                item.handle -= 1;
                
                //images[item.handle] = item;
                


            }

        }
    }
    public class ImageItem : ChunkLoader
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
            if (Program.DumpImages)
            {
                Load();
                return;
            }
            else
            {
                if (isCompressed)
                {
                    reader.Skip(8);
                    size = (int)reader.ReadUInt32();
                    reader.Skip(size + 20);
                }
                else
                {
                    reader.Skip(4);
                    size = (int)reader.ReadUInt32();
                    reader.Seek(size + position);
                }
            }
            
        }
        public void Load()
        {


            reader.Seek(position);

            if (isCompressed)
            {
                image_data = Decompressor.DecompressAsReader(reader);
            }
            else
            {
                image_data = reader;
            }

            var start = image_data.Tell();
            if (!isCompressed)
            {

            }
            checksum = image_data.ReadInt32();


            references = image_data.ReadInt32();
            size = (int)image_data.ReadUInt32();
            if (!isCompressed)
            {
                image_data = new ByteIO(image_data.ReadBytes(size + 20));
            }
            width = image_data.ReadInt16();
            height = image_data.ReadInt16();
            graphic_mode = image_data.ReadByte();//Graphic mode is always 4 for SL
            flags = image_data.ReadByte();

            image_data.Skip(2);
            x_hotspot = image_data.ReadInt16();
            y_hotspot = image_data.ReadInt16();
            action_x = image_data.ReadInt16();
            action_y = image_data.ReadInt16();

            Logger.Log($"Size: {width}x{height}");
            for (int i = 0; i < 4; i++)
            {
                image_data.ReadByte();
            }

            //Save($"{Program.DumpPath}\\ImageBank\\" + handle.ToString() + ".png");
            Save("cum.png");


            return;








        }
        public void Save(string filename)
        {
            Bitmap bitmap = new Bitmap((int)width, (int)height);
            Color[,] array = new Color[(int)width, (int)height];
            int num4 = ImageHelper.getPadding((int)width, 2);
            int num5 = 0;
            using (ByteIO binaryReader = image_data)
            {
                int colorSize = 3;
                for (int i = 0; i < (int)height; i++)
                {
                    for (int j = 0; j < (int)width; j++)
                    {
                        byte[] colorData = null;
                        if (graphic_mode == 4)
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
                int num6 = size - num5;
                if (flags == 16)
                {
                    num4 = (num6 - (int)(width * height)) / (int)height;
                    for (int k = 0; k < (int)height; k++)
                    {
                        for (int l = 0; l < (int)width; l++)
                        {
                            byte Calpha = binaryReader.ReadByte();
                            Color color = array[l, k];
                            array[l, k] = Color.FromArgb(Calpha, color.R, color.G, color.B);
                        }
                        binaryReader.ReadBytes(num4);
                    }
                }
                for (int m = 0; m < (int)height; m++)
                {
                    for (int n = 0; n < (int)width; n++)
                    {
                        bitmap.SetPixel(n, m, array[n, m]);
                    }
                }
                

            }
            bitmap.Save(filename, ImageFormat.Png);
        }
        
            
   
        

        public override void Print(bool ext)
        {

        }
        public ImageItem(ByteIO reader) : base(reader)
        {
        }

        public ImageItem(Chunk chunk) : base(chunk)
        {
        }
    }

    public class TestPoint
    {
        public int size = 3;
        public (byte r, byte g, byte b) Read(byte[] data, int position)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;
            try
            {


                b = data[position];
                g = data[position + 1];
                r = data[position + 2];
            }
            catch
            {

                Console.WriteLine(position);
            }
            return (r, g, b);
        }


    }


}
