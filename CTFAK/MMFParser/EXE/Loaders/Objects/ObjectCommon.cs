using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Objects
{
    public class ObjectCommon : ChunkLoader
    {
        private ushort _valuesOffset;
        private ushort _stringsOffset;
        private uint _fadeinOffset;
        private uint _fadeoutOffset;
        private ushort _movementsOffset;
        private ushort _animationsOffset;
        private ushort _systemObjectOffset;
        public ushort _counterOffset;
        public ushort _extensionOffset;
        public string Identifier;
        
        public Animations Animations;

        public BitDict Preferences = new BitDict(new string[]
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
        private ushort _zeroUnk;
        public Text Text;
        public Counter Counter;
        public short[] _qualifiers=new short[8];


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
            if (Settings.GameType == GameType.OnePointFive)
            {
                var currentPosition = Reader.Tell();
                var size = Reader.ReadInt16();
                var checksum = Reader.ReadInt16();
                _movementsOffset = (ushort) Reader.ReadInt16();
                _animationsOffset = (ushort) Reader.ReadInt16();
                var version = Reader.ReadInt16();
                _counterOffset = (ushort) Reader.ReadInt16();
                _systemObjectOffset = (ushort) Reader.ReadInt16();
                var ocVariable = Reader.ReadInt32();
                Flags.flag = (uint) Reader.ReadInt16();
                var end = Reader.Tell() + 8 * 2;//maybe its 9*2
                Reader.Seek(end);

                _extensionOffset = (ushort) Reader.ReadInt16();
                _valuesOffset = (ushort) Reader.ReadInt16();
                NewFlags.flag = (uint) Reader.ReadInt16();
                Preferences.flag = (uint) Reader.ReadInt16();
                Identifier = Reader.ReadAscii(2);
                BackColor = Reader.ReadColor();
                _fadeinOffset = (uint) Reader.ReadInt32();
                _fadeoutOffset = (uint) Reader.ReadInt32();


            }
            else
            {
                var currentPosition = Reader.Tell();
                var size = Reader.ReadInt32();
                if (Settings.Build >= 284)
                {
                    _animationsOffset = Reader.ReadUInt16();
                    _movementsOffset = Reader.ReadUInt16();
                    var version = Reader.ReadUInt16();
                    Reader.Skip(2);
                    _extensionOffset = Reader.ReadUInt16();
                    _counterOffset = Reader.ReadUInt16();
                }
                else
                {
                    _movementsOffset = Reader.ReadUInt16();
                    _animationsOffset = Reader.ReadUInt16();
                    var version = Reader.ReadUInt16();
                    Reader.Skip(2);
                    _extensionOffset = Reader.ReadUInt16();
                    _counterOffset = Reader.ReadUInt16();
                }

                Flags.flag = Reader.ReadUInt16();
                Reader.Skip(2);
                var end = Reader.Tell() + 8 * 2;
                for (int i = 0; i < 8; i++)
                {
                    _qualifiers[i] = Reader.ReadInt16();
                }
                Reader.Seek(end);

                if (Settings.Build >= 284)
                {
                    _systemObjectOffset = Reader.ReadUInt16();
                }
                else
                {
                    _extensionOffset = Reader.ReadUInt16();
                }

                _valuesOffset = Reader.ReadUInt16();
                _stringsOffset = Reader.ReadUInt16();
                NewFlags.flag = Reader.ReadUInt32();
                Preferences.flag = Reader.ReadUInt32();
                Identifier = Reader.ReadAscii(4);
                BackColor = Reader.ReadColor();
                _fadeinOffset = Reader.ReadUInt32();
                _fadeoutOffset = Reader.ReadUInt32();
            
                
                if (_animationsOffset > 0)
                {
                    Reader.Seek(currentPosition + _animationsOffset);
                    Animations = new Animations(Reader);
                    Animations.Read();
                }


                if (_movementsOffset > 0&&Settings.GameType!=GameType.Android)
                {
                    Reader.Seek(currentPosition + _movementsOffset);
                    Movements = new Movements(Reader);
                    Movements.Read();
                }

                if (_systemObjectOffset > 0)
                {
                    Reader.Seek(currentPosition + _systemObjectOffset);
                    switch (Parent.ObjectType)
                    {
                        //Text
                        case Constants.ObjectType.Text:
                            Text = new Text(Reader);
                            Text.Read();
                            break;
                        //Counter
                        case Constants.ObjectType.Counter:
                            Counters = new Counters(Reader);
                            Counters.Read();
                            break;
                        case Constants.ObjectType.Score:
                            Counters = new Counters(Reader);
                            Counters.Read();
                            break;
                        case Constants.ObjectType.Lives:
                            Counters = new Counters(Reader);
                            Counters.Read();
                            break;

                    }
                }

                if (_extensionOffset > 0)
                {
                    Reader.Seek(currentPosition + _extensionOffset);

                    var dataSize = Reader.ReadInt32() - 20;
                    Reader.Skip(4); //maxSize;
                    ExtensionVersion = Reader.ReadInt32();
                    ExtensionId = Reader.ReadInt32();
                    ExtensionPrivate = Reader.ReadInt32();
                    if (dataSize != 0)
                    {
                        ExtensionData = Reader.ReadBytes(dataSize);
                    }
                    else ExtensionData = new byte[0];
                }

                if (_counterOffset > 0)
                {
                    Reader.Seek(currentPosition + _counterOffset);
                    Counter = new Counter(Reader);
                    Counter.Read();
                }


            }

            

        }

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
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