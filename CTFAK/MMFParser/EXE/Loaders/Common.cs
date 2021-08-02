using CTFAK.Utils;
using System;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class Rect:ChunkLoader
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public Rect(ByteReader reader) : base(reader){}

        public override void Read()
        {
            //if (Settings.GameType == GameType.TwoFivePlus) Console.WriteLine("position: " + Reader.Tell() + " size: " + Reader.Size());
            if (Reader.Tell() > Reader.Size() + 1 || Reader.Size() < 4)
            {
                Console.WriteLine("E19:  Ran out of bytes reading Common Rectangle Loader (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
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

        public override string[] GetReadableData() => new string[]
        {
            $"Value: {Right}x{Bottom}"
        };
    }
}