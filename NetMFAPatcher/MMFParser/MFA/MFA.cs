using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.MMFParser.EXE.Loaders;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Banks;
using DotNetCTFDumper.MMFParser.MFA.Loaders;
using DotNetCTFDumper.Utils;
using ChunkList = DotNetCTFDumper.MMFParser.MFA.Loaders.ChunkList;
using Controls = DotNetCTFDumper.MMFParser.MFA.Loaders.Controls;
using Frame = DotNetCTFDumper.MMFParser.MFA.Loaders.Frame;

namespace DotNetCTFDumper.MMFParser.MFA
{
    public class MFA : DataLoader
    {
        public static readonly string FontBankId = "ATNF";
        public static readonly string ImageBankId = "AGMI";
        public static readonly string MusicBankId = "ASUM";
        public static readonly string SoundBankId = "APMS";

        public int MfaBuild;
        public int Product;
        public int BuildVersion;
        public int LangId;

        public string Name;
        public string Description;
        public string Path;

        public List<byte[]> BinaryFiles = new List<byte[]>();

        public FontBank Fonts;
        public SoundBank Sounds;
        public MusicBank Music;
        public AgmiBank Icons;
        public AgmiBank Images;


        public string Author;
        public string Copyright;
        public string Company;
        public string Version;

        public byte[] Stamp;

        public int WindowX;
        public int WindowY;

        public ValueList GlobalValues;
        public ValueList GlobalStrings;
        public Color BorderColor;

        public BitDict DisplayFlags = new BitDict(new string[]
        {
            "MaximizedOnBoot",
            "ResizeDisplay",
            "FullscreenAtStart",
            "AllowFullscreen",
            "Heading",
            "HeadingWhenMaximized",
            "MenuBar",
            "MenuOnBoot",
            "NoMinimize",
            "NoMaximize",
            "NoThickFrame",
            "NoCenter",
            "DisableClose",
            "HiddenAtStart",
            "MDI"
        });

        public BitDict GraphicFlags = new BitDict(new string[]
        {
            "MultiSamples",
            "SpeedIndependent",
            "SoundsOverFrames",
            "PlaySamplesWhenUnfocused",
            "IgnoreInputOnScreensaver",
            "DirectX",
            "VRAM",
            "EnableVisualThemes",
            "VSync",
            "RunWhenMinimized",
            "RunWhenResizing",
            "EnableDebuggerShortcuts",
            "NoDebugger",
            "NoSubappSharing"
        });

        public string HelpFile;
        public string unknown_string; //Found in original mfa build 283 after help file
        public string unknown_string_2; //Found in original mfa build 283 after build path
        public byte[] VitalizePreview;
        public int InitialScore;
        public int InitialLifes;
        public int FrameRate;
        public int BuildType;
        public string BuildPath;
        public string CommandLine;
        public string Aboutbox;
        public uint MenuSize;
        public AppMenu Menu;
        private int windowMenuIndex;
        private Dictionary<Int32, Int32> menuImages;
        private byte[] GlobalEvents;
        private int GraphicMode;
        private int IcoCount;
        private int QualCount;
        public Controls Controls;
        public List<int> IconImages;
        public List<Tuple<int, string, string, int, byte[]>> Extensions;
        public List<Tuple<string, int>> CustomQuals;
        public List<Frame> Frames;
        public ChunkList Chunks;

        public static Events emptyEvents;
        public static ChunkList emptyFrameChunks;


        public override void Print()
        {
            //Logger.Log($"MFA Product:{product}");
            //Logger.Log($"MFA Build:{mfaBuild}");
            //Logger.Log($"MFA Product:{buildVersion}");
        }

        public override void Write(ByteWriter Writer)
        {

            Writer.WriteAscii("MFU2");
            Writer.WriteInt32(MfaBuild);
            Writer.WriteInt32(Product);
            Writer.WriteInt32(BuildVersion);
            Writer.WriteInt32(LangId);
            Writer.AutoWriteUnicode(Name);
            Writer.AutoWriteUnicode(Description);
            Writer.AutoWriteUnicode(Path);

            Writer.WriteUInt32((uint) Stamp.Length);
            Writer.WriteBytes(Stamp);
            Writer.WriteAscii(FontBankId);
            Fonts.Write(Writer);
            Writer.WriteAscii(SoundBankId);
            Sounds.Write(Writer);

            Writer.WriteAscii(MusicBankId);
            // music.Write();
            Writer.WriteInt32(0); //someone is using musics lol?
            //TODO: Do music

            Writer.WriteAscii(ImageBankId);
            Icons.Write(Writer);

            Writer.WriteAscii(ImageBankId);
            Images.Write(Writer);


            Writer.AutoWriteUnicode(Name);
            Writer.AutoWriteUnicode(Author);
            Writer.AutoWriteUnicode(Description);
            Writer.AutoWriteUnicode(Copyright);
            Writer.AutoWriteUnicode(Company);
            Writer.AutoWriteUnicode(Version);
            Writer.WriteInt32(WindowX);
            Writer.WriteInt32(WindowY);
            Writer.WriteColor(Color.FromArgb(0,255,255,255));
            Writer.WriteInt32((int) DisplayFlags.flag);
            Writer.WriteInt32((int) GraphicFlags.flag);
            Writer.AutoWriteUnicode(HelpFile);
            Writer.AutoWriteUnicode(unknown_string);
            Writer.WriteUInt32((uint) InitialScore);
            Writer.WriteUInt32((uint) InitialLifes);
            Writer.WriteInt32(FrameRate);
            Writer.WriteInt32(BuildType);
            Writer.AutoWriteUnicode(BuildPath);
            Writer.AutoWriteUnicode(unknown_string_2);
            Writer.AutoWriteUnicode(CommandLine);
            Writer.AutoWriteUnicode(Aboutbox);
            Writer.WriteInt32(0);

            Writer.WriteInt32(BinaryFiles.Count);
            foreach (byte[] binaryFile in BinaryFiles)
            {
                Writer.WriteInt32(binaryFile.Length);
                Writer.WriteBytes(binaryFile);
            }

            Controls.Write(Writer);
            
            
            if (Menu != null)
            {
                using (ByteWriter menuWriter = new ByteWriter(new MemoryStream()))
                {
                    Menu.Write(menuWriter);
                    
                    Writer.WriteUInt32((uint) menuWriter.BaseStream.Position);
                    Writer.WriteWriter(menuWriter);
                }
            }
            else
            {
                Writer.WriteInt32(0);
            }
            
            
            Writer.WriteInt32(windowMenuIndex);
            Writer.WriteInt32(menuImages.Count);
            foreach (KeyValuePair<int, int> valuePair in menuImages)
            {
                Writer.WriteInt32(valuePair.Key);
                Writer.WriteInt32(valuePair.Value);
            }
            GlobalValues.Write(Writer);
            GlobalStrings.Write(Writer);
            Writer.WriteInt32(GlobalEvents.Length);
            Writer.WriteBytes(GlobalEvents);
            Writer.WriteInt32(GraphicMode);
            Writer.WriteUInt32((uint) IconImages.Count);
            foreach (int iconImage in IconImages)
            {
                Writer.WriteInt32(iconImage);
            }
            Writer.WriteInt32(CustomQuals.Count);
            foreach (Tuple<string,int> customQual in CustomQuals)
            {
                Writer.AutoWriteUnicode(customQual.Item1);
                Writer.WriteInt32(customQual.Item2);
            }
            Writer.WriteInt32(Extensions.Count);
            foreach(var extension in Extensions)
            {
                Writer.WriteInt32(extension.Item1);
                Writer.WriteUnicode(extension.Item2);
                Writer.WriteUnicode(extension.Item3);
                Writer.WriteInt32(extension.Item4);
                Writer.WriteBytes(extension.Item5);

            }
            
            Writer.WriteInt32(Frames.Count); //frame
            var startPos = Writer.Tell() + 4 * Frames.Count + 4;
            
            ByteWriter newWriter = new ByteWriter(new MemoryStream());
            foreach (Frame frame in Frames)
            {
                Writer.WriteUInt32((uint) (startPos+newWriter.Tell()+4));
                frame.Write(newWriter);
            }
            Writer.WriteUInt32((uint) (startPos+newWriter.Tell()+4));
            Writer.WriteWriter(newWriter);
            Chunks.Write(Writer);



        }

        public override void Read()
        {
            
            Logger.Log($"MFA HEADER:{Reader.ReadAscii(4)}\n");
            MfaBuild = Reader.ReadInt32();
            Product = Reader.ReadInt32();
            BuildVersion = Reader.ReadInt32();
            Settings.Build = BuildVersion;
            LangId = Reader.ReadInt32();
            Name = Helper.AutoReadUnicode(Reader);
            Description = Helper.AutoReadUnicode(Reader);
            Path = Helper.AutoReadUnicode(Reader);
            Stamp = Reader.ReadBytes(Reader.ReadInt32());

            if (Reader.ReadAscii(4) != FontBankId) throw new Exception("Invalid Font Bank");
            Fonts = new FontBank(Reader);
            Fonts.Read();

            if (Reader.ReadAscii(4) != SoundBankId) throw new Exception("Invalid Sound Bank");
            Sounds = new SoundBank(Reader);
            Sounds.IsCompressed = false;
            Sounds.Read();

            if (Reader.ReadAscii(4) != MusicBankId) throw new Exception("Invalid Music Bank");
            Music = new MusicBank(Reader);
            Music.Read();

            if (Reader.ReadAscii(4) != "AGMI") throw new Exception("Invalid Icon Bank");
            Icons = new AgmiBank(Reader);
            Icons.Read();

            if (Reader.ReadAscii(4) != "AGMI") throw new Exception("Invalid Image Bank");
            Images = new AgmiBank(Reader);
            Images.Read();

            Helper.CheckPattern(Helper.AutoReadUnicode(Reader), Name);
            Author = Helper.AutoReadUnicode(Reader);
            Helper.CheckPattern(Helper.AutoReadUnicode(Reader), Description);
            Copyright = Helper.AutoReadUnicode(Reader);
            Company = Helper.AutoReadUnicode(Reader);
            Version = Helper.AutoReadUnicode(Reader);
            WindowX = Reader.ReadInt32();
            WindowY = Reader.ReadInt32();
            BorderColor = Reader.ReadColor();
            DisplayFlags.flag = Reader.ReadUInt32();
            GraphicFlags.flag = Reader.ReadUInt32();
            HelpFile = Helper.AutoReadUnicode(Reader);
            unknown_string = Helper.AutoReadUnicode(Reader);
            //Int32 vit_size = Reader.ReadInt32();
            //VitalizePreview = Reader.ReadBytes(vit_size);
            
            InitialScore = Reader.ReadInt32();
            InitialLifes = Reader.ReadInt32();
            FrameRate = Reader.ReadInt32();
            BuildType = Reader.ReadInt32();
            BuildPath = Helper.AutoReadUnicode(Reader);
            unknown_string_2 = Helper.AutoReadUnicode(Reader);
            CommandLine = Helper.AutoReadUnicode(Reader);
            Aboutbox = Helper.AutoReadUnicode(Reader);
            Reader.ReadUInt32();

            var binCount = Reader.ReadInt32(); //wtf i cant put it in loop fuck shit

            for (int i = 0; i < binCount; i++)
            {
                BinaryFiles.Add(Reader.ReadBytes(Reader.ReadInt32()));
            }

            Controls = new Controls(Reader);
            Controls.Read();

            MenuSize = Reader.ReadUInt32();
            var currentPosition = Reader.Tell();
            Menu = new AppMenu(Reader);
            Menu.Read();
            Reader.Seek(MenuSize + currentPosition);

            windowMenuIndex = Reader.ReadInt32();
            menuImages = new Dictionary<Int32, Int32>();
            int miCount = Reader.ReadInt32();
            for (int i = 0; i < miCount; i++)
            {
                int id = Reader.ReadInt32();
                menuImages[id] = Reader.ReadInt32();
            }


            GlobalValues = new ValueList(Reader);
            GlobalValues.Read();
            GlobalStrings = new ValueList(Reader);
            GlobalStrings.Read();
            GlobalEvents = Reader.ReadBytes(Reader.ReadInt32());
            GraphicMode = Reader.ReadInt32();
            


            IcoCount = Reader.ReadInt32();
            IconImages = new List<int>();
            for (int i = 0; i < IcoCount; i++)
            {
                IconImages.Add(Reader.ReadInt32());
            }
            
            QualCount = Reader.ReadInt32();
            CustomQuals = new List<Tuple<string, int>>();
            for (int i = 0; i < QualCount; i++) //qualifiers
            {
                var name = Reader.ReadAscii(Reader.ReadInt32());
                var handle = Reader.ReadInt32();
                CustomQuals.Add(new Tuple<string, int>(name, handle));
            }

            var extCount = Reader.ReadInt32();
            Extensions = new List<Tuple<int, string, string, int, byte[]>>();
            for (int i = 0; i < extCount; i++) //extensions
            {
                var handle = Reader.ReadInt32();
                var filename = Helper.AutoReadUnicode(Reader);
                var name = Helper.AutoReadUnicode(Reader);
                var magic = Reader.ReadInt32();
                var data = Reader.ReadBytes(Reader.ReadInt32());
                var tuple = new Tuple<int, string, string, int, byte[]>(handle, filename, name, magic, data);
                Extensions.Add(tuple);
            }

            List<int> frameOffsets = new List<int>();
            var offCount = Reader.ReadInt32();
            for (int i = 0; i < offCount; i++)
            {
                frameOffsets.Add(Reader.ReadInt32());
            }
            

            var nextOffset = Reader.ReadInt32();
            Frames = new List<Frame>();
            foreach (var item in frameOffsets)
            {
                Reader.Seek(item);
                var testframe = new Frame(Reader);
                testframe.Read();
                Frames.Add(testframe);
            }

            Reader.Seek(nextOffset);
            Chunks = new ChunkList(Reader);
            Chunks.Read();
            Reader.Dispose();
            return;
        }

        

        public MFA(ByteReader reader) : base(reader)
        {
        }
    }
}