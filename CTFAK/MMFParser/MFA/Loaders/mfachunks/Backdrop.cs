using System.Drawing;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders.mfachunks
{
    public class Backdrop:BackgroundLoader
    {
        public int Handle;

        public Backdrop(ByteReader reader) : base(reader)
        {
        }

        public Backdrop(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            base.Read();
            Handle = Reader.ReadInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            Writer.WriteInt32(Handle);
        }
    }
    public class QuickBackdrop:BackgroundLoader
    {
        public int Width;
        public int Height;
        public int Shape;
        public int BorderSize;
        public Color BorderColor;
        public int FillType;
        public Color Color1;
        public Color Color2;
        public int Flags;
        public int Image;

        public QuickBackdrop(ByteReader reader) : base(reader)
        {
        }

        public QuickBackdrop(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            base.Read();
            Width = Reader.ReadInt32();
            Height = Reader.ReadInt32();
            Shape = Reader.ReadInt32();
            BorderSize = Reader.ReadInt32();
            BorderColor = Reader.ReadColor();

            FillType = Reader.ReadInt32();
            Color1 = Reader.ReadColor();
            Color2 = Reader.ReadColor();
            Flags = Reader.ReadInt32();
            Image = Reader.ReadInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            Writer.WriteInt32(Width);
            Writer.WriteInt32(Height);
            Writer.WriteInt32(Shape);
            Writer.WriteInt32(BorderSize);
            Writer.WriteColor(BorderColor);
            
            Writer.WriteInt32(FillType);
            Writer.WriteColor(Color1);
            Writer.WriteColor(Color2);
            Writer.WriteInt32(Flags);
            Writer.WriteInt32(Image);
        }
    }

    public class BackgroundLoader : DataLoader
    {
        public uint ObstacleType;
        public uint CollisionType;

        public BackgroundLoader(ByteReader reader) : base(reader)
        {
        }

        public BackgroundLoader(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            ObstacleType = Reader.ReadUInt32();
            CollisionType = Reader.ReadUInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUInt32(ObstacleType);
            Writer.WriteUInt32(CollisionType);
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }
    }
}