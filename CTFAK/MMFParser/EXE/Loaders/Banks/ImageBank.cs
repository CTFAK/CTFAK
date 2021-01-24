using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using CTFAK.GUI;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;


namespace CTFAK.MMFParser.EXE.Loaders.Banks
{
    public class ImageBank : ChunkLoader
    {
        public bool SaveImages = false;
        public Dictionary<int, ImageItem> Images = new Dictionary<int, ImageItem>();
        public uint NumberOfItems;
        public bool PreloadOnly = false;

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
            if (handle == -1) return Images[Images.Count - 1];
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

            NumberOfItems = (uint) Reader.ReadInt32();

            Logger.Log($"Found {NumberOfItems} images", true, ConsoleColor.Green);


            //if (!Settings.DumpImages) return;
            Logger.Log("Reading Images", true, ConsoleColor.Green);
            for (int i = 0; i < NumberOfItems; i++)
            {
                if (MainForm.BreakImages) break;
                {


                    var item = new ImageItem(Reader);

                    item.Read(!PreloadOnly);
                    tempImages.Add(item.Handle, item);
                    if (SaveImages) item.Save($"{Settings.ImagePath}\\" + item.Handle.ToString() + ".png");
                    OnImageSaved?.Invoke(i, (int) NumberOfItems);

                }
                
            }
            Logger.Log("Images success", true, ConsoleColor.Green);
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
        Color _transparent;
        byte[] _colorArray;

        public byte[] rawImg;
        public byte[] rawAlpha;


        public bool Debug = false;
        public int Debug2 = 1;
        private Bitmap _bitmap;

        public void Read(bool load)
        {
            Handle = Reader.ReadInt32();
            if (!Debug)
            {
                if (Program.CleanData.ProductVersion != Constants.Products.MMF15&&Settings.Build>=284) Handle -= 1;
            }
            
            Position = (int) Reader.Tell();
            if (load) Load();
            else Preload();

        }
        public override void Read()
        {
            Handle = Reader.ReadInt32();
            if (!Debug)
            {
                if (Exe.Instance.GameData.ProductVersion != Constants.Products.MMF15&&Settings.Build>=284) Handle -= 1;

            }
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
            if (!Settings.twofiveplus)
                imageReader = Debug ? Reader : Decompressor.DecompressAsReader(Reader, out var a);
            else imageReader = Reader;           
            long start = imageReader.Tell();


            if (Settings.twofiveplus)
            {
                var unk = imageReader.ReadBytes(4);
                if (unk.GetHex(4) != "FF FF FF FF ")
                {
                    Logger.Log(Reader.Tell().ToString());
                    Size = (int) BitConverter.ToUInt32(unk,0);
                    _references = imageReader.ReadInt32();
                    _checksum = (int) imageReader.ReadUInt32();
                }
                else
                {
                    Size = (int) imageReader.ReadInt32();
                    _references = imageReader.ReadInt32();
                    _checksum = (int) imageReader.ReadUInt32();
                }
                
                Logger.Log("Size: "+Size);
                Logger.Log("References: "+_references);
                Logger.Log("Checksum: "+_checksum);

            }
            else
            {
                _checksum = imageReader.ReadInt32();
                _references = imageReader.ReadInt32();
                Size = (int) imageReader.ReadUInt32();  
            }

            
            imageReader.Seek(start+ _checksum);//to prevent bugs
            
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
            _graphicMode = imageReader.ReadByte();
            
            Flags.flag = imageReader.ReadByte();

            imageReader.Skip(2);
            XHotspot = imageReader.ReadInt16();
            YHotspot = imageReader.ReadInt16();
            ActionX = imageReader.ReadInt16();
            ActionY = imageReader.ReadInt16();
            _transparent = imageReader.ReadColor();
            // Logger.Log($"Loading image {Handle.ToString(),4} Size: {_width,4}x{_height,4}");
            byte[] imageData;
            if(Settings.twofiveplus) Flags["LZX"] = false;
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
                    case 8:
                    {
                        Logger.Log("Reading 32-bit color");
                        (_colorArray, bytesRead) = ImageHelper.Read32(imageData, _width, _height);
                        break; 
                    }
                    default:
                    {
                        Logger.Log("Unknown Color Mode");
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
            else if (_transparent != null)
            {
                for (int i = 0; i < (_height * _width * 4)-3; i++)
                {
                    if (_colorArray[i+1]==_transparent.R&&_colorArray[i+2]==_transparent.G&&_colorArray[i+3]==_transparent.B)
                    {
                        _colorArray[i] = _transparent.A;
                    }
                }
            }

            return;
        }

        public void Save(string filename)
        {
           
            try
            {
               if(Settings.twofiveplus) Logger.Log("Trying to save image");
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
            chunk.WriteColor(_transparent);
            if (Flags["LZX"])
            {
                chunk.WriteInt32(rawImg.Length);
                chunk.WriteBytes(compressedImg);
            }

            else
            {
                chunk.WriteBytes(rawImg);
            }

            writer.WriteInt32(Handle);
            // writer.WriteInt32(Handle-1);//FNAC3 FIX
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