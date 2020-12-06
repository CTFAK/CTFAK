using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NetMFAPatcher.GUI;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;

using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Banks
{
    public class ImageBank : ChunkLoader
    {
        public Dictionary<int, ImageItem> Images = new Dictionary<int, ImageItem>();
        public uint NumberOfItems;

        public ImageBank(ByteIO reader) : base(reader)
        {
        }

        public ImageBank(Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Number of images: {NumberOfItems}"               
            };
        }

        public override void Read()
        {
            
            Reader.Seek(0);//Reset the reader to avoid bugs when dumping more than once
            
            NumberOfItems = Reader.ReadUInt32();
            
            Console.WriteLine($"Found {NumberOfItems} images");
            
            if (!Settings.DumpImages) return;
            for (int i = 0; i < NumberOfItems; i++)
            {
                if (MainForm.BreakImages)
                {
                    MainForm.BreakImages = false;
                    break;
                }
                var item = new ImageItem(Reader);
                item.Read();
                Images.Add(item.Handle,item);
                if (Settings.DumpImages)
                {

                    item.Save($"{Settings.ImagePath}\\" + item.Handle.ToString() + ".png");
                    Console.ReadKey();
                    Helper.OnImageSaved(i,(int) NumberOfItems);
                }

                if (Exe.LatestInst.GameData.ProductBuild >= 284)
                    item.Handle -= 1;

                //images[item.handle] = item;
            }
        }
    }

    public class ImageItem : ChunkLoader
    {
        public int Handle;
        int Position;
        int _checksum;
        int _references;
        int _width;
        int _height;
        int _graphicMode;
        int _xHotspot;
        int _yHotspot;
        int _actionX;
        int _actionY;

        BitDict Flags = new BitDict(new string[]
        {
            "RLE",
            "RLEW",
            "RLET",
            "LZX",
            "Alpha",
            "ACE",
            "Mac"
        });

        public int Size;

        //tranparent,add later
        byte[] _transparent;
        byte[] _colorArray;
        int _indexed;

        public byte[] rawImg;


        public bool Debug = false;
        public int Debug2 = 1;

        public override void Read()
        {
            Handle = Reader.ReadInt32();
            Position = (int) Reader.Tell();
            Load();
        }

        public override void Print(bool ext)
        {
            
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            Reader.Seek(Position);
            ByteIO imageReader;

            imageReader = Debug ? Reader : Decompressor.DecompressAsReader(Reader, out var a);

            long start = imageReader.Tell();

            _checksum = imageReader.ReadInt32();
            _references = imageReader.ReadInt32();
            Size = (int) imageReader.ReadUInt32();

            if (Debug)
            {
                imageReader = new ByteIO(imageReader.ReadBytes(Size + 20));
            }

            _width = imageReader.ReadInt16();
            _height = imageReader.ReadInt16();
            _graphicMode = imageReader.ReadByte(); //Graphic mode is always 4 for SL
            Flags.flag = imageReader.ReadByte();

            imageReader.Skip(2);
            _xHotspot = imageReader.ReadInt16();
            _yHotspot = imageReader.ReadInt16();
            _actionX = imageReader.ReadInt16();
            _actionY = imageReader.ReadInt16();
            _transparent = imageReader.ReadBytes(4);
            Logger.Log($"Loading image {Handle.ToString(),4} Size: {_width,4}x{_height,4}");
            byte[] imageData = new byte[1];
            if (Debug2 == 1)
            {
                var imgLen = imageReader.Size() - imageReader.Tell();
                var data = imageReader.ReadBytes((int) imgLen);
                imageReader.BaseStream.Position -= imgLen;
                File.WriteAllBytes("CumImage.bin", Ionic.Zlib.DeflateStream.CompressBuffer(data));
            }

            if (Debug2 == 2)
            {
                imageData = File.ReadAllBytes("CumImage.bin");
            }
            if (Flags["LZX"])
            {
                var DecompressedSize = imageReader.ReadUInt32();
                imageData = Decompressor.decompress_block(imageReader, (int) (imageReader.Size() - imageReader.Tell()),
                    (int) DecompressedSize);

            }
            else
            {
                imageData = imageReader.ReadBytes((int) (imageReader.Size() - imageReader.Tell()));
            }

            


        int bytesRead = 0;
            rawImg = imageData;
            if (Flags["RLE"] || Flags["RLEW"] || Flags["RLET"])
            {
            }

            
            else
            {

                switch (_graphicMode)
                {
                    case 4:
                    {
                        (_colorArray, bytesRead) = ImageHelper.ReadPoint(imageData, _width, _height);
                        break;
                    }
                    case 6:
                    {
                        (_colorArray, bytesRead) = ImageHelper.ReadFifteen(imageData, _width, _height);
                        break;
                    }
                    case 7:
                    {
                        (_colorArray, bytesRead) = ImageHelper.ReadSixteen(imageData, _width, _height);
                        break;
                    }
                }
            }

            int alphaSize = Size - bytesRead;
            if (Flags["Alpha"])
            {
                byte[,] alpha = ImageHelper.ReadAlpha(imageData, _width, _height, Size - alphaSize);
                int stride = _width * 4;
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        _colorArray[(y * stride) + (x * 4) + 3] = alpha[x, y];
                    }
                }
            }

            return;
        }

        public void Save(string filename)
        {
            using (var bmp = new Bitmap(_width, _height, PixelFormat.Format32bppArgb))
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0,
                        bmp.Width,
                        bmp.Height),
                    ImageLockMode.WriteOnly,
                    bmp.PixelFormat);

                IntPtr pNative = bmpData.Scan0;
                Marshal.Copy(_colorArray, 0, pNative, _colorArray.Length);

                bmp.UnlockBits(bmpData);

                bmp.Save(filename);
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt32(_checksum);
            writer.WriteInt32(_references);
            writer.WriteInt32(_colorArray.Length);
            writer.WriteInt16((short) _width);
            writer.WriteInt16((short) _height);
            writer.WriteInt8(4);
            if (Flags["Alpha"])
            {
                writer.WriteInt8(16);
            }
            else
            {
                writer.WriteInt8(0);
            }
            writer.Skip(2);
            writer.WriteInt16((short) _xHotspot);
            writer.WriteInt16((short) _yHotspot);
            writer.WriteInt16((short) _actionX);
            writer.WriteInt16((short) _actionY);
            writer.WriteBytes(_transparent);
            writer.Skip(1);
            
                writer.WriteBytes(rawImg);
                
           
            

            



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
        public int Size = 3;

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