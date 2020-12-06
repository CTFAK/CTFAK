using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Objects
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


        public ObjectCommon(ByteReader reader) : base(reader)
        {
        }

        public ObjectCommon(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            long currentPosition = Reader.Tell();
            int size = Reader.ReadInt32();
            bool newobj = Settings.Build > 284;
            if (newobj && true)
            {
                _animationsOffset = (short) Reader.ReadUInt16();
                _movementsOffset = Reader.ReadUInt16();
                short version = Reader.ReadInt16();
                Reader.ReadBytes(2);
                ushort extensionOffset = Reader.ReadUInt16();
                ushort counterOffset = Reader.ReadUInt16();
                flags.flag = Reader.ReadUInt32();

                long end = Reader.Tell() + 16;

                List<short> qualifiers = new List<short>();
                for (int i = 0; i < 8; i++)
                {
                    short qualifier = Reader.ReadInt16();
                    if (qualifier == -1) break;
                    qualifiers.Add(qualifier);
                }

                Reader.Seek(end);

                short systemObjectOffset = Reader.ReadInt16();
                
                _valuesOffset = Reader.ReadInt16();
                _stringsOffset = Reader.ReadInt16();
                new_flags.flag = Reader.ReadUInt16();
                preferences.flag = Reader.ReadUInt16();
                Identifier = Reader.ReadFourCc();
                Color backColor = Reader.ReadColor();
                int fadeinOffset = Reader.ReadInt32();
                int fadeoutOffset = Reader.ReadInt32();
            }
            else if (newobj)
            {
                _counterOffset = Reader.ReadInt16();
                short version = Reader.ReadInt16();
                Reader.ReadBytes(2);
                _movementsOffset = (ushort) Reader.ReadInt16();
                _extensionOffset = Reader.ReadInt16();
                _animationsOffset = Reader.ReadInt16();
                uint flags = Reader.ReadUInt32();
                long end = Reader.Tell() + 16;
                List<short> qualifiers = new List<short>();
                for (int i = 0; i < 8; i++)
                {
                    short qualifier = Reader.ReadInt16();
                    if (qualifier == -1) break;
                    qualifiers.Add(qualifier);
                }

                Reader.Seek(end);

                _valuesOffset = Reader.ReadInt16();
                _stringsOffset = Reader.ReadInt16();
                ushort newFlags = Reader.ReadUInt16();
                byte[] preferences = Reader.ReadFourCc();
                Color backColor = Reader.ReadColor();
                _fadeinOffset = Reader.ReadInt32();
                _fadeoutOffset = Reader.ReadInt32();
            }
            else
            {
                _movementsOffset = Reader.ReadUInt16();
                _animationsOffset = Reader.ReadInt16();
                short version = Reader.ReadInt16();
                _counterOffset = Reader.ReadInt16();
                _systemObjectOffset = Reader.ReadInt16();
                Reader.ReadBytes(2);
                flags.flag = Reader.ReadUInt32();

                _end = Reader.Tell() + 16;

                List<short> qualifiers = new List<short>();
                for (int i = 0; i < 8; i++)
                {
                    short qualifier = Reader.ReadInt16();
                    if (qualifier == -1) break;
                    qualifiers.Add(qualifier);
                }

                Reader.Seek(_end);

                _extensionOffset = Reader.ReadInt16();

                _valuesOffset = Reader.ReadInt16();
                _stringsOffset = Reader.ReadInt16();
                new_flags.flag = Reader.ReadUInt16();
                preferences.flag = Reader.ReadUInt16();
                byte[] identifier = Reader.ReadFourCc();
                Color backColor = Reader.ReadColor();
                _fadeinOffset = Reader.ReadInt32();
                _fadeoutOffset = Reader.ReadInt32();
            }

            if (_movementsOffset != 0)
            {
                //Reader.Seek(currentPosition+_movementsOffset);
                //var movements = new Movements(Reader);
                //movements.Read();
            }

            Console.WriteLine("Movements done");
            if (_valuesOffset != 0)
            {
                Reader.Seek(currentPosition + _valuesOffset);
                AlterableValues values = new AlterableValues(Reader);
                values.Read();
            }

            Console.WriteLine("Values done");
            if (_stringsOffset != 0)
            {
                Reader.Seek(currentPosition + _stringsOffset);
                AlterableStrings strings = new AlterableStrings(Reader);
                strings.Read();
            }

            Console.WriteLine("Strings done");
            if (_animationsOffset != 0)
            {
                Reader.Seek(currentPosition + _stringsOffset);
                Animations = new Animations(Reader);

                Animations.Read();
            }

            Console.WriteLine("Animations done");
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