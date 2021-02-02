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

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(Left);
            Writer.WriteInt32(Top);
            Writer.WriteInt32(Right);
            Writer.WriteInt32(Bottom);
        }

        public override void Print(bool ext){}
        public override string[] GetReadableData() => new string[]
        {
            $"Value: {Right}x{Bottom}"
        };
    }
}