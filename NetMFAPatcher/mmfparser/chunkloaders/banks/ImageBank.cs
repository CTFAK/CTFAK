using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using DotNetCTFDumper.GUI;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.Data.ChunkList;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders.Banks
{
    public class ImageBank : ChunkLoader
    {
        public bool SaveImages=true;
        public Dictionary<int, ImageItem> Images = new Dictionary<int, ImageItem>();
        public uint NumberOfItems;

        public ImageBank(ByteReader reader) : base(reader)
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
        public void Read(bool load,bool save)
        {
            var cache = Settings.DumpImages;
            Settings.DumpImages = load;
            SaveImages = save;
            Read();
            Settings.DumpImages = cache;

        }
        public override void Read()
        {
            if (!Settings.DoMFA)Reader.Seek(0);//Reset the reader to avoid bugs when dumping more than once
            Images = new Dictionary<int, ImageItem>();
            

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
                Images.Add(item.Handle, item);
                
                    if(SaveImages)item.Save($"{Settings.ImagePath}\\" + item.Handle.ToString() + ".png");
 
                    Helper.OnImageSaved(i, (int) NumberOfItems);
                

                if (Exe.Instance.GameData.ProductBuild >= 284)
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
        public int XHotspot;
        public int YHotspot;
        public int ActionX;
        public int ActionY;

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
        public byte[] rawAlpha;


        public bool Debug = false;
        public int Debug2 = 1;
        private Bitmap _bitmap;

        public override void Read()
        {
            Handle = Reader.ReadInt32()-1;
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
            _bitmap = null;
            Reader.Seek(Position);
            ByteReader imageReader;

            imageReader = Debug ? Reader : Decompressor.DecompressAsReader(Reader, out var a);

            long start = imageReader.Tell();

            _checksum = imageReader.ReadInt32();
            _references = imageReader.ReadInt32();
            Size = (int) imageReader.ReadUInt32();

            if (Debug)
            {
                imageReader = new ByteReader(imageReader.ReadBytes(Size + 20));
            }

            _width = imageReader.ReadInt16();
            _height = imageReader.ReadInt16();
            _graphicMode = imageReader.ReadByte(); //Graphic mode is always 4 for SL
            Flags.flag = imageReader.ReadByte();

            imageReader.Skip(2);
            XHotspot = imageReader.ReadInt16();
            YHotspot = imageReader.ReadInt16();
            ActionX = imageReader.ReadInt16();
            ActionY = imageReader.ReadInt16();
            _transparent = imageReader.ReadBytes(4);
            //Logger.Log($"Loading image {Handle.ToString(),4} Size: {_width,4}x{_height,4}");
            byte[] imageData;
            if (Flags["LZX"])
            {
                uint decompressedSize = imageReader.ReadUInt32();
                imageData = Decompressor.decompress_block(imageReader, (int) (imageReader.Size() - imageReader.Tell()),
                    (int) decompressedSize);
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
                rawAlpha = Helper.To1DArray(alpha);
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

        public Bitmap Bitmap
        {
            get
            {
                if (_bitmap == null)
                {
                    _bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);

                    BitmapData bmpData = _bitmap.LockBits(new Rectangle(0, 0,
                            _bitmap.Width,
                            _bitmap.Height),
                        ImageLockMode.WriteOnly,
                        _bitmap.PixelFormat);

                    IntPtr pNative = bmpData.Scan0;
                    Marshal.Copy(_colorArray, 0, pNative, _colorArray.Length);

                    _bitmap.UnlockBits(bmpData);

                }

                return _bitmap;
            }


        }
        

        public void Write(ByteWriter writer)
        {
            ByteWriter chunk = new ByteWriter(new MemoryStream());
            chunk.WriteInt32(_checksum);
            chunk.WriteInt32(_references);
            chunk.WriteInt32(_colorArray.Length);
            chunk.WriteInt16((short) _width);
            chunk.WriteInt16((short) _height);
            chunk.WriteInt8((byte) _graphicMode);
            if (Flags["Alpha"])
            {
                chunk.WriteInt8(16);
            }
            else
            {
                chunk.WriteInt8(0);
            }

            chunk.Skip(2);
            chunk.WriteInt16((short) XHotspot);
            chunk.WriteInt16((short) YHotspot);
            chunk.WriteInt16((short) ActionX);
            chunk.WriteInt16((short) ActionY);
            chunk.WriteBytes(_transparent);

            chunk.WriteBytes(_colorArray);
            
            if(Flags["Alpha"])chunk.WriteBytes(rawAlpha);
            writer.WriteInt32(Handle);
            
            
            writer.WriteWriter(chunk);
            //MemoryStream ms = (MemoryStream) chunk.BaseStream;
            //writer.WriteBytes(ms.GetBuffer());
        }


        public ImageItem(ByteReader reader) : base(reader)
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