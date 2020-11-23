

using NetMFAPatcher.Utils;
using mmfparser.mfaloaders;
using NetMFAPatcher.chunkloaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.mmfparser;
using NetMFAPatcher.mmfparser.mfaloaders;
using NetMFAPatcher.mmfparser.chunkloaders;
using System.IO;
using Frame = NetMFAPatcher.mmfparser.mfaloaders.Frame;
using mmfparser;

namespace NetMFAPatcher.mfa
{
    class MFA : DataLoader
    {
        public int mfaBuild;
        public int product;
        public int buildVersion;

        public string name;
        public string description;
        public string path;
        public string author;
        public string copyright;
        public string company;
        public string version;

        public byte[] stamp;

        public int windowX;
        public int windowY;

        public ValueList globalValues;
        public ValueList globalStrings;
        

        public override void Print()
        {
            //Logger.Log($"MFA Product:{product}");
            //Logger.Log($"MFA Build:{mfaBuild}");
            //Logger.Log($"MFA Product:{buildVersion}");

        }

        public override void Read()
        {
            Logger.Log($"MFA HEADER:{reader.ReadAscii(4)}");
            mfaBuild = reader.ReadInt32();           
            product = reader.ReadInt32();            
            buildVersion = reader.ReadInt32();
            
            name = reader.ReadAscii(reader.ReadInt32());
            description = reader.ReadAscii(reader.ReadInt32());
            path = reader.ReadAscii(reader.ReadInt32());
            stamp = reader.ReadBytes(reader.ReadInt32());
            Console.WriteLine(path);
            


            if(reader.ReadAscii(4)!="ATNF")
            {
                throw new Exception("Invalid Font Bank");
            }
            var fonts = new FontBank(reader);
            fonts.Read();


            if (reader.ReadAscii(4) != "APMS")
            {
                throw new Exception("Invalid Sound Bank");
            }
            var sounds = new SoundBank(reader);
            sounds.isCompressed = false;
            sounds.Read();


            if (reader.ReadAscii(4) != "ASUM")
            {
                throw new Exception("Invalid Music Bank");
            }
            var music = new MusicBank(reader);
            music.Read();

            //if (reader.ReadAscii(4) != "AGMI")
            //{
            //    throw new Exception("Invalid Icon Bank");
            //}
            //var icons = new AGMIBank(reader);
            //icons.Read();

            reader.Seek(17710);//hardcoded offset

            reader.ReadInt32();//checkDefault
            author = reader.ReadAscii(reader.ReadInt32());
            reader.ReadInt32();//checkDefault

            copyright = reader.ReadAscii(reader.ReadInt32());
            reader.ReadInt32();//checkDefault

            company = reader.ReadAscii(reader.ReadInt32());
            version = reader.ReadAscii(reader.ReadInt32());
            windowX = reader.ReadInt32();
            windowY = reader.ReadInt32();
            var borderColor = reader.ReadBytes(4);
            var displayFlags = reader.ReadInt32();
            var graphicFlags = reader.ReadInt32();
            var helpFile = reader.ReadBytes(reader.ReadInt32());
            var vitalizePreview = reader.ReadBytes(reader.ReadInt32());
            var initialScore = reader.ReadInt32();
            var initialLifes = reader.ReadInt32();
            var frameRate = reader.ReadInt32();
            var buildType = reader.ReadInt32();
            var buildPath = reader.ReadAscii(reader.ReadInt32());
            reader.ReadInt32();
            var commandLine = reader.ReadAscii(reader.ReadInt32());
            var aboutbox = reader.ReadAscii(reader.ReadInt32());
            reader.ReadInt32();
            var binCount = reader.ReadInt32();//wtf i cant put it in loop fuck shit

            for (int i = 0; i < binCount; i++)
            {
                reader.ReadBytes(reader.ReadInt32());//binaryfiles
            }
            var controls = new mmfparser.mfaloaders.Controls(reader);
            controls.Read();

            var menuSize = reader.ReadUInt32();
            var currentPosition = reader.Tell();
            var menu = new AppMenu(reader);
            menu.Read();
            reader.Seek(menuSize + currentPosition);

            var windowMenuIndex = reader.ReadInt32();
            int[] menuImages = new int[65535];//govnokod suka
            var MICount = reader.ReadInt32();
            for (int i = 0; i < MICount; i++)
            {
                var id = reader.ReadInt32();
                menuImages[id] = reader.ReadInt32();
            }


            

            globalValues = new ValueList(reader);
            globalValues.Read();
            globalStrings = new ValueList(reader);
            globalStrings.Read();
            var globalEvents = reader.ReadBytes(reader.ReadInt32());
            var graphicMode = reader.ReadInt32();
            


            var icoCount = reader.ReadInt32();
            for (int i = 0; i < icoCount; i++)
            {
                reader.ReadInt32();
            }

            var qualCount = reader.ReadInt32();
            for (int i = 0; i < qualCount; i++)//qualifiers
            {
                var nameQ = reader.ReadAscii(reader.ReadInt32());
                var handleQ = reader.ReadInt32();
            }
            var extCount = reader.ReadInt32();
            for (int i = 0; i < extCount; i++)//extensions
            {
                var handleE = reader.ReadInt32();
                var filenameE = reader.ReadAscii(reader.ReadInt32());
                var nameE = reader.ReadAscii(reader.ReadInt32());
                var magicE = reader.ReadInt32();
                var subType = reader.ReadBytes(reader.ReadInt32());


            }
            List<int> frameOffsets = new List<int>();
            var offCount = reader.ReadInt32();
            for (int i = 0; i < offCount; i++)
            {
                frameOffsets.Add(reader.ReadInt32());
            }
            var nextOffset = reader.ReadInt32();
            foreach (var item in frameOffsets)
            {
                reader.Seek(item);
                var testframe = new Frame(reader);
                testframe.Read();
                

            }









        }
        public MFA(ByteIO reader) : base(reader)
        {
        }

        
    }
}
