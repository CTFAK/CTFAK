using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.mmfparser;
using NetMFAPatcher.mmfparser.chunkloaders;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.mmfparser.Constants;

namespace NetMFAPatcher.MMFParser.Data
{
    public class GameData
    {
        public int runtime_version;
        public int runtime_subversion;
        public int product_build;
        public int product_version;
        public Products build;
        public ChunkList gameChunks;

        public string Name;
        public string Author;
        public string Copyright;
        public string aboutText;
        public string Doc;

        public string EditorFilename;
        public string TargetFilename;

        //public ExeOnly Exe_Only;

        public AppMenu Menu;
        public AppIcon Icon;

        public AppHeader Header;
        //public ExtentedHeader ExtHeader;

        public FontBank Fonts;
        public SoundBank Sounds;
        public MusicBank Music;
        public ImageBank Images;

        public GlobalValues GValues;
        public GlobalStrings GStrings;
        public static FrameItems testItems;

        //public Extensions Ext;

        public FrameItems Frameitems;

        public List<Frame> Frames = new List<Frame>();


        public void Read(ByteIO exeReader)
        {
            string magic = exeReader.ReadAscii(4); //Reading header

            //Checking for header
            if (magic == Constants.UNICODE_GAME_HEADER) Constants.isUnicode = true;//PAMU
            else if (magic == Constants.GAME_HEADER) Constants.isUnicode = false;//PAME
            else Logger.Log("Header Fucked Up", true, ConsoleColor.Red);//Header not found

            runtime_version = exeReader.ReadUInt16(); //
            runtime_subversion = exeReader.ReadUInt16(); //0
            product_version = exeReader.ReadInt32();  //CTF/MMF2/MMF1.5/CNC
            product_build = exeReader.ReadInt32(); //CTF Build
            build = (Products)runtime_version;

            //Print();
            //Logger.Log("Press any key to continue", true, ConsoleColor.Magenta);
            //Console.ReadKey();
            

            gameChunks = new ChunkList(); //Reading game chunks
            gameChunks.Read(exeReader);

            //Load chunks into gamedata for easier access
            if (gameChunks.get_chunk<AppName>() != null) Name = gameChunks.get_chunk<AppName>().value;
            if (gameChunks.get_chunk<Copyright>() != null) Copyright = gameChunks.get_chunk<Copyright>().value;
            if(gameChunks.get_chunk<AppAuthor>()!=null) Author = gameChunks.get_chunk<AppAuthor>().value;
            if (gameChunks.get_chunk<EditorFilename>() != null) EditorFilename = gameChunks.get_chunk<EditorFilename>().value;
            if (gameChunks.get_chunk<TargetFilename>() != null) TargetFilename = gameChunks.get_chunk<TargetFilename>().value;
            if (gameChunks.get_chunk<AppMenu>() != null) Menu = gameChunks.get_chunk<AppMenu>();
            if (gameChunks.get_chunk<AppHeader>() != null) Header = gameChunks.get_chunk<AppHeader>();
            if (gameChunks.get_chunk<SoundBank>() != null) Sounds = gameChunks.get_chunk<SoundBank>();
            if (gameChunks.get_chunk<MusicBank>() != null) Music = gameChunks.get_chunk<MusicBank>();
            if (gameChunks.get_chunk<FontBank>() != null) Fonts = gameChunks.get_chunk<FontBank>();
            if (gameChunks.get_chunk<ImageBank>() != null) Images = gameChunks.get_chunk<ImageBank>();
            if (gameChunks.get_chunk<AppIcon>() != null) Icon = gameChunks.get_chunk<AppIcon>();
            if (gameChunks.get_chunk<GlobalStrings>() != null) GStrings = gameChunks.get_chunk<GlobalStrings>();
            if (gameChunks.get_chunk<GlobalValues>() != null) GValues = gameChunks.get_chunk<GlobalValues>();
            if (gameChunks.get_chunk<FrameItems>() != null) Frameitems = gameChunks.get_chunk<FrameItems>();
            Frames = gameChunks.Frames;

            Print();
        }
        public void Print()
        {
            Logger.Log($"GameData Info:", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Runtime Version: {runtime_version}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Runtime Subversion: { runtime_subversion}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Product Version: { ((Products)product_version).ToString()}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Product Build: {product_build}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    {(isUnicode ? "Unicode" : "NonUnicode")} Game", true, ConsoleColor.DarkGreen);
            Logger.Log($"Game Info:", true, ConsoleColor.Cyan);
            Logger.Log($"    Name:{Name}", true, ConsoleColor.Cyan);
            Logger.Log($"    Author:{Author}", true, ConsoleColor.Cyan);
            Logger.Log($"    Copyright:{Copyright}", true, ConsoleColor.Cyan);
            Logger.Log($"    Editor Filename:{EditorFilename}", true, ConsoleColor.Cyan);
            Logger.Log($"    Target Filename:{TargetFilename}", true, ConsoleColor.Cyan);
            Logger.Log($"    Screen Resolution: {Header.windowWidth}x{Header.windowHeight}", true, ConsoleColor.Cyan);

            Logger.Log($"    Frame Count:{Header.numberOfFrames}", true, ConsoleColor.Cyan);
            if (GStrings != null && GStrings.items.Count > 0)
            {


                Logger.Log($"    Global Strings:", true, ConsoleColor.Cyan);
                foreach (var item in GStrings.items)
                {
                    Logger.Log($"       {item}");
                }
            }
            if (GValues != null && GValues.items.Count > 0)
            {


                Logger.Log($"    Global Values:", true, ConsoleColor.Cyan);
                foreach (var item in GValues.items)
                {
                    Logger.Log($"       {item.ToString()}");
                }
            }
            if(Frames!=null&&Frames.Count>0)
            {
                Logger.Log("Frames: ", true, ConsoleColor.Cyan);
                foreach (var item in Frames)
                {
                    Logger.Log($"       Frame: {item.name}, Size: {item.width}x{item.height}, Number of objects: {item.CountOfObjs}", true, ConsoleColor.Cyan);
                }
                

            }






        }


    }
}
