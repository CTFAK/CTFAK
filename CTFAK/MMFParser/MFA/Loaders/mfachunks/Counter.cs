using System.Collections.Generic;
using System.Drawing;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders.mfachunks
{
    public class Counter:ObjectLoader
    {
        public int Value;
        public int Minimum;
        public int Maximum;
        public uint DisplayType;
        public uint Flags;
        public Color Color1;
        public uint VerticalGradient;
        public Color Color2;
        public int CountType;
        public int Width;
        public int Height;
        public List<int> Images;
        public uint Font;

        public Counter(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            base.Read();
            Value = Reader.ReadInt32();
            Minimum = Reader.ReadInt32();
            Maximum = Reader.ReadInt32();
            DisplayType = Reader.ReadUInt32();
            Flags = Reader.ReadUInt32();
            Color1 = Reader.ReadColor();
            Color2 = Reader.ReadColor();
            VerticalGradient = Reader.ReadUInt32();
            CountType = Reader.ReadInt32();
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            Images = new List<int>();
            var imageCount = Reader.ReadUInt32();
            for (int i = 0; i < imageCount; i++)
            {
                Images.Add((int) Reader.ReadUInt32());
            }

            Font = Reader.ReadUInt32();

        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            Writer.WriteInt32(Value);
            Writer.WriteInt32(Minimum);
            Writer.WriteInt32(Maximum);
            Writer.WriteUInt32(DisplayType);
            Writer.WriteUInt32(Flags);
            Writer.WriteColor(Color1);;
            Writer.WriteColor(Color2);;
            Writer.WriteUInt32(VerticalGradient);
            Writer.WriteInt32(CountType);
            Writer.WriteInt32(Width);
            Writer.WriteInt32(Height);
            Writer.WriteInt32(Images.Count);
            foreach (var item in Images)
            {
                Writer.WriteUInt32((uint) item);
            }
            Writer.WriteUInt32(Font);
        }
    }
}