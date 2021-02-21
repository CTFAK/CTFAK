using System;
using System.Collections.Generic;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE
{
    public class GameData
    {
        public short RuntimeVersion;
        public short RuntimeSubversion;
        public int ProductBuild;
        public Constants.Products ProductVersion;
        public int Build;
        public ChunkList GameChunks;

        public AppName Name;
        public AppAuthor Author;
        public Copyright Copyright;
        public string AboutText;
        public string Doc;

        public EditorFilename EditorFilename;
        public TargetFilename TargetFilename;

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
        public FrameHandles FrameHandles;
        public Extensions Extensions;

        public void Write(ByteWriter Writer)
        {
            Writer.WriteAscii("PAMU");
            Writer.WriteInt16(RuntimeVersion);
            Writer.WriteInt16(RuntimeSubversion);
            Writer.WriteInt32((int) ProductVersion);
            Writer.WriteInt32(ProductBuild);
            var newChunks = new ChunkList();
            newChunks.Chunks = GameChunks.Chunks;
            newChunks.Write(Writer);

        }
        public void Read(ByteReader exeReader)
        {
            string magic = exeReader.ReadAscii(4); //Reading header
            Logger.Log("MAGIC HEADER: "+magic);
            //Checking for header
            if (magic == Constants.UnicodeGameHeader) Settings.Unicode = true;//PAMU
            else if (magic == Constants.GameHeader) Settings.Unicode = false;//PAME
            else Logger.Log("Couldn't found any known headers", true, ConsoleColor.Red);//Header not found
            RuntimeVersion = (short) exeReader.ReadUInt16(); 
            RuntimeSubversion = (short) exeReader.ReadUInt16(); 
            ProductVersion = (Constants.Products)exeReader.ReadInt32();
            ProductBuild = exeReader.ReadInt32();
            Settings.Build=ProductBuild;//Easy Access
            Logger.Log("GAME BUILD: "+Settings.Build);
            Logger.Log("PRODUCT: "+ProductVersion);
            
            Build = ProductBuild;

            GameChunks = new ChunkList(); //Reading game chunks
            GameChunks.Read(exeReader);

            //Load chunks into gamedata for easier access
            //Can only be accessed from here AFTER loading all the chunks
            //If you need it AT LOADING - use ChunkList.GetChunk<ChunkType>();
            if (GameChunks.GetChunk<AppName>() != null) Name = GameChunks.PopChunk<AppName>();
            if (GameChunks.GetChunk<Copyright>() != null) Copyright = GameChunks.PopChunk<Copyright>();
            if (GameChunks.GetChunk<AppAuthor>()!=null) Author = GameChunks.PopChunk<AppAuthor>();
            if (GameChunks.GetChunk<EditorFilename>() != null) EditorFilename = GameChunks.PopChunk<EditorFilename>();
            if (GameChunks.GetChunk<TargetFilename>() != null) TargetFilename = GameChunks.PopChunk<TargetFilename>();
            if (GameChunks.GetChunk<AppMenu>() != null) Menu = GameChunks.PopChunk<AppMenu>();
            if (GameChunks.GetChunk<AppHeader>() != null) Header = GameChunks.PopChunk<AppHeader>();
            if (GameChunks.GetChunk<SoundBank>() != null) Sounds = GameChunks.PopChunk<SoundBank>();
            if (GameChunks.GetChunk<MusicBank>() != null) Music = GameChunks.PopChunk<MusicBank>();
            if (GameChunks.GetChunk<FontBank>() != null) Fonts = GameChunks.PopChunk<FontBank>();
            if (GameChunks.GetChunk<ImageBank>() != null) Images = GameChunks.PopChunk<ImageBank>();
            if (GameChunks.GetChunk<AppIcon>() != null) Icon = GameChunks.PopChunk<AppIcon>();
            if (GameChunks.GetChunk<GlobalStrings>() != null) GStrings = GameChunks.PopChunk<GlobalStrings>();
            if (GameChunks.GetChunk<GlobalValues>() != null) GValues = GameChunks.PopChunk<GlobalValues>();
            if (GameChunks.GetChunk<FrameHandles>() != null) FrameHandles = GameChunks.PopChunk<FrameHandles>();
            if (GameChunks.GetChunk<Extensions>() != null) Extensions = GameChunks.PopChunk<Extensions>();
            if (GameChunks.GetChunk<FrameItems>() != null) Frameitems = GameChunks.PopChunk<FrameItems>();
            else Frameitems=new FrameItems(null as ByteReader);
            
            for (int i = 0; i < Header.NumberOfFrames; i++)
            {
                Frames.Add(GameChunks.PopChunk<Frame>());
            }

            if (Settings.GameType == GameType.TwoFivePlus)
            {
                var headers = GameChunks.PopChunk<ObjectHeaders>().Headers;
                var names = GameChunks.PopChunk<ObjectNames>().Names;
                if(headers.Count>names.Count)Logger.LogWarning("Warning: Some object names for 2.5+ are missing");
                if(headers.Count<names.Count)Logger.LogWarning("Warning: Some object headers for 2.5+ are missing");
                
                Program.CleanData.Frameitems = new FrameItems((ByteReader) null);
                foreach (KeyValuePair<int, ObjectHeader> header in headers)
                {
                    var newInfo = new ObjectInfo((ByteReader) null);
                    newInfo.Handle = header.Value.Handle;
                    newInfo.ObjectType = (Constants.ObjectType) header.Value.ObjectType;
                    newInfo.InkEffect = (int) header.Value.InkEffect;
                    newInfo.InkEffectValue = header.Value.InkEffectParameter;
                    string name = $"{newInfo.ObjectType}-{newInfo.Handle}";
                    names.TryGetValue(header.Key, out name);
                    newInfo.Name = name;
                    Frameitems.ItemDict.Add(newInfo.Handle, newInfo);
                }
            }
            
        }
        public void Print()
        {
            Logger.Log($"GameData Info:", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Runtime Version: {RuntimeVersion}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Runtime Subversion: { RuntimeSubversion}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Product Version: { ((Constants.Products)ProductVersion).ToString()}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    Product Build: {ProductBuild}", true, ConsoleColor.DarkGreen);
            Logger.Log($"    {(Settings.Unicode ? "Unicode" : "NonUnicode")} Game", true, ConsoleColor.DarkGreen);
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
                    Logger.Log($"       Frame: {item.Name,25}, Size: {item.Width,4}x{item.Height,4}, Number of objects: {item.Objects.Count,5}", true, ConsoleColor.Cyan);
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
