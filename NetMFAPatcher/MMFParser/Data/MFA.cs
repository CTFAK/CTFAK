using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Frame = NetMFAPatcher.mmfparser.mfaloaders.Frame;
using mmfparser;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using NetMFAPatcher.MMFParser.ChunkLoaders.banks;
using NetMFAPatcher.mmfparser.mfaloaders;
using NetMFAPatcher.MMFParser.MFALoaders;
using NetMFAPatcher.utils;

namespace NetMFAPatcher.mfa
{
    class Mfa : DataLoader
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


        public string Author;
        public string Copyright;
        public string Company;
        public string Version;

        public byte[] Stamp;

        public int WindowX;
        public int WindowY;

        public ValueList GlobalValues;
        public ValueList GlobalStrings;
        

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




            


        }

        public override void Read()
        {
            Logger.Log($"MFA HEADER:{Reader.ReadAscii(4)}\n");
            MfaBuild = Reader.ReadInt32();
            Product = Reader.ReadInt32();
            BuildVersion = Reader.ReadInt32();
            Console.WriteLine($"mfaBuild: {MfaBuild}, product: {Product}, buildVersion: {BuildVersion}");
            LangId = Reader.ReadInt32();

            
            Name = Helper.AutoReadUnicode(Reader);
            

            
            Description = Helper.AutoReadUnicode(Reader);

            
            Path = Helper.AutoReadUnicode(Reader);
            Console.WriteLine($"\nMFAName: {Name}\nDescription: {Description}\nPath: {Path}");

            Stamp = Reader.ReadBytes(Reader.ReadInt32());
            
            if (Reader.ReadAscii(4) != "ATNF")
            {
                throw new Exception("Invalid Font Bank");
            }
            Fonts = new FontBank(Reader);
            Fonts.Read();
            Console.WriteLine("FONTS: " + Fonts.NumberOfItems);


            if (Reader.ReadAscii(4) != "APMS")
            {
                throw new Exception("Invalid Sound Bank");
            }
            Sounds = new SoundBank(Reader);
            Sounds.IsCompressed = false;
            Sounds.Read();


            if (Reader.ReadAscii(4) != "ASUM")
            {
                throw new Exception("Invalid Music Bank");
            }
            Music = new MusicBank(Reader);
            Music.Read();
            
            if (Reader.ReadAscii(4) != "AGMI")
            {
                throw new Exception("Invalid Icon Bank");
            }
            var icons = new AgmiBank(Reader);
            icons.Read();
            if (Reader.ReadAscii(4) != "AGMI")
            {
                throw new Exception("Invalid Image Bank");
            }
            var images = new AgmiBank(Reader);
            images.Read();

            
            if (Helper.AutoReadUnicode(Reader) != Name) throw new Exception("Invalid name");

            
            Author = Helper.AutoReadUnicode(Reader);
            

            
            var newDesc = Helper.AutoReadUnicode(Reader);
            if ( newDesc!= Description) throw new Exception("Invalid description: "+newDesc);



            
            Copyright = Helper.AutoReadUnicode(Reader);


            Company = Helper.AutoReadUnicode(Reader);
            Console.WriteLine("Company: "+Company);
            Version = Helper.AutoReadUnicode(Reader);
            Console.WriteLine("Version: " + Version);
            WindowX = Reader.ReadInt32();
            WindowY = Reader.ReadInt32();
            Console.WriteLine($"Window:{WindowX}x{WindowY}");
            var borderColor = Reader.ReadColor();
            var displayFlags = Reader.ReadUInt32();
            var graphicFlags = Reader.ReadUInt32();
            var helpFile = Helper.AutoReadUnicode(Reader);
            Console.WriteLine(Reader.Tell());
            var vitalizePreview = Reader.ReadInt32();
            var initialScore = Reader.ReadInt32();
            var initialLifes = Reader.ReadInt32();
            var frameRate = Reader.ReadInt32();
            var buildType = Reader.ReadInt32();
            var buildPath = Helper.AutoReadUnicode(Reader);
            Reader.ReadInt32();
            var commandLine = Helper.AutoReadUnicode(Reader);
            var aboutbox = Helper.AutoReadUnicode(Reader);
            
            Console.WriteLine(aboutbox);
            Reader.ReadInt32();
            var binCount = Reader.ReadInt32();//wtf i cant put it in loop fuck shit

            for (int i = 0; i < binCount; i++)
            {
                Reader.ReadBytes(Reader.ReadInt32());//binaryfiles
            }
            var controls = new mmfparser.mfaloaders.Controls(Reader);
            controls.Read();

            var menuSize = Reader.ReadUInt32();
            var currentPosition = Reader.Tell();
            var menu = new AppMenu(Reader);
            menu.Read();
            Reader.Seek(menuSize + currentPosition);

            var windowMenuIndex = Reader.ReadInt32();
            int[] menuImages = new int[65535];//govnokod suka
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
            var globalEvents = Reader.ReadBytes(Reader.ReadInt32());
            var graphicMode = Reader.ReadInt32();;
            


            var icoCount = Reader.ReadInt32();
            for (int i = 0; i < icoCount; i++)
            {
                Reader.ReadInt32();
            }

            var qualCount = Reader.ReadInt32();
            for (int i = 0; i < qualCount; i++)//qualifiers
            {
                var nameQ = Reader.ReadAscii(Reader.ReadInt32());
                var handleQ = Reader.ReadInt32();
            }
            var extCount = Reader.ReadInt32();
            for (int i = 0; i < extCount; i++)//extensions
            {
                var handleE = Reader.ReadInt32();
                var filenameE = Helper.AutoReadUnicode(Reader);
                var nameE = Helper.AutoReadUnicode(Reader);
                var magicE = Reader.ReadInt32();
                var subType = Reader.ReadBytes(Reader.ReadInt32());


            }
            List<int> frameOffsets = new List<int>();
            var offCount = Reader.ReadInt32();
            for (int i = 0; i < offCount; i++)
            {
                frameOffsets.Add(Reader.ReadInt32());
            }
            var nextOffset = Reader.ReadInt32();
            foreach (var item in frameOffsets)
            {
                Reader.Seek(item);
                var testframe = new Frame(Reader);
                testframe.Read();
                

            }
            Reader.Seek(nextOffset);
            var chunks = new ChunkList(Reader);
            chunks.Read();
            return;








        }
        public Mfa(ByteIO reader) : base(reader)
        {
        }

        
    }
}
