using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.Chunks
{
    class ImageBank:ChunkLoader
    {
        public override void Print()
        {
           
        }

        public override void Read()
        {
            reader = new ByteIO(chunk.chunk_data);
            var number_of_items = reader.ReadUInt32();
            Console.WriteLine("Total images: "+number_of_items);
            Console.WriteLine("OnImageBankStart: " + reader.Tell());
            for (int i = 0; i < number_of_items; i++)
            {
                var item = new ImageItem();
                item.reader = reader;
                item.Read();
                item.Save(i.ToString()+".raw");




            }

        }
    }
    class ImageItem:ChunkLoader
    {

        int handle;
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
        //tranparent,add later
        int indexed;
        byte[] image;
        byte[] alpha;
        private int currentSize;
        private byte[] currentImage;
        private byte[] curPoints;
        private int currentN;

        public void ReadRGB(byte[] data,int width,int heigth,TestPoint pointClass)
        {
            var n = 0;
            var i = 0;
            List<byte> points = new List<byte>();
            var pad = GetPadding(width, 3);
            for (int y = 0; y < heigth; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    points.AddRange(pointClass.Read(data, n));
                    n += 3;//pointClass.size;
                    i += 1;
                }
                n += 3;//(int)pad + pointClass.size;

            }
            curPoints = points.ToArray();
            currentN = n;
        }
        public void ReadAlpha(byte[] data, int width, int heigth, int position)
        {
            
            var n = 0;
            var i = 0;
            List<byte> points = new List<byte>();
            var pad = GetPadding(width, 1, 4);
            for (int y = 0; y < heigth; y++)
            {
                for (int x = 0; x < heigth; x++)
                {
                    points[i] = data[n + position];
                    n++;
                    i++;

                }
                n += (int)pad;
            }
            curPoints = points.ToArray();



        }

        public double GetPadding(int width,int classSize,int bytes=2)
        {
            var pad = bytes - ((width * classSize) % bytes);
            if (pad == bytes) pad = 0;
            var padding = Math.Ceiling((float)(pad / classSize));
            return padding;//Correct

        }


        
        

        public override void Read()
        {
            handle = reader.ReadInt32();
            position = (int)reader.Tell();
            Load();

          
        }
        public void Load()
        {


            reader.Seek(position);



            var decompressedImg = Decompressor.Decompress(reader);
            var image_data = new ByteIO(decompressedImg);
            var start = image_data.Tell();
            checksum = image_data.ReadInt32();
            references = image_data.ReadInt32();
            var size = image_data.ReadUInt32();
            width = image_data.ReadInt16();
            height = image_data.ReadInt16();
            graphic_mode = image_data.ReadByte();//Graphic mode is always 4 for SL
            var flags = image_data.ReadSByte();
            image_data.Skip(2);
            x_hotspot = image_data.ReadInt16();
            y_hotspot = image_data.ReadInt16();
            action_x = image_data.ReadInt16();
            action_y = image_data.ReadInt16();

            Logger.Log($"Size: {width}x{height}");
            for (int i = 0; i < 4; i++)
            {
                image_data.ReadByte();
                //transparence

            }
            int alpha_size=0;
            byte[] data;
            if (false)//lzx flag
            {
                var decompressed_size = image_data.ReadUInt32();
                image_data = Decompressor.decompress_asReader(image_data, (int)(image_data.Size() - image_data.Tell()), (int)decompressed_size);
            }
            else
            {
                //image_data = image_data.ReadBytes(-1)
                data = image_data.ReadBytes((int)reader.Size());
                var curPoint = new TestPoint();
                ReadRGB(data,width,height,curPoint);
                var image = curPoints;
                var image_size = currentN;
                this.image = image;
                alpha_size = (int)(size - image_size);
                
            }
            var pad = (alpha_size - width * height) / height;
            //ReadAlpha(data, width, height, (int)(size - alpha_size));
            //alpha = curPoints;
            


        }
        public void Save(string filename)
        {

            File.WriteAllBytes(filename,image);

        }

        public override void Print()
        {
            
        }
    }

    public class TestPoint
    {
        public int size = 3;
        public byte[] Read(byte[]data,int position)
        {
            byte r=0;
            byte g=0;
            byte b=0;
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
            return (new List<byte>() { r, g, b }).ToArray();
        }


    }


}
