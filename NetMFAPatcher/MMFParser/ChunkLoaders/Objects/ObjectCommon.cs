using System;
using System.Collections.Generic;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Objects
{
    public class ObjectCommon:ChunkLoader
    {
        private short _valuesOffset;
        private short _stringsOffset;
        private int _fadeinOffset;
        private int _fadeoutOffset;
        private ushort _movementsOffset;
        private short _animationsOffset;
        private short _systemObjectOffset;
        private short _counterOffset;
        private short _extensionOffset;
        public Animations Animations;
        private long _end;


        public ObjectCommon(ByteIO reader) : base(reader)
        {
        }

        public ObjectCommon(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {

            var currentPosition = Reader.Tell();
            var size = Reader.ReadInt32();
            var newobj = Settings.Build > 284;
            var newobj2 = true;
            if (newobj&&newobj2)
            {
                _animationsOffset = (short) Reader.ReadUInt16();
                _movementsOffset = Reader.ReadUInt16();
                var Version = Reader.ReadInt16();
                Reader.ReadBytes(2);
                var extensionOffset = Reader.ReadUInt16();
                var counterOffset = Reader.ReadUInt16();
                var flags = Reader.ReadUInt32();

                var end = Reader.Tell() + 16;

                var qualifiers = new List<short>();
                for (int i = 0; i < 8; i++)
                {
                    var qualifier = Reader.ReadInt16();
                    if (qualifier == -1) break;
                    qualifiers.Add(qualifier);
                }
                
                Reader.Seek(end);

                var systemObjectOffset = Reader.ReadInt16();
                _valuesOffset = Reader.ReadInt16();
                _stringsOffset = Reader.ReadInt16();
                var newFlags = Reader.ReadUInt16();
                var preferences = Reader.ReadFourCc();
                var backColor = Reader.ReadColor();
                var fadeinOffset = Reader.ReadInt32();
                var fadeoutOffset = Reader.ReadInt32();
                
            }
            else if (newobj)
            {
                _counterOffset = Reader.ReadInt16();
                var version = Reader.ReadInt16();
                Reader.ReadBytes(2);
                _movementsOffset = (ushort) Reader.ReadInt16();
                _extensionOffset = Reader.ReadInt16();
                _animationsOffset = Reader.ReadInt16();
                var flags = Reader.ReadUInt32();
                var end = Reader.Tell() + 16;
                var qualifiers = new List<short>();
                for (int i = 0; i < 8; i++)
                {
                    var qualifier = Reader.ReadInt16();
                    if (qualifier == -1) break;
                    qualifiers.Add(qualifier);
                }
                
                Reader.Seek(end);
                
                _valuesOffset = Reader.ReadInt16();
                _stringsOffset = Reader.ReadInt16();
                var newFlags = Reader.ReadUInt16();
                var preferences = Reader.ReadFourCc();
                var backColor = Reader.ReadColor();
                _fadeinOffset = Reader.ReadInt32();
                _fadeoutOffset = Reader.ReadInt32();
            }
            else
            {
                _movementsOffset = Reader.ReadUInt16();
                _animationsOffset = Reader.ReadInt16();
                var version = Reader.ReadInt16();
                _counterOffset = Reader.ReadInt16();
                _systemObjectOffset = Reader.ReadInt16();
                Reader.ReadBytes(2);
                var flags = Reader.ReadUInt32();
                
                _end = Reader.Tell() + 16;
                
                var qualifiers = new List<short>();
                for (int i = 0; i < 8; i++)
                {
                    var qualifier = Reader.ReadInt16();
                    if (qualifier == -1) break;
                    qualifiers.Add(qualifier);
                }
                
                Reader.Seek(_end);
                
                _extensionOffset = Reader.ReadInt16();
                
                _valuesOffset = Reader.ReadInt16();
                _stringsOffset = Reader.ReadInt16();
                var newFlags = Reader.ReadUInt16();
                var preferences = Reader.ReadUInt16();
                var identifier = Reader.ReadFourCc();
                var backColor = Reader.ReadColor();
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
                Reader.Seek(currentPosition+_valuesOffset);
                var values = new AlterableValues(Reader);
                values.Read();
            }
            Console.WriteLine("Values done");
            if (_stringsOffset != 0)
            {
                Reader.Seek(currentPosition+_stringsOffset);
                var strings = new AlterableStrings(Reader);
                strings.Read();
            }
            Console.WriteLine("Strings done");
            if (_animationsOffset != 0)
            {
                Reader.Seek(currentPosition+_stringsOffset);
                Animations = new Animations(Reader);
                
                Animations.Read();
            }
            Console.WriteLine("Animations done");
            Console.WriteLine("SysObjOff: "+_systemObjectOffset);
            Console.WriteLine("ExtOff: "+_extensionOffset);

            





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