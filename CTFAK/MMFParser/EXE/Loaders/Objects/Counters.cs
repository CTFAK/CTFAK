using System;
using System.Collections.Generic;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Objects
{
    public class Counter:ChunkLoader
    {
        public short Size;
        public int Initial;
        public int Minimum;
        public int Maximum;

        public Counter(ByteReader reader) : base(reader)
        {
        }

        public Counter(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Size = Reader.ReadInt16();
            Initial = Reader.ReadInt32();
            Minimum = Reader.ReadInt32();
            Maximum = Reader.ReadInt32();
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

    public class Counters : ChunkLoader
    {
        int _intDigitsMask = 0xF;
        int _floatDigitsMask = 0xF0;
        int _formatFloat = 0x0200;
        int _floatDigitsShift = 4;
        int _useDecimals = 0x0400;
        int _floatDecimalsMask = 0xF000;
        int _floatDecimalsShift = 12;
        int _floatPad = 0x0800;
        public List<int> Frames;

        public Counters(ByteReader reader) : base(reader)
        {
        }

        public Counters(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            
            var size = Reader.ReadUInt32();
            var width = Reader.ReadUInt32();
            var height = Reader.ReadUInt32();
            var player = Reader.ReadUInt16();
            var displayType = Reader.ReadUInt16();
            var flags = Reader.ReadUInt16();

            var integerDigits = flags & _intDigitsMask;
            var formatFloat = (flags & _formatFloat) != 0;
            var floatDigits = (flags & _floatDigitsMask) >> _floatDigitsShift + 1;
            var useDecimals = (flags & _useDecimals) != 0;
            var decimals = (flags & _floatDecimalsMask) >> _floatDecimalsShift;
            var addNulls = (flags & _floatPad) != 0;

            var inverse = ByteFlag.GetFlag(flags, 8);
            var font = Reader.ReadUInt16();
            if (displayType == 0) return;
            else if (displayType == 1 || displayType == 4|| displayType==50)
            {
                
                Frames = new List<int>();
                var count = Reader.ReadInt16();
                for (int i = 0; i < count; i++)
                {
                    Frames.Add(Reader.ReadUInt16());
                }
            }
            else if (displayType == 2 || displayType == 3 || displayType == 5)
            {
                //TODO: Shapes
                Logger.Log("Ignoring unsupported counter type");
            }

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