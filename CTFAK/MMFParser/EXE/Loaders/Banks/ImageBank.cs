using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DotNetCTFDumper.GUI;
using DotNetCTFDumper.Utils;
using Joveler.Compression.ZLib;
using static DotNetCTFDumper.MMFParser.EXE.ChunkList;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders.Banks
{
    public class ImageBank : ChunkLoader
    {
        public bool SaveImages = true;
        public Dictionary<int, ImageItem> Images = new Dictionary<int, ImageItem>();
        public uint NumberOfItems;
        public bool PreloadOnly=false;

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

        public void Read(bool load, bool save)
        {
            var cache = Settings.DumpImages;
            Settings.DumpImages = load;
            SaveImages = save;
            Read();
            Settings.DumpImages = cache;
        }

        public ImageItem FromHandle(int handle)
        {
            Images.TryGetValue(handle, out var img);
            if (img == null) return null;
            else return img;
        }
        

        public void LoadByHandle(int handle)
        {
            Images[handle].Load();
        }

        
        
        public event MainForm.SaveHandler OnImageSaved;
        
        


        public override void Read()
        {
            if (!Settings.DoMFA) Reader.Seek(0); //Reset the reader to avoid bugs when dumping more than once
            var tempImages = new Dictionary<int, ImageItem>();


            NumberOfItems = Reader.ReadUInt32();

            Logger.Log($"Found {NumberOfItems} images",true,ConsoleColor.Green);
            

            //if (!Settings.DumpImages) return;
            Logger.Log("Reading Images",true,ConsoleColor.Green);
            for (int i = 0; i < NumberOfItems; i++)
            {
                if (MainForm.BreakImages)
                {
                    
                    break;
                }

                var item = new ImageItem(Reader);
                
                item.Read(!PreloadOnly);
                tempImages.Add(item.Handle, item);

                if (SaveImages) item.Save($"{Settings.ImagePath}\\" + item.Handle.ToString() + ".png");
                OnImageSaved?.Invoke(i,(int) NumberOfItems);



                if (Settings.Build >= 284)
                    item.Handle -= 1;

                //images[item.handle] = item;
            }
            Logger.Log("Images success",true,ConsoleColor.Green);
            if (!MainForm.BreakImages) Images = tempImages;
            MainForm.BreakImages = false;

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

        public void Read(bool load)
        {
            Handle = Reader.ReadInt32() - 1;
            Position = (int) Reader.Tell();
            Logger.Log("ImageFound: "+Handle);
            if (load) Load();
            
            else Preload();

        }
        public override void Read()
        {
            Handle = Reader.ReadInt32() - 1;
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

        public void Preload()
        {
            _bitmap = null;
            Reader.Seek(Position);
            ByteReader imageReader;
            Console.WriteLine("Preloading Image");
            if (Settings.twofiveplus)
            {

                //Do 2.5+ decryption
                
            }
            // imageReader = Debug ? Reader : Decompressor.DecompressAsReader(Reader, out var a);
            
            imageReader = Debug ? Reader : Decompressor.DecompressAsReader(Reader, out var a);
            
            //Directory.CreateDirectory("DUMP\\DEBUG");
            //File.WriteAllBytes($"DUMP\\DEBUG\\Img-{Handle}.imgb",imageReader.ReadBytes((int) imageReader.Size()));
            
            
            long start = imageReader.Tell();

            _checksum = imageReader.ReadInt32();
            _references = imageReader.ReadInt32();
            Size = (int) imageReader.ReadUInt32();
            imageReader.Seek(start+Size);
            
        }
        

        public void Load()
        {
            _bitmap = null;
            Reader.Seek(Position);
            ByteReader imageReader;
            if (!Settings.twofiveplus)
                imageReader = Debug ? Reader : Decompressor.DecompressAsReader(Reader, out var a);
            else imageReader = Reader;           
            long start = imageReader.Tell();
            
            
            //return;
            if(Settings.twofiveplus) imageReader.Skip(4);
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
            Logger.Log($"Loading image {Handle.ToString(),4} Size: {_width,4}x{_height,4}");
            byte[] imageData;
            Flags["LZX"] = false;
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
                    default:
                    {
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
           
            try
            {
                Logger.Log("Trying to save image");
                Bitmap.Save(filename);
            }
            catch 
            {
                Logger.Log("RIP");
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
                    Logger.Log("Trying again");
                }
            } 
            
            
            
        }

        public Bitmap Bitmap
        {
            get
            {
                if (_colorArray==null) Load();
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
            byte[] compressedImg = null;
            Flags["LZX"] = true;
            if (Flags["LZX"])
            {
                compressedImg = Decompressor.compress_block(rawImg);
                chunk.WriteUInt32((uint) compressedImg.Length+4);
            }
            else
            {
                chunk?.WriteUInt32((uint) (rawImg?.Length??0));
            }

            chunk.WriteInt16((short) _width);
            chunk.WriteInt16((short) _height);
            chunk.WriteInt8((byte) _graphicMode);
            chunk.WriteInt8((byte) Flags.flag);
            chunk.WriteInt16(0);
            chunk.WriteInt16((short) XHotspot);
            chunk.WriteInt16((short) YHotspot);
            chunk.WriteInt16((short) ActionX);
            chunk.WriteInt16((short) ActionY);
            chunk.WriteBytes(_transparent);
            if (Flags["LZX"])
            {
                chunk.WriteInt32(rawImg.Length);
                chunk.WriteBytes(compressedImg);
            }

            else
            {
                chunk.WriteBytes(rawImg);
            }

            writer.WriteInt32(Handle + 1);
            writer.WriteWriter(chunk);
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