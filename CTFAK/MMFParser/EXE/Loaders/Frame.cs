﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CTFAK.MMFParser.Attributes;
using CTFAK.MMFParser.EXE.Loaders.Objects;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    class FrameName : StringChunk
    {
        public FrameName(ByteReader reader) : base(reader)
        {
        }

        
    }

    class FramePassword : StringChunk
    {
        public FramePassword(ByteReader reader) : base(reader)
        {
        }

        
    }
    [SubChunkList(nameof(Frame.Chunks))]
    [CustomVisualName(nameof(Frame.Name),true)]
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


        public override void Write(ByteWriter Writer)
        {
            Chunks.Write(Writer);
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
            var frameReader = new ByteReader(Reader.ReadBytes());
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
            
            Flags.flag = _header?.Flags?.flag??0;
            Logger.Log(Properties.GlobalStrings.readingFrame+$" {_name.Value}",true,ConsoleColor.Green);

            Width = _header.Width;
            Height = _header.Height;
            VirtWidth = _virtualSize?.Right??0;
            VirtHeight = _virtualSize?.Bottom??0;
            MovementTimer = _movementTimer?.Value??0;
            Name = _name?.Value;
            Password = _password?.Value;
            Background = _header.Background;
            Objects = _objects?.Items;
            Palette = _palette?.Items;
            Events = _events;
            FadeIn = _fadeIn;
            FadeOut = _fadeOut;
            Layers = _layers?.Items;


        }

        public int Width { get; set; }
        public int Height { get; set; }
        public int VirtWidth { get; set; }
        public int VirtHeight { get; set; }
        public int MovementTimer { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Color Background { get; set; }
        public List<ObjectInstance> Objects { get; set; }
        public List<Color> Palette { get; set; }
        public Events.Events Events { get; set; }
        public Transition FadeIn { get; set; }
        public Transition FadeOut { get; set; }
        public List<Layer> Layers { get; set; }
        

        public Frame(ByteReader reader) : base(reader){}
        
        
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

       

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Width);
            Writer.WriteInt32(Height);
            Writer.WriteColor(Background);
            Writer.WriteInt32((int) Flags.flag);
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
                if (Reader.Tell() > Reader.Size() - 10)
                {
                    Console.WriteLine("E202: Ran out of bytes reading Frame (" + Reader.Tell() + "/" + Reader.Size() + ")");
                    return; //really hacky shit, but it works
                }
                Width = Reader.ReadInt32();
                Height = Reader.ReadInt32();
                Background = Reader.ReadColor();
                Flags.flag = Reader.ReadUInt32(); 
            }
            
            
            

        }
    }
    [SubList(nameof(ObjectInstances.Items))]
    public class ObjectInstances : ChunkLoader
    {
        public List<ObjectInstance> Items = new List<ObjectInstance>();

        public ObjectInstances(ByteReader reader) : base(reader)
        {
        }

        

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
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
            if (Reader.Size() < 4)
            {
                Console.WriteLine("E244: Ran out of bytes reading Frame (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
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
                var res = Reader.ReadInt16();
            }
           
            
            //-------------------------

        }

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }

        public ObjectInfo FrameItem
        {
            get
            {
                if (Program.CleanData.Frameitems == null) return null;
                return Program.CleanData.Frameitems.FromHandle(ObjectInfo);
            }
        }

        public string Name => FrameItem.Name;


        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Name: {Name}",
                $"Type:{FrameItem?.ObjectType ?? 0}",
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

       

        public override void Read()
        {
            Items = new List<Layer>();
            if (Reader.Size() < 4)
            {
                Console.WriteLine("E345: Ran out of bytes reading Frame (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
            var count = Reader.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                Layer item = new Layer(Reader);
                item.Read();
                Items.Add(item);
            }

        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Items.Count);
            foreach (Layer layer in Items)
            {
                layer.Write(Writer);
            }
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

       

        public override void Read()
        {
            Flags.flag = Reader.ReadUInt32();
            XCoeff = Reader.ReadSingle();
            YCoeff = Reader.ReadSingle();
            NumberOfBackgrounds = Reader.ReadInt32();
            BackgroudIndex = Reader.ReadInt32();
            Name = Reader.ReadUniversal();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32((int) Flags.flag);
            Writer.WriteSingle(XCoeff);
            Writer.WriteSingle(YCoeff);
            Writer.WriteInt32(NumberOfBackgrounds);
            Writer.WriteInt32(BackgroudIndex);
            Writer.WriteUniversal(Name);
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

        

        public override void Read()
        {
            if (Reader.Size() < 4)
            {
                Console.WriteLine("E445: Ran out of bytes reading Frame (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
            Items = new List<Color>();
            for (int i = 0; i < 257; i++)
            {
                Items.Add(Reader.ReadColor());
            }
        }

        public override void Write(ByteWriter Writer)
        {
            foreach (Color item in Items)
            {
                Writer.WriteColor(item);
            }
        }

        public override string[] GetReadableData() => new string[]{"Length: 256"};
    }
    public class VirtualRect:Rect
    {
        public VirtualRect(ByteReader reader) : base(reader){}
    }
    public class MovementTimerBase:ChunkLoader
    {
        public int Value;

        public MovementTimerBase(ByteReader reader) : base(reader)
        {
        }

   

        public override void Read()
        {
            Value = Reader.ReadInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Value);
        }

        public override string[] GetReadableData()=>new string[]{"Value: "+Value};
        
    }
}