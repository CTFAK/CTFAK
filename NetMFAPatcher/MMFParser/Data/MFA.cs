using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.MMFParser.MFALoaders;
using NetMFAPatcher.Utils;
using Controls = NetMFAPatcher.MMFParser.MFALoaders.Controls;
using Frame = NetMFAPatcher.MMFParser.MFALoaders.Frame;

namespace NetMFAPatcher.MMFParser.Data
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
        public int VitalizePreview;
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
        private int[] menuImages;
        private byte[] GlobalEvents;
        private int GraphicMode;
        private int IcoCount;
        private int QualCount;
        public Controls Controls;
        public List<int> IconImages;
        public List<Tuple<int, string, string, int, byte[]>> Extensions;
        public List<Tuple<string, int>> CustomQuals;
        public List<Frame> Frames;


        public override void Print()
        {
            //Logger.Log($"MFA Product:{product}");
            //Logger.Log($"MFA Build:{mfaBuild}");
            //Logger.Log($"MFA Product:{buildVersion}");

        }
        public void Write(ByteWriter writer)
        {
            
            writer.WriteAscii("MFU2");
            writer.WriteInt32(MfaBuild);
            writer.WriteInt32(Product);
            writer.WriteInt32(BuildVersion);
            writer.WriteInt32(LangId);
            writer.AutoWriteUnicode(Name);
            writer.AutoWriteUnicode(Description);
            writer.AutoWriteUnicode(Path);

            writer.WriteUInt32((uint)Stamp.Length);
            writer.WriteBytes(Stamp);
            writer.WriteAscii(FontBankId);
            Fonts.Write(writer);
            writer.WriteAscii(SoundBankId);
            Sounds.Write(writer);
            writer.WriteAscii(MusicBankId);
            //music.Write();//cum cum cum cum cum cum cum cum
            writer.WriteInt32(0);//someone is using musics lol?
            writer.WriteAscii(ImageBankId);
            Icons.Write(writer);
            writer.WriteAscii(ImageBankId);
            Images.Write(writer);
            writer.AutoWriteUnicode(Name);
            writer.AutoWriteUnicode(Author);
            writer.AutoWriteUnicode(Description);
            writer.AutoWriteUnicode(Copyright);
            writer.AutoWriteUnicode(Company);
            writer.AutoWriteUnicode(Version);
            writer.WriteInt32(WindowX);
            writer.WriteInt32(WindowY);
            writer.WriteColor(Color.White);
            writer.WriteInt32((int) DisplayFlags.flag);
            writer.WriteInt32((int) GraphicFlags.flag);
            writer.WriteUInt32((uint) InitialScore);
            writer.WriteUInt32((uint) InitialLifes);
            writer.WriteInt32(FrameRate);
            writer.WriteInt32(BuildType);
            writer.AutoWriteUnicode(BuildPath);
            writer.WriteInt32(0);
            writer.AutoWriteUnicode(CommandLine);
            writer.AutoWriteUnicode(Aboutbox);
            writer.WriteInt32(0);//anaconda
            writer.WriteInt32(0);//binary files are not supported because i am lazy asshole
            Controls.Write(writer);
            Menu = null; //cunt
            if (Menu != null)
            {
                byte[] menuData = new byte[15]; //Menu.Generate;
                writer.WriteInt32(menuData.Length);
                writer.WriteBytes(menuData);
            }
            else
            {
                writer.WriteInt32(0);
            }
            writer.WriteInt32(windowMenuIndex);
            writer.WriteInt32(menuImages.Length);
            foreach (var item in menuImages)
            {
                writer.WriteInt32(item);
            }

            GlobalValues.Write(writer);
            GlobalStrings.Write(writer);
            writer.WriteInt32(GlobalEvents.Length);
            writer.WriteBytes(GlobalEvents);
            writer.WriteInt32(GraphicMode);
            writer.WriteInt32(IconImages.Count);
            foreach (var item in IconImages)
            {
                writer.WriteInt32(item);
            }
            
            //Qualifiers
            writer.WriteInt32(CustomQuals.Count);
            foreach (var item in CustomQuals)
            {
                writer.AutoWriteUnicode(item.Item1);
                writer.WriteInt32(item.Item2);
            }
            //Extensions
            
            foreach (var item in Extensions)
            {
                writer.WriteInt32(item.Item1);
                writer.AutoWriteUnicode(item.Item2);
                writer.AutoWriteUnicode(item.Item3);
                writer.WriteInt32(item.Item4);
                writer.WriteBytes(item.Item5);
                writer.WriteInt32(Frames.Count);
            }
            writer.WriteInt32(Extensions.Count);
            var startPosition = writer.Tell() + 4 * Frames.Count + 4;
            //help
            //how to implement write writer
            
            
            












        }

        public override void Read()
        {
            Logger.Log($"MFA HEADER:{Reader.ReadAscii(4)}\n");
            MfaBuild = Reader.ReadInt32();
            Product = Reader.ReadInt32();
            BuildVersion = Reader.ReadInt32();
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

            Helper.CheckPattern(Helper.AutoReadUnicode(Reader),Name);
            Author = Helper.AutoReadUnicode(Reader);
            Helper.CheckPattern(Helper.AutoReadUnicode(Reader),Description);
            Copyright = Helper.AutoReadUnicode(Reader);
            Company = Helper.AutoReadUnicode(Reader);
            Version = Helper.AutoReadUnicode(Reader);
            WindowX = Reader.ReadInt32();
            WindowY = Reader.ReadInt32();
            BorderColor = Reader.ReadColor();
            DisplayFlags.flag = Reader.ReadUInt32();
            GraphicFlags.flag = Reader.ReadUInt32();
            HelpFile = Helper.AutoReadUnicode(Reader);
            VitalizePreview = Reader.ReadInt32();
            InitialScore = Reader.ReadInt32();
            InitialLifes = Reader.ReadInt32();
            FrameRate = Reader.ReadInt32();
            BuildType = Reader.ReadInt32(); 
            BuildPath = Helper.AutoReadUnicode(Reader);
            //Console.WriteLine(BuildPath);
            //Helper.CheckPattern(Reader.ReadInt32(),0);
            Reader.ReadInt32();
            CommandLine = Helper.AutoReadUnicode(Reader);
            Aboutbox = Helper.AutoReadUnicode(Reader);
            Reader.ReadInt32();
            
            var binCount = Reader.ReadInt32();//wtf i cant put it in loop fuck shit

            for (int i = 0; i < binCount; i++)
            {
                Reader.ReadBytes(Reader.ReadInt32());//binaryfiles
            }
            Controls = new Controls(Reader);
            Controls.Read();

            MenuSize = Reader.ReadUInt32();
            var currentPosition = Reader.Tell();
            Menu = new AppMenu(Reader);
            Menu.Read();
            Reader.Seek(MenuSize + currentPosition);

            windowMenuIndex = Reader.ReadInt32();
            menuImages = new int[65535];//govnokod suka
            var miCount = Reader.ReadInt32();
            for (int i = 0; i < miCount; i++)
            {
                var id = Reader.ReadInt32();
                menuImages[id] = Reader.ReadInt32();
            }


            

            GlobalValues = new ValueList(Reader);
            GlobalValues.Read();
            GlobalStrings = new ValueList(Reader);
            GlobalStrings.Read();
            GlobalEvents = Reader.ReadBytes(Reader.ReadInt32());
            GraphicMode = Reader.ReadInt32();;
            


            IcoCount = Reader.ReadInt32();
            IconImages = new List<int>();
            for (int i = 0; i < IcoCount; i++)
            {
                IconImages.Add(Reader.ReadInt32());
            }
            
            //I STUCK HERE
            QualCount = Reader.ReadInt32();
            CustomQuals = new List<Tuple<string, int>>();
            for (int i = 0; i < QualCount; i++)//qualifiers
            {
                var name = Reader.ReadAscii(Reader.ReadInt32());
                var handle = Reader.ReadInt32();
                CustomQuals.Add(new Tuple<string,int>(name,handle));

            }
            var extCount = Reader.ReadInt32();
            Extensions = new List<Tuple<int, string, string, int, byte[]>>();
            for (int i = 0; i < extCount; i++)//extensions
            {
                var handle = Reader.ReadInt32();
                var filename = Helper.AutoReadUnicode(Reader);
                var name = Helper.AutoReadUnicode(Reader);
                var magic = Reader.ReadInt32();
                var data = Reader.ReadBytes(Reader.ReadInt32());
                var tuple = new Tuple<int,string,string,int,byte[]>(handle,filename,name,magic,data);
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
            var chunks = new MFALoaders.ChunkList(Reader);
            chunks.Read();
            return;








        }
        public MFA(ByteIO reader) : base(reader)
        {
        }

        
    }
}
