using System;
using System.Collections.Generic;
using System.Drawing;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders
{
    public class Frame : DataLoader
    {
        public string Name = "ERROR";
        public int SizeX;
        public int SizeY;
        public Color Background;
        public int MaxObjects;
        public List<FrameItem> Items=new List<FrameItem>();
        public int Handle;
        public int LastViewedX;
        public int LastViewedY;
        public List<ItemFolder> Folders=new List<ItemFolder>();
        public List<FrameInstance> Instances=new List<FrameInstance>();
        public uint Flags;
        public string Password;
        public List<Color> Palette;
        public int StampHandle;
        public int ActiveLayer;
        public List<Layer> Layers;
        public Events Events;
        public ChunkList Chunks;

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
            Writer.WriteUInt32(Flags);
            Writer.WriteInt32(MaxObjects);
            Writer.AutoWriteUnicode(Password);
            Writer.WriteInt32(0);
            Writer.WriteInt32(LastViewedX);
            Writer.WriteInt32(LastViewedY);
            Writer.WriteInt32(Palette.Count);
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
            
            //TODO: Do transitions
            Writer.WriteInt8(0);
            Writer.WriteInt8(0);

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

            if (Instances != null)
            {
                Writer.WriteInt32(Instances.Count);
                foreach (var item in Instances)
                {
                    item.Write(Writer);
                }
            }

            Events.Write(Writer);
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
            Console.WriteLine(Name);
            SizeX = Reader.ReadInt32();
            SizeY = Reader.ReadInt32();
            Background = Reader.ReadColor();
            Flags = Reader.ReadUInt32();
            MaxObjects = Reader.ReadInt32();
            Password = Helper.AutoReadUnicode(Reader);
            Reader.Skip(4);
            LastViewedX = Reader.ReadInt32();
            LastViewedY = Reader.ReadInt32();

            var paletteNum = Reader.ReadInt32();
            Palette = new List<Color>();
            for (int i = 0; i < paletteNum; i++)
            {
                Palette.Add(Reader.ReadColor());
            }
            StampHandle = Reader.ReadInt32();
            ActiveLayer = Reader.ReadInt32();
            var layersCount = Reader.ReadInt32();
            Layers = new List<Layer>();
            for (int i = 0; i < layersCount; i++)
            {
                var layer = new Layer(Reader);
                layer.Read();
                Layers.Add(layer);

            }
            //fadein

            //fadeout
            Reader.Skip(2);
            Items = new List<FrameItem>();
            var frameitemsCount = Reader.ReadInt32();
            for (int i = 0; i < frameitemsCount; i++)
            {
                var frameitem = new FrameItem(Reader);
                frameitem.Read();
                Items.Add(frameitem);
                Console.WriteLine("Frameitem:"+frameitem.Name);
            }
            
            Folders = new List<ItemFolder>();
            var folderCount = Reader.ReadInt32();
            for (int i = 0; i < frameitemsCount; i++)
            {
                var folder = new ItemFolder(Reader);
                folder.Read();
                Folders.Add(folder);
            }
            
            Instances = new List<FrameInstance>();
            var instancesCount = Reader.ReadInt32();
            for (int i = 0; i < instancesCount; i++)
            {
                var inst = new FrameInstance(Reader);
                inst.Read();
                Instances.Add(inst);
            }
            Events = new Events(Reader);
            Console.WriteLine("BeforeEventsPos: "+Reader.Tell());
            Events.Read();
            Console.WriteLine("AfterEventsPos: "+Reader.Tell());
            Chunks = new ChunkList(Reader);
            Chunks.Read();
            MFA.emptyEvents = Events;
            MFA.emptyFrameChunks = Chunks;






        }

        
    }
    
    


}





