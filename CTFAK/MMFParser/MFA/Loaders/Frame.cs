using System;
using System.Collections.Generic;
using System.Drawing;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders
{
    public class Frame : DataLoader
    {
        public string Name = "ERROR";
        public int SizeX;
        public int SizeY;
        public Color Background;
        public int MaxObjects;
        public List<FrameItem> Items = new List<FrameItem>();
        public int Handle;
        public int LastViewedX;
        public int LastViewedY;
        public List<ItemFolder> Folders = new List<ItemFolder>();
        public List<FrameInstance> Instances = new List<FrameInstance>();
        public BitDict Flags = new BitDict(new string[]
        {
            "GrabDesktop",
            "KeepDisplay",
            "BackgroundCollisions",
            "DisplayFrameTitle",
            "ResizeToScreen",
            "ForceLoadOnCall",
            "NoDisplaySurface",
            "ScreenSaverSetup",
            "TimerBasedMovements",
            "MochiAds",
            "NoGlobalEvents"
        });

        public string Password="";
        public string UnkString="";
        public List<Color> Palette;
        public int StampHandle;
        public int ActiveLayer;
        public List<Layer> Layers = new List<Layer>();
        public Events Events;
        public ChunkList Chunks;
        public Transition FadeIn;
        public Transition FadeOut;
        public int PaletteSize;

        public Frame(ByteReader reader) : base(reader)
        {
        }


        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Handle);
            Writer.AutoWriteUnicode(Name);
            Writer.WriteInt32(SizeX);
            Writer.WriteInt32(SizeY);
            Writer.WriteColor(Background);
            Writer.WriteUInt32(Flags.flag);
            Writer.WriteInt32(MaxObjects);
            Writer.AutoWriteUnicode(Password);
            Writer.AutoWriteUnicode(UnkString);
            Writer.WriteInt32(LastViewedX);
            Writer.WriteInt32(LastViewedY);
            Writer.WriteInt32(Palette.Count);//WTF HELP 
            
            foreach (var item in Palette)
            {
                Writer.WriteColor(item);
            }
            
            Writer.WriteInt32(StampHandle);
            Writer.WriteInt32(ActiveLayer);
            Writer.WriteInt32(Layers.Count);
            foreach (var layer in Layers)
            {
                layer.Write(Writer);
            }
            
            if (FadeIn != null)
            {
                Writer.WriteInt8(1);
                FadeIn.Write(Writer);
            }
            else Writer.Skip(1);

            if (FadeOut != null)
            {
                Writer.WriteInt8(1);
                FadeOut.Write(Writer);
            }
            else Writer.Skip(1);
            
            
            Writer.WriteInt32(Items.Count);
            foreach (var item in Items)
            {
                item.Write(Writer);
            }
         
            Writer.WriteInt32(Folders.Count);
            foreach (var item in Folders)
            {
                item.Write(Writer);
            }
            
            Writer.WriteInt32(Instances.Count);
            foreach (var item in Instances)
            {
                item.Write(Writer);
            }
            

            Events.Write(Writer);

            for (int i = 0; i < Layers.Count-1; i++)
            {
                Writer.WriteColor(Layers[i].RGBCoeff);
                Writer.WriteInt32(Layers[i].Unk1);
                Writer.WriteInt32(Layers[i].Unk2);
            }
            
            Chunks.Write(Writer);
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Handle = Reader.ReadInt32();
            Name = Helper.AutoReadUnicode(Reader);
            SizeX = Reader.ReadInt32();
            SizeY = Reader.ReadInt32();
            Background = Reader.ReadColor();
            Flags.flag = Reader.ReadUInt32();

            MaxObjects = Reader.ReadInt32();
            Password = Helper.AutoReadUnicode(Reader);
            UnkString = Helper.AutoReadUnicode(Reader);

            LastViewedX = Reader.ReadInt32();
            LastViewedY = Reader.ReadInt32();

            PaletteSize = Reader.ReadInt32();
            Palette = new List<Color>();
            for (int i = 0; i < 256; i++)
            {
                Palette.Add(Reader.ReadColor());
            }

            StampHandle = Reader.ReadInt32();
            ActiveLayer = Reader.ReadInt32();
            int layersCount = Reader.ReadInt32();
            Logger.Log("Layers: "+layersCount);
            for (int i = 0; i < layersCount; i++)
            {
                var layer = new Layer(Reader);
                layer.Read();
                Layers.Add(layer);
            }

            if (Reader.ReadByte() == 1)
            {
                FadeIn = new Transition(Reader);
                FadeIn.Read();
            }

            if (Reader.ReadByte() == 1)
            {
                FadeOut = new Transition(Reader);
                FadeOut.Read();
            }
            var frameItemsCount = Reader.ReadInt32();
            for (int i = 0; i < frameItemsCount; i++)
            {
                var frameitem = new FrameItem(Reader);
                frameitem.Read();
                
                Items.Add(frameitem);
            }
            var folderCount = Reader.ReadInt32();
            for (int i = 0; i < folderCount; i++)
            {
                var folder = new ItemFolder(Reader);
                folder.Read();
                Folders.Add(folder);
            }
            var instancesCount = Reader.ReadInt32();
            for (int i = 0; i < instancesCount; i++)
            {
                var inst = new FrameInstance(Reader);
                inst.Read();
                Instances.Add(inst);
            }

            // var unkCount = Reader.ReadInt32();
            // for (int i = 0; i < unkCount; i++)
            // {
            // UnkBlocks.Add(Reader.ReadBytes(32));
            // }

            Events = new Events(Reader);
            Events.Read();
            for (int i=0;i<Layers.Count-1;i++)
            {
                Layers[i].RGBCoeff= Reader.ReadColor();
                Layers[i].Unk1=Reader.ReadInt32();
                Layers[i].Unk2=Reader.ReadInt32();
            } 
            
            Chunks = new ChunkList(Reader);
            Chunks.Read();
            if(Events.Items.Count==0)MFA.emptyEvents = Events;
             
            MFA.emptyFrameChunks = Chunks;
        }
    }
}