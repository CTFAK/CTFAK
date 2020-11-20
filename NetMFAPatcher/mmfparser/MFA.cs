

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

        public override void Print()
        {
            //Logger.Log($"MFA Product:{product}");
            //Logger.Log($"MFA Build:{mfaBuild}");
            //Logger.Log($"MFA Product:{buildVersion}");

        }

        public override void Read()
        {
            Logger.Log($"MFA HEADER:{reader.ReadAscii(4)}");
            var mfaBuild = reader.ReadInt32();           
            var product = reader.ReadInt32();            
            var buildVersion = reader.ReadInt32();
            
            var name = reader.ReadAscii(reader.ReadInt32());
            var description = reader.ReadAscii(reader.ReadInt32());
            var path = reader.ReadAscii(reader.ReadInt32());
            var stamp = reader.ReadBytes(reader.ReadInt32());
            


            Logger.Log(reader.ReadAscii(4));
            var fonts = new FontBank(reader);
            fonts.Read();

            Logger.Log("FontsDone\n");

            Logger.Log(reader.ReadAscii(4));
            var sounds = new SoundBank(reader);
            sounds.isCompressed = false;
            sounds.Read();

            Logger.Log("SoundsDone\n");

            Logger.Log(reader.ReadAscii(4));
            var music = new MusicBank(reader);
            music.Read();

            Logger.Log("MusicDone\n");

            //Logger.Log(reader.ReadAscii(4));
            //var icons = new AGMIBank(reader);
            //icons.Read();

            //Logger.Log("IconDone\n");

            //Logger.Log(reader.ReadAscii(4));



            reader.Seek(1191186);

            var author = reader.ReadAscii(reader.ReadInt32());
            reader.ReadInt32();
            var copyright = reader.ReadAscii(reader.ReadInt32());
            reader.ReadInt32();
            var company = reader.ReadAscii(reader.ReadInt32());
            var version = reader.ReadAscii(reader.ReadInt32());
            var windowX = reader.ReadInt32();
            var windowY = reader.ReadInt32();
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


            

            var globalValues = new ValueList(reader);
            globalValues.Read();
            var globalStrings = new ValueList(reader);
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
            for (int i = 0; i < extCount; i++)
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
                Console.WriteLine($"Done reading frame '{testframe.name}'");

            }









        }
        public MFA(ByteIO reader) : base(reader)
        {
        }

        
    }
}
