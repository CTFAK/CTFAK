using System;
using System.Drawing;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Objects
{
    public class ObjectCommon : ChunkLoader
    {
        private ushort _valuesOffset;
        private ushort _stringsOffset;
        private int Identifier;
        private uint _fadeinOffset;
        private uint _fadeoutOffset;
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
        private ushort _unk;
        public Text Text;


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
                _unk = Reader.ReadUInt16();
                _extensionOffset = Reader.ReadUInt16();
                _counterOffset = Reader.ReadUInt16();
                Flags.flag = Reader.ReadUInt16();
                _end = Reader.Tell() + (8+1) * 2;
                
                Reader.Seek(_end);
                _systemObjectOffset = Reader.ReadUInt16();
                

                _valuesOffset = Reader.ReadUInt16();
                _stringsOffset = Reader.ReadUInt16();
                NewFlags.flag = Reader.ReadUInt16();
                preferences.flag = Reader.ReadUInt16();
                Identifier = Reader.ReadInt32();
                BackColor = Reader.ReadColor();
                _fadeinOffset = Reader.ReadUInt32();
                _fadeoutOffset = Reader.ReadUInt32();
            }
            else
            {
                _movementsOffset = Reader.ReadUInt16();
                _animationsOffset = Reader.ReadUInt16();
                var version = Reader.ReadUInt16();
                //_unk = Reader.ReadUInt16();
                _extensionOffset = Reader.ReadUInt16();
                _counterOffset = Reader.ReadUInt16();
                Flags.flag = Reader.ReadUInt16();
                _end = Reader.Tell() + (8+1) * 2;
                
                Reader.Seek(_end);
                _systemObjectOffset = Reader.ReadUInt16();
                

                _valuesOffset = Reader.ReadUInt16();
                _stringsOffset = Reader.ReadUInt16();
                NewFlags.flag = Reader.ReadUInt16();
                preferences.flag = Reader.ReadUInt16();
                Identifier = Reader.ReadInt32();
                BackColor = Reader.ReadColor();
                _fadeinOffset = Reader.ReadUInt32();
                _fadeoutOffset = Reader.ReadUInt32();
            }

            if (_animationsOffset > 0)
            {
                Reader.Seek(currentPosition+_animationsOffset);
                Animations=new Animations(Reader);
                Animations.Read();
            }
            
            if (_systemObjectOffset > 0)
            {
                Reader.Seek(currentPosition+_systemObjectOffset);
                if (Parent.ObjectType == 7) //Counter
                {
          
                    Counters=new Counters(Reader);
                    Counters.Read();
                }
                else if(Parent.ObjectType==3)//Text
                {
        
                    Text = new Text(Reader);
                    Text.Read();
                    
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