using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CTFAK.MMFParser.EXE.Loaders.Objects;
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

        public ChunkList Chunks;
        private FrameHeader _header;
        private FrameName _name;
        private FramePassword _password;
        private FramePalette _palette;
        private Layers _layers;
        private ObjectInstances _objects;
        private Events.Events _events;
        private Transition _fadeIn;
        private Transition _fadeOut;
        private VirtualRect _virtualSize;
        private MovementTimerBase _movementTimer;


        public override void Print(bool ext)
        {
            Logger.Log($"Frame: {Name}", true, ConsoleColor.Green);
            Logger.Log($"   Password: {Password}", true, ConsoleColor.Green);
            Logger.Log($"   Size: {Width}x{Height}", true, ConsoleColor.Green);
            Logger.Log($"   Objects: {Objects.Count}", true, ConsoleColor.Green);
            Logger.Log($"-------------------------", true, ConsoleColor.Green);
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Name: {Name}",
                $"Size: {Width}x{Height}",
                $"Virtual Size: {VirtWidth}x{VirtHeight}",
                $"MVTimer: {MovementTimer}",
                $"Objects: {Objects.Count}",
                $"Layers: {Layers.Count}",
                $"Event Count: {Events.Items.Count}",
                $"{(FadeIn!=null ?$"Fade In: {FadeIn.Name}":"")}",
                $"{(FadeOut!=null ?$"Fade Out: {FadeOut.Name}":"")}",
            };
        }

        public override void Read()
        {
            var frameReader = new ByteReader(Chunk.ChunkData);
            Chunks = new ChunkList();
            Chunks.Read(frameReader);
            
            
            _header = Chunks.GetChunk<FrameHeader>();
            _virtualSize = Chunks.GetChunk<VirtualRect>();
            _name = Chunks.GetChunk<FrameName>();
            _password = Chunks.GetChunk<FramePassword>();
            _palette = Chunks.GetChunk<FramePalette>();
            _layers = Chunks.GetChunk<Layers>();
            //layerEffects
            _objects = Chunks.GetChunk<ObjectInstances>();
            _events = Chunks.GetChunk<Events.Events>();
            _movementTimer = Chunks.GetChunk<MovementTimerBase>();
            //frameEffects
            _fadeIn = Chunks.PopChunk<Transition>();
            _fadeOut = Chunks.PopChunk<Transition>();
            
            Flags.flag = _header.Flags.flag;
            Logger.Log(Properties.GlobalStrings.readingFrame+$" {Name}",true,ConsoleColor.Green);

        }

        public int Width => _header.Width;
        public int Height => _header.Height;
        public int VirtWidth => _virtualSize.Right;
        public int VirtHeight => _virtualSize.Bottom;
        public int MovementTimer => _movementTimer.Value;
        public string Name => _name?.Value ?? "UNK";
        public string Password => _password?.Value ?? "";
        public Color Background => _header.Background;
        public List<ObjectInstance> Objects => _objects?.Items ?? null;
        public List<Color> Palette => _palette?.Items ?? new Color[256].ToList();
        public Events.Events Events => _events;
        public Transition FadeIn => _fadeIn;
        public Transition FadeOut => _fadeOut;
        public List<Layer> Layers => _layers?.Items ?? null;
        

        public Frame(ByteReader reader) : base(reader){}
        public Frame(ChunkList.Chunk chunk) : base(chunk){}
        
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
            if (Settings.GameType == GameType.OnePointFive)
            {
                Width = Reader.ReadInt16();
                Height = Reader.ReadInt16();
                Background = Reader.ReadColor();
                Flags.flag = (uint) Reader.ReadInt16(); 
            }
            else
            {
                Width = Reader.ReadInt32();
                Height = Reader.ReadInt32();
                Background = Reader.ReadColor();
                Flags.flag = Reader.ReadUInt32(); 
            }
            
            
            

        }
    }

    public class ObjectInstances : ChunkLoader
    {
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
                $"Number of objects: {Items.Count}"
            };
        }

        public override void Read()
        {
            
            var count = Reader.ReadInt32();
            for (int i = 0; i < count; i++)
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
            if (Settings.GameType == GameType.OnePointFive)
            {
                X = Reader.ReadInt16();
                Y = Reader.ReadUInt16();
                ParentType = Reader.ReadInt16();
                ParentHandle = Reader.ReadInt16();
            }
            else
            {
                X = Reader.ReadInt32();
                Y = Reader.ReadInt32();
                ParentType = Reader.ReadInt16();
                ParentHandle = Reader.ReadInt16();
                Layer = Reader.ReadInt16();
                var reserved = Reader.ReadInt16();
            }
            
            
            //-------------------------

        }

        public ObjectInfo FrameItem
        {
            get
            {
                if (Program.CleanData.GameChunks.GetChunk<FrameItems>() == null) return null;
                return Program.CleanData.GameChunks.GetChunk<FrameItems>().FromHandle(ObjectInfo);
            }
        }

        public string Name => FrameItem.Name;

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

        public override string[] GetReadableData()=>new string[]{$"Layers: {Items.Count}" };
        
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
            Name = Reader.ReadUniversal();
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

        public override void Print(bool ext){}
        public override string[] GetReadableData() => new string[]{"Length: 256"};
    }
    public class VirtualRect:Rect
    {
        public VirtualRect(ByteReader reader) : base(reader){}
        public VirtualRect(ChunkList.Chunk chunk) : base(chunk){}
    }
    public class MovementTimerBase:ChunkLoader
    {
        public int Value;

        public MovementTimerBase(ByteReader reader) : base(reader)
        {
        }

        public MovementTimerBase(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Value = Reader.ReadInt32();
        }
        public override void Print(bool ext){}
        public override string[] GetReadableData()=>new string[]{"Value: "+Value};
        
    }
}