using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.utils;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Banks
{
    public class ImageBank : ChunkLoader
    {
        Dictionary<int, ImageItem> _images = new Dictionary<int, ImageItem>();

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
            Reader = new ByteIO(Chunk.ChunkData);

            var numberOfItems = Reader.ReadUInt32();
            Console.WriteLine(@"Found {numberOfItems} images");
            for (int i = 0; i < numberOfItems; i++)
            {
                var item = new ImageItem(Reader);
                item.Read();
                if (Program.DumpImages)
                    item.Save($"{Program.DumpPath}\\ImageBank\\" + item.Handle.ToString() + ".png");

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


        public bool IsCompressed = true;

        public override void Read()
        {
            Handle = Reader.ReadInt32();
            Position = (int) Reader.Tell();
            Load();
        }

        public void Load()
        {
            Reader.Seek(Position);
            ByteIO imageReader;
            if (IsCompressed)
            {
                imageReader = Decompressor.DecompressAsReader(Reader);
            }
            else
            {
                imageReader = Reader;
            }

            long start = imageReader.Tell();

            _checksum = imageReader.ReadInt32();
            _references = imageReader.ReadInt32();
            Size = (int) imageReader.ReadUInt32();
            if (!IsCompressed)
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
            Logger.Log($"{Handle.ToString(),4} Size: {_width,4}x{_height,4}, flags: {Flags}");
            byte[] imageData;
            if (Flags["LZX"])
            {
                throw new NotImplementedException();
                imageData = new byte[1];
            }
            else
            {
                imageData = imageReader.ReadBytes((int) (imageReader.Size() - imageReader.Tell()));
            }

            int bytesRead = 0;
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