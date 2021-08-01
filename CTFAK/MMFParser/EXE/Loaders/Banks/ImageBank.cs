using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using CTFAK.GUI;
using CTFAK.Utils;
using Joveler.Compression.ZLib;
using K4os.Compression.LZ4;
using static CTFAK.MMFParser.EXE.ChunkList;
using System.Threading;


namespace CTFAK.MMFParser.EXE.Loaders.Banks
{
    public class ImageBank : ChunkLoader
    {
        public bool SaveImages = false;
        public Dictionary<int, ImageItem> Images = new Dictionary<int, ImageItem>();
        public static bool Load = false;

        public ImageBank(ByteReader reader) : base(reader)
        {
        }

        

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Images.Count);
            foreach (ImageItem item in Images.Values)
            {
                item.Write(Writer);
            }
        }


        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Number of images: {Images.Count}"
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
            // if (Settings.GameType == GameType.OnePointFive && !Settings.DoMFA)
            // {
            // Load = true;
            // return;
            // }
            if (!Settings.DoMFA) Reader.Seek(0); //Reset the reader to avoid bugs when dumping more than once
            if(Settings.GameType==GameType.TwoFivePlus)
            {
                //File.WriteAllBytes("images.bank",Reader.ReadBytes((int)Reader.Size()));
                //return;
            }
            
            var tempImages = new Dictionary<int, ImageItem>();
            int count;
            if(Settings.GameType==GameType.TwoFivePlus)
            {
                //return;
            }
            if (Settings.GameType == GameType.Android)
            {
                return;
                Reader.Skip(2);
                count = Reader.ReadInt16();
            }
            else
            {
                count = Reader.ReadInt32();
            }
            Logger.Log($"Found {count} images", true, ConsoleColor.Green);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            //if (!Settings.DumpImages) return;
            Logger.Log("Reading Images", true, ConsoleColor.Green);
            for (int i = 0; i < count; i++)
            {
                if (MainForm.BreakImages) break;
                {
                    var item = new ImageItem(Reader);
                    item.Read();
                    if(Settings.GameType != GameType.Android)tempImages.Add(item.Handle, item);
                    if (SaveImages) item.Save($"{Settings.ImagePath}\\" + item.Handle.ToString() + ".png");
                    OnImageSaved?.Invoke(i, (int) count);
                }
            }

            stopWatch.Stop();
            Logger.Log(
                $"Images finished in {stopWatch.Elapsed.ToString("g")}, bytes left in bank: {Reader.Size() - Reader.Tell()}",
                true, ConsoleColor.Green);
            Load = true;
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
        public int HotspotX;
        public int HotspotY;
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

        Color _transparent;
        byte[] _colorArray;

        public byte[] rawImg;
        public byte[] rawAlpha;

        public bool Debug = false;
        private int decompressed_size = 0;
        private Bitmap _bitmap;

        public override void Read()
        {
            if ((Settings.GameType == GameType.TwoFivePlus) && !Settings.DoMFA)
            {
                Handle = Reader.ReadInt32();
                
                var unk2 = Reader.ReadInt32();
                var unk = Reader.ReadInt32();
                //Flags.flag = Reader.ReadUInt32();
                var unk3 = Reader.ReadInt32();
                var dataSize = Reader.ReadInt32();
                _width = Reader.ReadInt16(); //width
                _height = Reader.ReadInt16(); //height
                var unk6 = Reader.ReadInt16();
                var unk7= Reader.ReadInt16();
                HotspotX = Reader.ReadInt16();
                HotspotY = Reader.ReadInt16();
                ActionX = Reader.ReadInt16();
                ActionY = Reader.ReadInt16();
                _transparent = Reader.ReadColor();
                var decompressedSize = Reader.ReadInt32();
                rawImg = Reader.ReadBytes(dataSize-4);
                byte[] target = new byte[decompressedSize];
                string ll = LZ4Codec.Decode(rawImg, target);
                Console.WriteLine("Decoded: " + ll);
                Console.WriteLine("Cleaning up memory using Garbare Collectors..");
                int maxGarbage = 3000;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Console.WriteLine("Cleaned!");
                
                rawImg = target;
                _graphicMode = 16;
                
                Load();
                //Save($"{Settings.ImagePath}\\{Handle}.png");

                //Logger.Log(
                //       $"Loading image {_width}x{_height} with handle {Handle}, Size {dataSize},decompressedSize {decompressedSize}");


            }
            else if ((Settings.GameType == GameType.Android||Settings.GameType == GameType.NSwitch)&&!Settings.DoMFA)
            {
                Handle = Reader.ReadInt16();
                var unk = (uint) Reader.ReadInt32();

                _width = Reader.ReadInt16(); //width
                _height = Reader.ReadInt16(); //height
                HotspotX = Reader.ReadInt16();
                HotspotY = Reader.ReadInt16();
                ActionX = Reader.ReadInt16();
                ActionY = Reader.ReadInt16();
                _graphicMode = 16;
                var size = Reader.ReadInt32();
                var thefuk1 = Reader.PeekByte();
                var thefuk2 = Reader.PeekUInt16();
                ByteReader imageReader;
                Logger.Log(
                    $"Loading image {_width}x{_height} with handle {Handle}, Size {size}. Bytes per pixel: {size/(_width*_height)}");
                if (thefuk1 == 255)
                {
                    rawImg = Reader.ReadBytes(size);
                    return;
                }
                else
                {
                    imageReader = new ByteReader(Decompressor.DecompressBlock(Reader,size));
                    imageReader.Seek(0);
                    rawImg = imageReader.ReadBytes();
                }

                if (Handle == 88)
                {
                    
                    Load();
                    Environment.Exit(0);
                }
            }
            else
            {
                Handle = Reader.ReadInt32();
                if (!Debug) if (Settings.Build >= 284 && Settings.GameType != GameType.OnePointFive) Handle -= 1;
                _bitmap = null;

                ByteReader imageReader;

                if (Settings.GameType == GameType.OnePointFive && !Settings.DoMFA) imageReader = new ByteReader(Decompressor.DecompressOld(Reader));
                else if (!Settings.DoMFA) imageReader = Debug ? Reader : Decompressor.DecompressAsReader(Reader, out var a);
                else imageReader = Reader;

                if (Settings.GameType == GameType.OnePointFive && !Settings.DoMFA) _checksum = imageReader.ReadInt16();
                else _checksum = imageReader.ReadInt32();

                _references = imageReader.ReadInt32();
                Size = (int) imageReader.ReadUInt32();

                if (Debug) imageReader = new ByteReader(imageReader.ReadBytes(Size + 20));

                _width = imageReader.ReadInt16();
                _height = imageReader.ReadInt16();
                _graphicMode = imageReader.ReadByte();
                Flags.flag = imageReader.ReadByte();

                if (Settings.GameType != GameType.OnePointFive ||
                    (Settings.DoMFA && Settings.GameType == GameType.OnePointFive)) imageReader.Skip(2);

                HotspotX = imageReader.ReadInt16();
                HotspotY = imageReader.ReadInt16();
                ActionX = imageReader.ReadInt16();
                ActionY = imageReader.ReadInt16();
                if (Settings.GameType != GameType.OnePointFive ||
                    (Settings.DoMFA && Settings.GameType == GameType.OnePointFive))
                    _transparent = imageReader.ReadColor();
                byte[] imageData;
                if (Flags["LZX"])
                {
                    uint decompressedSize = imageReader.ReadUInt32();
                    
                    imageData = Decompressor.DecompressBlock(imageReader,
                        (int) (imageReader.Size() - imageReader.Tell()),
                        (int) decompressedSize);
                }
                else imageData = imageReader.ReadBytes((int)(Size));

                rawImg = imageData;
            }
            if(!Settings.DoMFA)Load();
        }


        public override string[] GetReadableData() { return null; }


        public void Load()
        {
            int bytesRead = 0;
            // if (!ImageBank.Load) return;

            if (Flags["RLE"] || Flags["RLEW"] || Flags["RLET"]) return;
            else
            {
                Console.WriteLine("-------------------------------------------------------------------Loading image of type " + _graphicMode + " (size: "+rawImg.Length+")");
                switch (_graphicMode)
                {
                    case 4:
                    {
                        (_colorArray, bytesRead) = ImageHelper.ReadPoint(rawImg, _width, _height);
                        break;
                    }
                    case 6:
                    {
                        (_colorArray, bytesRead) = ImageHelper.ReadFifteen(rawImg, _width, _height);
                        break;
                    }
                    case 7:
                    {
                        (_colorArray, bytesRead) = ImageHelper.ReadSixteen(rawImg, _width, _height);
                        break;
                    }
                    case 16: //just using this number for 2.5+
                    {
                        (_colorArray, bytesRead) = ImageHelper.Read32(rawImg, _width, _height);
                        break;
                    }
                    default:
                    {
                        Logger.Log("Unknown Color Mode: " + _graphicMode);
                        break;
                    }
                }
            }

            int alphaSize = Size - bytesRead;
            if (Flags["Alpha"])
            {
                byte[,] alpha = ImageHelper.ReadAlpha(rawImg, _width, _height, Size - alphaSize);
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
            //Alpha channel in 2.5+ is not separated from the image data. That's smart
            if (Settings.GameType != GameType.OnePointFive&&Settings.GameType!=GameType.TwoFivePlus)
            {
                
                if (Settings.Build > 283) // No idea why, but this is not working with old games
                {
                    if (_transparent != null&&!Flags["Alpha"])
                    {
                        for (int i = 0; i < (_height * _width * 4) - 3; i++)
                        {
                            if (_colorArray[i + 1] == _transparent.R && _colorArray[i + 2] == _transparent.G &&
                                _colorArray[i + 3] == _transparent.B)
                            {
                                _colorArray[4] = _transparent.A;
                            }
                        }
                    }
                }
            }

            // Logger.Log("ImageSize: "+_colorArray.Length);
            Save($@"{Settings.ImagePath}\\{Handle}.png");
            return;
        }

        public void Save(string filename)
        {
            try
            {
                //if (Settings.GameType == GameType.TwoFivePlus) Logger.Log("Trying to save image");
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
                if (_colorArray == null) Load();
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


        public override void Write(ByteWriter writer)
        {
            ByteWriter chunk = new ByteWriter(new MemoryStream());
            chunk.WriteInt32(_checksum);
            chunk.WriteInt32(_references);
            if (Settings.GameType == GameType.TwoFivePlus && _graphicMode == 16)
            {
                //Logger.Log("gMode " + _graphicMode);
                var rawBuffer = rawImg;
                var padBuffer = rawImg;
                int vag = 0;
                for (int pp = 0; pp < rawImg.Length; pp = pp + 4)
                {
                    rawBuffer[vag + 0] = rawImg[pp + 0];
                    rawBuffer[vag + 1] = rawImg[pp + 1];
                    rawBuffer[vag + 2] = rawImg[pp + 2];
                    vag = vag + 3;
                }
                Array.Resize(ref rawBuffer, 3 * rawImg.Length / 4);
                //Console.WriteLine("-----RawImg size: " + rawImg.Length + " RawBuffer size: " + rawBuffer.Length);
                rawImg = rawBuffer;
                Array.Resize(ref rawImg, rawBuffer.Length);
                //we need to fix the padding here
                int pad = GetPadding(_width, 3);
                int padPos = 0;
                for (int i = 0; i < rawBuffer.Length; i = i + 3)
                {
                    padBuffer[padPos + 0] = rawBuffer[i + 0];
                    padBuffer[padPos + 1] = rawBuffer[i + 1];
                    padBuffer[padPos + 2] = rawBuffer[i + 2];
                    padPos = padPos + 3;
                    if ((i / 3) % _width == 0) padPos = padPos + (pad * 3);
                }
                Console.WriteLine("--------------------------------------------------New size with padding is " + padPos + " (pad = "+pad+")");
                Array.Resize(ref padBuffer, padPos);
                rawImg = padBuffer;
                Array.Resize(ref rawImg, padPos);
                _graphicMode = 4;
            }
            byte[] compressedImg = null;
            Flags["LZX"] = true;
            if (Flags["LZX"])
            {
                compressedImg = Decompressor.compress_block(rawImg);
                chunk.WriteUInt32((uint)compressedImg.Length + 4);
            }
            else
            {
                chunk?.WriteUInt32((uint)(rawImg?.Length ?? 0));
            }

            chunk.WriteInt16((short)_width);
            chunk.WriteInt16((short)_height);
            chunk.WriteInt8((byte)_graphicMode);
            chunk.WriteInt8((byte)Flags.flag);
            chunk.WriteInt16(0);
            chunk.WriteInt16((short)HotspotX);
            chunk.WriteInt16((short)HotspotY);
            chunk.WriteInt16((short)ActionX);
            chunk.WriteInt16((short)ActionY);
            chunk.WriteColor(_transparent);
            if (Flags["LZX"])
            {
                Console.WriteLine("Adding LZX image (" + _width+"*" + _height + ") of size " + rawImg.Length + " to MFA");
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

        private int GetPadding(int width, int pointSize, int bytes = 2)
        {
                int pad = bytes - ((width * pointSize) % bytes);
                if (pad == bytes)
                {
                    return 0;
                }

                return (int)Math.Ceiling((double)((float)pad / (float)pointSize));
        }

        public ImageItem(ByteReader reader) : base(reader)
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
