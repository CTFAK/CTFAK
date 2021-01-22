using System.Collections.Generic;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders.mfachunks
{
    public class Lives:ObjectLoader
    {
        public uint Player;
        public List<int> Images;
        public int Flags;
        public int DisplayType;
        public int Font;
        public int Width;
        public int Height;

        public Lives(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            base.Read();
            Player = Reader.ReadUInt32();
            Images = new List<int>();
            var imgCount = Reader.ReadInt32();
            for (int i = 0; i < imgCount; i++)
            {
                Images.Add(Reader.ReadInt32());
            }

            DisplayType = Reader.ReadInt32();
            Flags = Reader.ReadInt32();
            Font = Reader.ReadInt32();
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            Writer.WriteInt32((int) Player);
            Writer.WriteInt32(Images.Count);
            foreach (int i in Images)
            {
                Writer.WriteInt32(i);
            }
            Writer.WriteInt32(DisplayType);
            Writer.WriteInt32(Flags);
            Writer.WriteInt32(Font);
            Writer.WriteInt32(Width);
            Writer.WriteInt32(Height);
            
        }
    }
}