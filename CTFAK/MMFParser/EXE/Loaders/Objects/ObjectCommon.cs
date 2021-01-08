using System;
using System.Drawing;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Objects
{
    public class ObjectCommon : ChunkLoader
    {
        private ushort _valuesOffset;
        private ushort _stringsOffset;
        private byte[] Identifier;
        private ushort _fadeinOffset;
        private ushort _fadeoutOffset;
        private ushort _movementsOffset;
        private ushort _animationsOffset;
        private ushort _systemObjectOffset;
        public ushort _counterOffset;
        public ushort _extensionOffset;
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

        public BitDict Flags = new BitDict(new string[]
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

        public BitDict NewFlags = new BitDict(new string[]
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
        public AlterableValues Values;
        public AlterableStrings Strings;
        public Movements Movements;


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
            //if(Parent.ObjectType!=2)return;
            var currentPosition = Reader.Tell();
            var size = Reader.ReadInt32();
            if (Settings.Build >= 284)
            {
                _animationsOffset = Reader.ReadUInt16();
                _movementsOffset = Reader.ReadUInt16();
                var version = Reader.ReadUInt16();
                Reader.Skip(2); //TODO: Find out
                _counterOffset = Reader.ReadUInt16();
                _systemObjectOffset = Reader.ReadUInt16();
            }
            else
            {
                _movementsOffset = Reader.ReadUInt16();
                _animationsOffset = Reader.ReadUInt16();
                var version = Reader.ReadUInt16();
                _counterOffset = Reader.ReadUInt16();
                _systemObjectOffset = Reader.ReadUInt16();  
            }

            Flags.flag = Reader.ReadUInt32();
            var end = Reader.Tell() + 16;
            //Ignoring qualifiers
            Reader.Seek(end);
            if (Settings.Build == 284)
            {
                _systemObjectOffset = Reader.ReadUInt16();
            }
            else
            {
                _extensionOffset = Reader.PeekUInt16();
            }

            _valuesOffset = Reader.ReadUInt16();
            _stringsOffset = Reader.ReadUInt16();
            NewFlags.flag = Reader.ReadUInt32();
            preferences.flag = Reader.ReadUInt32();
            Identifier = Reader.ReadBytes(4);
            BackColor = Reader.ReadColor();
            _fadeinOffset = (ushort) Reader.ReadUInt32();
            _fadeoutOffset = (ushort) Reader.ReadUInt32();
            
            if (_movementsOffset != 0)
            {
                Reader.Seek(currentPosition+_movementsOffset);
                Movements = new Movements(Reader);
                Movements.Read();
                //Console.WriteLine("Movements done");
            }
            
            
            if (_valuesOffset != 0)
            {
                Reader.Seek(currentPosition + _valuesOffset);
                Values = new AlterableValues(Reader);
                Values.Read();
                // Console.WriteLine("Values done");
            }
            
            
            if (_stringsOffset != 0)
            {
                Reader.Seek(currentPosition + _stringsOffset);
                Strings = new AlterableStrings(Reader);
                Strings.Read();
                // Console.WriteLine("Strings done");
            }
            
            
            if (_animationsOffset != 0&& Parent.ObjectType==2)
            {
                Reader.Seek(currentPosition + _animationsOffset);
                Animations = new Animations(Reader);
                Animations.Read();
                // Console.WriteLine("Animations done");
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

            if (_systemObjectOffset != 0)
            {
                Logger.Log("Reading System Object: "+_systemObjectOffset);
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
                    Logger.Log("Counter: "+Parent.Name);
                    Counters = new Counters(Reader);
                    Counters.Read();
                }
                
            }
            // Logger.Log("anims: "+_animationsOffset);
            // Logger.Log("fadeIn: "+_fadeinOffset);
            // Logger.Log("fadeOut: "+_fadeoutOffset);
            // Logger.Log("movements: "+_movementsOffset);
            // Logger.Log("strings: "+_stringsOffset);
            // Logger.Log("values: "+_valuesOffset);
            // Logger.Log("sysObj: "+_systemObjectOffset);

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