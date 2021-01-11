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
        public uint Width;
        public uint Height;
        public int IntegerDigits;
        public bool FormatFloat;
        public int FloatDigits;
        public bool UseDecimals;
        public int Decimals;
        public ushort Font;
        public bool Inverse;
        public bool AddNulls;

        public Counters(ByteReader reader) : base(reader)
        {
        }

        public Counters(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            
            var size = Reader.ReadUInt32();
            Width = Reader.ReadUInt32();
            Height = Reader.ReadUInt32();
            var player = Reader.ReadUInt16();
            var displayType = Reader.ReadUInt16();
            var flags = Reader.ReadUInt16();

            IntegerDigits = flags & _intDigitsMask;
            FormatFloat = (flags & _formatFloat) != 0;
            FloatDigits = (flags & _floatDigitsMask) >> _floatDigitsShift + 1;
            UseDecimals = (flags & _useDecimals) != 0;
            Decimals = (flags & _floatDecimalsMask) >> _floatDecimalsShift;
            AddNulls = (flags & _floatPad) != 0;

            Inverse = ByteFlag.GetFlag(flags, 8);
            Font = Reader.ReadUInt16();
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