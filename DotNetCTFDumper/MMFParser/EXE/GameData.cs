using System;
using System.Collections.Generic;
using DotNetCTFDumper.GUI;
using DotNetCTFDumper.MMFParser.EXE.Loaders;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Banks;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.EXE
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

            RuntimeVersion = exeReader.ReadUInt16(); 
            RuntimeSubversion = exeReader.ReadUInt16(); 
            ProductVersion = (Constants.Products)exeReader.ReadInt32();
            ProductBuild = exeReader.ReadInt32();//Easy Access
            Settings.Build=ProductBuild;
            
            Build = ProductBuild;

            GameChunks = new ChunkList(); //Reading game chunks
            GameChunks.Read(exeReader);

            //Load chunks into gamedata for easier access
            //Can only be accessed from here AFTER loading all the chunks
            //If you need it AT LOADING - use ChunkList.get_chunk<ChunkType>();
            if (GameChunks.GetChunk<AppName>() != null) Name = GameChunks.GetChunk<AppName>().Value;
            if (GameChunks.GetChunk<Copyright>() != null) Copyright = GameChunks.GetChunk<Copyright>().Value;
            if (GameChunks.GetChunk<AppAuthor>()!=null) Author = GameChunks.GetChunk<AppAuthor>().Value;
            if (GameChunks.GetChunk<EditorFilename>() != null) EditorFilename = GameChunks.GetChunk<EditorFilename>().Value;
            if (GameChunks.GetChunk<TargetFilename>() != null) TargetFilename = GameChunks.GetChunk<TargetFilename>().Value;
            if (GameChunks.GetChunk<AppMenu>() != null) Menu = GameChunks.GetChunk<AppMenu>();
            if (GameChunks.GetChunk<AppHeader>() != null) Header = GameChunks.GetChunk<AppHeader>();
            if (GameChunks.GetChunk<SoundBank>() != null) Sounds = GameChunks.GetChunk<SoundBank>();
            if (GameChunks.GetChunk<MusicBank>() != null) Music = GameChunks.GetChunk<MusicBank>();
            if (GameChunks.GetChunk<FontBank>() != null) Fonts = GameChunks.GetChunk<FontBank>();
            if (GameChunks.GetChunk<ImageBank>() != null) Images = GameChunks.GetChunk<ImageBank>();
            if (GameChunks.GetChunk<AppIcon>() != null) Icon = GameChunks.GetChunk<AppIcon>();
            if (GameChunks.GetChunk<GlobalStrings>() != null) GStrings = GameChunks.GetChunk<GlobalStrings>();
            if (GameChunks.GetChunk<GlobalValues>() != null) GValues = GameChunks.GetChunk<GlobalValues>();
            if (GameChunks.GetChunk<FrameItems>() != null) Frameitems = GameChunks.GetChunk<FrameItems>();
            Frames = GameChunks.Frames; //Its a list, so i have to manually parse them in chunk list. 

            //Print();
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
                    var objects = item.Chunks.GetChunk<ObjectInstances>();
                    if (objects != null)
                    {
                        foreach (var obj in objects.Items)
                        {
                            Logger.Log($"           Object: {obj.Name} - {obj.Handle} - {obj.FrameItem.ObjectType}", true, ConsoleColor.Green);
                        }
                    }

                }
                

            }






        }


    }
}
