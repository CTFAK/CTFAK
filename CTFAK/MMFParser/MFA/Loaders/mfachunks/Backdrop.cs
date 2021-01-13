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