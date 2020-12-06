using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetMFAPatcher.MMFParser.ChunkLoaders;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.Utils;


namespace NetMFAPatcher.MMFParser.Data
{
    public class GameData
    {
        public int RuntimeVersion;
        public int RuntimeSubversion;
        public int ProductBuild;
        public Constants.Products ProductVersion;
        public int Build;
        public ChunkList GameChunks;

        public string Name;
        public string Author;
        public string Copyright;
        public string AboutText;
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
        public static FrameItems TestItems;

        //public Extensions Ext;

        public FrameItems Frameitems;

        public List<Frame> Frames = new List<Frame>();


        public void Read(ByteReader exeReader)
        {
            string magic = exeReader.ReadAscii(4); //Reading header

            //Checking for header
            if (magic == Constants.UnicodeGameHeader) Constants.IsUnicode = true;//PAMU
            else if (magic == Constants.GameHeader) Constants.IsUnicode = false;//PAME
            else Logger.Log("Couldn't found any known headers", true, ConsoleColor.Red);//Header not found

            RuntimeVersion = exeReader.ReadUInt16(); //
            RuntimeSubversion = exeReader.ReadUInt16(); //0
            ProductVersion = (Constants.Products)exeReader.ReadInt32();  //CTF/MMF2/MMF1.5/CNC
            ProductBuild = exeReader.ReadInt32(); //CTF Build
            Settings.Build=ProductBuild;
            
            Build = ProductBuild;

            GameChunks = new ChunkList(); //Reading game chunks
            GameChunks.Read(exeReader);

            //Load chunks into gamedata for easier access
            if (GameChunks.get_chunk<AppName>() != null) Name = GameChunks.get_chunk<AppName>().Value;
            if (GameChunks.get_chunk<Copyright>() != null) Copyright = GameChunks.get_chunk<Copyright>().Value;
            if (GameChunks.get_chunk<AppAuthor>()!=null) Author = GameChunks.get_chunk<AppAuthor>().Value;
            if (GameChunks.get_chunk<EditorFilename>() != null) EditorFilename = GameChunks.get_chunk<EditorFilename>().Value;
            if (GameChunks.get_chunk<TargetFilename>() != null) TargetFilename = GameChunks.get_chunk<TargetFilename>().Value;
            if (GameChunks.get_chunk<AppMenu>() != null) Menu = GameChunks.get_chunk<AppMenu>();
            if (GameChunks.get_chunk<AppHeader>() != null) Header = GameChunks.get_chunk<AppHeader>();
            if (GameChunks.get_chunk<SoundBank>() != null) Sounds = GameChunks.get_chunk<SoundBank>();
            if (GameChunks.get_chunk<MusicBank>() != null) Music = GameChunks.get_chunk<MusicBank>();
            if (GameChunks.get_chunk<FontBank>() != null) Fonts = GameChunks.get_chunk<FontBank>();
            if (GameChunks.get_chunk<ImageBank>() != null) Images = GameChunks.get_chunk<ImageBank>();
            if (GameChunks.get_chunk<AppIcon>() != null) Icon = GameChunks.get_chunk<AppIcon>();
            if (GameChunks.get_chunk<GlobalStrings>() != null) GStrings = GameChunks.get_chunk<GlobalStrings>();
            if (GameChunks.get_chunk<GlobalValues>() != null) GValues = GameChunks.get_chunk<GlobalValues>();
            if (GameChunks.get_chunk<FrameItems>() != null) Frameitems = GameChunks.get_chunk<FrameItems>();
            Frames = GameChunks.Frames; //Its a list, so i have to manually parse them in chunk list. 

            Print();
        }
        public void Print()
        {
            Logger.Log($"GameData Info:", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Runtime Version: {RuntimeVersion}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Runtime Subversion: { RuntimeSubversion}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Product Version: { ((Constants.Products)ProductVersion).ToString()}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Product Build: {ProductBuild}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    {(Constants.IsUnicode ? "Unicode" : "NonUnicode")} Game", true, ConsoleColor.DarkGreen);
            Logger.Log($"Game Info:", true, ConsoleColor.Cyan);
            Logger.Log($"    Name:{Name}", true, ConsoleColor.Cyan);
            Logger.Log($"    Author:{Author}", true, ConsoleColor.Cyan);
            Logger.Log($"    Copyright:{Copyright}", true, ConsoleColor.Cyan);
            Logger.Log($"    Editor Filename:{EditorFilename}", true, ConsoleColor.Cyan);
            Logger.Log($"    Target Filename:{TargetFilename}", true, ConsoleColor.Cyan);
            Logger.Log($"    Screen Resolution: {Header.WindowWidth}x{Header.WindowHeight}", true, ConsoleColor.Cyan);

            Logger.Log($"    Frame Count:{Header.NumberOfFrames}", true, ConsoleColor.Cyan);
            if (GStrings != null && GStrings.Items.Count > 0)
            {


                Logger.Log($"    Global Strings:", true, ConsoleColor.Cyan);
                foreach (var item in GStrings.Items)
                {
                    Logger.Log($"       {item}");
                }
            }
            if (GValues != null && GValues.Items.Count > 0)
            {


                Logger.Log($"    Global Values:", true, ConsoleColor.Cyan);
                foreach (var item in GValues.Items)
                {
                    Logger.Log($"       {item.ToString()}");
                }
            }
            if(Frames!=null&&Frames.Count>0)
            {
                Logger.Log("Frames: ", true, ConsoleColor.Cyan);
                foreach (var item in Frames)
                {
                    Logger.Log($"       Frame: {item.Name,25}, Size: {item.Width,4}x{item.Height,4}, Number of objects: {item.CountOfObjs,5}", true, ConsoleColor.Cyan);
                    var objects = item.Chunks.get_chunk<ObjectInstances>();
                    if (objects != null)
                    {
                        foreach (var obj in objects.Items)
                        {
                            Logger.Log($"           Object: {obj.Name}", true, ConsoleColor.Green);
                        }
                    }

                }
                

            }






        }


    }
}
