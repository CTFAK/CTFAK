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

        

        public override void Read()
        {
            if (Settings.GameType == GameType.TwoFivePlus) Reader.Skip(-4);
            Size = Reader.ReadInt16();
            Initial = Reader.ReadInt32();
            Minimum = Reader.ReadInt32();
            Maximum = Reader.ReadInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
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
        public ushort DisplayType;
        public ushort Flags;
        public ushort Player;
        public Shape Shape;

        public Counters(ByteReader reader) : base(reader)
        {
        }

        

        public override void Read()
        {
            if (Settings.GameType == GameType.TwoFivePlus) Reader.Skip(-4);
            if (Settings.GameType == GameType.OnePointFive)
            {
                var size = Reader.ReadUInt32();
                Width = Reader.ReadUInt16();
                Height = Reader.ReadUInt16();
                Player = Reader.ReadUInt16();
                DisplayType = Reader.ReadUInt16();
                Flags = Reader.ReadUInt16();
                Inverse = ByteFlag.GetFlag(Flags, 8);
                if (DisplayType == 0) return;
                else if (DisplayType == 1 || DisplayType == 4|| DisplayType==50)
                {
                
                    Frames = new List<int>();
                    var count = Reader.ReadInt16();
                    for (int i = 0; i < count; i++)
                    {
                        Frames.Add(Reader.ReadUInt16());
                    }
                }
                else if (DisplayType == 2 || DisplayType == 3 || DisplayType == 5)
                {
                    Frames=new List<int>(){0};
                    Shape = new Shape(Reader);
                    Shape.Read();
                } 
            }
            else
            {
                var size = Reader.ReadUInt32();
                Width = Reader.ReadUInt32();
                Height = Reader.ReadUInt32();
                Player = Reader.ReadUInt16();
                DisplayType = Reader.ReadUInt16();
                Flags = Reader.ReadUInt16();

                IntegerDigits = Flags & _intDigitsMask;
                FormatFloat = (Flags & _formatFloat) != 0;
                FloatDigits = (Flags & _floatDigitsMask) >> _floatDigitsShift + 1;
                UseDecimals = (Flags & _useDecimals) != 0;
                Decimals = (Flags & _floatDecimalsMask) >> _floatDecimalsShift;
                AddNulls = (Flags & _floatPad) != 0;

                Inverse = ByteFlag.GetFlag(Flags, 8);
                Font = Reader.ReadUInt16();
                if (DisplayType == 0) return;
                else if (DisplayType == 1 || DisplayType == 4|| DisplayType==50)
                {
                
                    Frames = new List<int>();
                    var count = Reader.ReadInt16();
                    for (int i = 0; i < count; i++)
                    {
                        Frames.Add(Reader.ReadUInt16());
                    }
                }
                else if (DisplayType == 2 || DisplayType == 3 || DisplayType == 5)
                {
                    Frames=new List<int>(){0};
                    Shape = new Shape(Reader);
                    Shape.Read();
                } 
            }
            

        }

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
    
}