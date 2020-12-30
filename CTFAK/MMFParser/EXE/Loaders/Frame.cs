using System;
using System.Collections.Generic;
using System.Drawing;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    class FrameName : StringChunk
    {
        public FrameName(ByteReader reader) : base(reader)
        {
        }

        public FrameName(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

    class FramePassword : StringChunk
    {
        public FramePassword(ByteReader reader) : base(reader)
        {
        }

        public FramePassword(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

    public class Frame : ChunkLoader
    {
        public string Name;
        public string Password;
        public int Width;
        public int Height;
        public Color Background;
        public BitDict Flags=new BitDict(new string[]
        {
           "XCoefficient",
           "YCoefficient",
           "DoNotSaveBackground",
           "Wrap",
           "Visible",
           "WrapHorizontally",
           "WrapVertically",
           "","","","","","","","","",
           "Redraw",
           "ToHide",
           "ToShow"
        });
        
        public int CountOfObjs;
        // int _top;
        // int _bottom;
        // int _left;
        // int _right;
        public ChunkList Chunks;
        public FrameHeader Header;
        public ObjectInstances Objects;
        public Layers Layers;
        public Events.Events Events;
        public FramePalette Palette;
        public Transition FadeIn;
        public Transition FadeOut;


        public override void Print(bool ext)
        {
            Logger.Log($"Frame: {Name}", true, ConsoleColor.Green);
            Logger.Log($"   Password: {(Password!=null ? Password : "None")}", true, ConsoleColor.Green);
            Logger.Log($"   Size: {Width}x{Height}", true, ConsoleColor.Green);
            Logger.Log($"   Objects: {CountOfObjs}", true, ConsoleColor.Green);
            Logger.Log($"-------------------------", true, ConsoleColor.Green);
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Name: {Name}",
                $"Size: {Width}x{Height}",
                $"Objects: {CountOfObjs}"
            
            };
        }

        public override void Read()
        {
            
            var frameReader = new ByteReader(Chunk.ChunkData);
            Chunks = new ChunkList();
            
            
            Chunks.Read(frameReader);
            var name = Chunks.GetChunk<FrameName>();
            if (name != null) //Just to be sure
            {
                this.Name = name.Value;
                Logger.Log("Reading Frame: "+Name,true,ConsoleColor.Green);
            }
            var password = Chunks.GetChunk<FramePassword>();
            if (password != null) //Just to be sure
            {
                this.Password = password.Value;
            }

            var layers = Chunks.GetChunk<Layers>();
            if (layers != null)
            {
                Layers = layers;
            }
            var events = Chunks.GetChunk<Events.Events>();
            if (events != null)
            {
                Events = events;
            }
            var palette = Chunks.GetChunk<FramePalette>();
            if (palette != null)
            {
                Palette = palette;
            }
            Header = Chunks.GetChunk<FrameHeader>();
            Width = Header.Width;
            Height = Header.Height;
            Background = Header.Background;
            Flags.flag = Header.Flags.flag;
            Objects = Chunks.GetChunk<ObjectInstances>();
            if(Objects!=null)
            {
                CountOfObjs = Objects.CountOfObjects;              
            }

            FadeIn = Chunks.PopChunk<Transition>();
            FadeOut = Chunks.PopChunk<Transition>();







            foreach (var item in Chunks.Chunks)
            {
                //Directory.CreateDirectory($"{Program.DumpPath}\\CHUNKS\\FRAMES\\{this.name}");
                //string path = $"{Program.DumpPath}\\CHUNKS\\FRAMES\\{this.name}\\{chunk.name}.chunk";
                //File.WriteAllBytes(path, item.chunk_data);

            }
            
            


        }

        public Frame(ByteReader reader) : base(reader)
        {
        }

        public Frame(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

    public class FrameHeader : ChunkLoader
    {
        public int Width;
        public int Height;
        public BitDict Flags = new BitDict(new string[]
        {
            "XCoefficient",
            "YCoefficient",
            "DoNotSaveBackground",
            "Wrap",
            "Visible",
            "WrapHorizontally",
            "WrapVertically","","","","","","","","","",
            "Redraw",
            "ToHide",
            "ToShow"
                
        });
        public Color Background;
        public FrameHeader(ByteReader reader) : base(reader)
        {
        }

        public FrameHeader(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {
            
        }

        public override string[] GetReadableData()
        {
        return new string[]
            {
              $"Size: {Width}x{Height}",
              $"Flags:;{Flags.ToString()}"
 
            };
        }

        public override void Read()
        {
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            Background = Reader.ReadColor();
            Flags.flag = Reader.ReadUInt32();
            
            

        }
    }

    public class ObjectInstances : ChunkLoader
    {
        
        public int CountOfObjects=0;
        public List<ObjectInstance> Items = new List<ObjectInstance>();

        public ObjectInstances(ByteReader reader) : base(reader)
        {
        }

        public ObjectInstances(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Print(bool ext)
        {

        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Number of objects: {CountOfObjects}"
            };
        }

        public override void Read()
        {
            
            CountOfObjects = Reader.ReadInt32();
            for (int i = 0; i < CountOfObjects; i++)
            {
                var item = new ObjectInstance(Reader);
                item.Read();
                Items.Add(item);
            }
            Reader.Skip(4);
        }
    }

    public class ObjectInstance : ChunkLoader
    {
        public ushort Handle;
        public ushort ObjectInfo;
        public int X;
        public int Y;
        public short ParentType;
        public short Layer;
        public string Name;
        public short ParentHandle;

        public ObjectInstance(ByteReader reader) : base(reader)
        {
        }

        public ObjectInstance(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Handle = (ushort) Reader.ReadInt16();
            ObjectInfo = (ushort) Reader.ReadInt16();
            X = Reader.ReadInt32();
            Y = Reader.ReadInt32();
            ParentType = Reader.ReadInt16();
            ParentHandle = Reader.ReadInt16();
            Layer = Reader.ReadInt16();
            var reserved = Reader.ReadInt16();
            
            //-------------------------
            if (FrameItem != null) Name = FrameItem.Name;
            else Name = $"UNKNOWN-{Handle}";

        }

        public ObjectInfo FrameItem
        {
            get
            {
                if (Exe.Instance.GameData.GameChunks.GetChunk<FrameItems>() == null) return null;
                return Exe.Instance.GameData.GameChunks.GetChunk<FrameItems>().FromHandle(ObjectInfo);
            }
        } 

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Name: {Name}",
                $"Type:{(Constants.ObjectType)FrameItem.ObjectType} - {FrameItem.ObjectType}",
                $"Position: {X,5}x{Y,5}",
                $"Size: NotImplementedYet"

            };
        }
    }

    public class Layers : ChunkLoader
    {
        public List<Layer> Items;

        public Layers(ByteReader reader) : base(reader)
        {
        }

        public Layers(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Items = new List<Layer>();
            var count = Reader.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                Layer item = new Layer(Reader);
                item.Read();
                Items.Add(item);
            }

        }

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }
    }

    public class Layer : ChunkLoader
    {
        public string Name;
        public BitDict Flags = new BitDict(new string[]
        {
            "XCoefficient",
            "YCoefficient",
            "DoNotSaveBackground",
            "",
            "Visible",
            "WrapHorizontally",
            "WrapVertically",
            "", "", "", "",
            "", "", "", "", "",
            "Redraw",
            "ToHide",
            "ToShow"
        }

        );
        public float XCoeff;
        public float YCoeff;
        public int NumberOfBackgrounds;
        public int BackgroudIndex;
        

        public Layer(ByteReader reader) : base(reader)
        {
        }

        public Layer(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Flags.flag = Reader.ReadUInt32();
            XCoeff = Reader.ReadSingle();
            YCoeff = Reader.ReadSingle();
            NumberOfBackgrounds = Reader.ReadInt32();
            BackgroudIndex = Reader.ReadInt32();
            Name = Reader.ReadWideString();
        }

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }
    }

    public class FramePalette : ChunkLoader
    {
        public List<Color> Items;

        public FramePalette(ByteReader reader) : base(reader)
        {
        }

        public FramePalette(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Items = new List<Color>();
            for (int i = 0; i < 257; i++)
            {
                Items.Add(Reader.ReadColor());
            }
        }

        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }
    }


}