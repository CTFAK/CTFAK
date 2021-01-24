using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class Rect:ChunkLoader
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public Rect(ByteReader reader) : base(reader){}
        public Rect(ChunkList.Chunk chunk) : base(chunk){}
        public override void Read()
        {
            Left = Reader.ReadInt32();
            Top = Reader.ReadInt32();
            Right = Reader.ReadInt32();
            Bottom = Reader.ReadInt32();
        }

        public override void Print(bool ext){}
        public override string[] GetReadableData() => null;
    }
}