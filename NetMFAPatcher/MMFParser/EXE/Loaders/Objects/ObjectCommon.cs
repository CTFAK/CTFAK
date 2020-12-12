using System;
using System.Drawing;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders.Objects
{
    public class ObjectCommon : ChunkLoader
    {
        private short _valuesOffset;
        private short _stringsOffset;
        private byte[] Identifier;
        private int _fadeinOffset;
        private int _fadeoutOffset;
        private ushort _movementsOffset;
        private short _animationsOffset;
        private short _systemObjectOffset;
        private short _counterOffset;
        private short _extensionOffset;
        public Animations Animations;
        private long _end;

        private BitDict preferences = new BitDict(new string[]
            {
                "Backsave",
                "ScrollingIndependant",
                "QuickDisplay",
                "Sleep",
                "LoadOnCall",
                "Global",
                "BackEffects",
                "Kill",
                "InkEffects",
                "Transitions",
                "FineCollisions",
                "AppletProblems"
            }
        );

        private BitDict flags = new BitDict(new string[]
            {
                "DisplayInFront",
                "Background",
                "Backsave",
                "RunBeforeFadeIn",
                "Movements",
                "Animations",
                "TabStop",
                "WindowProc",
                "Values",
                "Sprites",
                "InternalBacksave",
                "ScrollingIndependant",
                "QuickDisplay",
                "NeverKill",
                "NeverSleep",
                "ManualSleep",
                "Text",
                "DoNotCreateAtStart",
                "FakeSprite",
                "FakeCollisions"
            }
        );

        private BitDict new_flags = new BitDict(new string[]
            {
                "DoNotSaveBackground",
                "SolidBackground",
                "CollisionBox",
                "VisibleAtStart",
                "ObstacleSolid",
                "ObstaclePlatform",
                "AutomaticRotation"
            }
        );

        public Color BackColor;
        public ObjectInfo Parent;
        public Counters Counters;
        public byte[] ExtensionData;
        public int ExtensionPrivate;
        public int ExtensionId;
        public int ExtensionVersion;


        public ObjectCommon(ByteReader reader) : base(reader)
        {
        }
        public ObjectCommon(ByteReader reader,ObjectInfo parent) : base(reader)
        {
            Parent = parent;
        }
        public ObjectCommon(ChunkList.Chunk chunk) : base(chunk)
        {
        }
        

        public override void Read()
        {
            if (Parent.ObjectType != 2) return;
            long currentPosition = Reader.Tell();
            int size = Reader.ReadInt32();
            bool newobj = Settings.Build >= 284;
            if (newobj)
            {
                _animationsOffset = (short) Reader.ReadUInt16();
                _movementsOffset = Reader.ReadUInt16();
            }
            else
            {
                _movementsOffset = Reader.ReadUInt16();
                _animationsOffset = Reader.ReadInt16();
            }
            short version = Reader.ReadInt16();
            _counterOffset = (short) Reader.ReadUInt16();
            _systemObjectOffset = (short) Reader.ReadUInt16();

            flags.flag = Reader.ReadUInt32();
            var end = Reader.Tell() + 16;
            Reader.Seek(end);
            if (newobj)
            {
                _systemObjectOffset = Reader.ReadInt16();
    
            }
            else
            {
                _extensionOffset = Reader.ReadInt16();
            }
            
            _valuesOffset = Reader.ReadInt16();
            _stringsOffset = Reader.ReadInt16();
            new_flags.flag = Reader.ReadUInt16();
            preferences.flag = Reader.ReadUInt16();
            byte[] identifier = Reader.ReadFourCc();
            BackColor = Reader.ReadColor();
            _fadeinOffset = Reader.ReadInt32();
            _fadeoutOffset = Reader.ReadInt32();

            if (_movementsOffset != 0)
            {
                //Reader.Seek(currentPosition+_movementsOffset);
                //var movements = new Movements(Reader);
                //movements.Read();
                Console.WriteLine("Movements done");
            }
            
            
            if (_valuesOffset != 0)
            {
                Reader.Seek(currentPosition + _valuesOffset);
                AlterableValues values = new AlterableValues(Reader);
                values.Read();
                Console.WriteLine("Values done");
            }
            
            
            if (_stringsOffset != 0)
            {
                Reader.Seek(currentPosition + _stringsOffset);
                AlterableStrings strings = new AlterableStrings(Reader);
                strings.Read();
                Console.WriteLine("Strings done");
            }
            
            
            if (_animationsOffset != 0&& Parent.ObjectType==2)
            {
                Reader.Seek(currentPosition + _animationsOffset);
                Animations = new Animations(Reader);
                Animations.Read();
                Console.WriteLine("Animations done");
            }

            /*if (_counterOffset != 0)
            {
                Reader.Seek(currentPosition + _counterOffset);
                var counter = new Counter(Reader);
                counter.Read();
                Console.WriteLine("Counters done");
            }

            if (_extensionOffset != 0)
            {
                var dataSize = Reader.ReadInt32() - 20;
                ExtensionVersion = Reader.ReadInt32();
                ExtensionId = Reader.ReadInt32();
                ExtensionPrivate = Reader.ReadInt32();
                if (dataSize != 0)
                    ExtensionData = Reader.ReadBytes(dataSize);
                Console.WriteLine("Extensions Done");

            }*/

            if (_systemObjectOffset > 0)
            {
                Console.WriteLine("Reading System Object");
                Reader.Seek(currentPosition+_systemObjectOffset);
                if (Parent.ObjectType == ((int) Constants.ObjectType.Text) ||
                    Parent.ObjectType == ((int) Constants.ObjectType.Question))
                {
                    //TODO; Text.read();
                }
                else if (Parent.ObjectType == ((int) Constants.ObjectType.Score) ||
                    Parent.ObjectType == ((int) Constants.ObjectType.Lives)||
                    Parent.ObjectType == ((int) Constants.ObjectType.Counter))
                {
                    Counters = new Counters(Reader);
                    Counters.Read();
                }
                
            }
            
            
            Console.WriteLine("SysObjOff: " + _systemObjectOffset);
            Console.WriteLine("ExtOff: " + _extensionOffset);
        }

        public override void Print(bool ext)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
}