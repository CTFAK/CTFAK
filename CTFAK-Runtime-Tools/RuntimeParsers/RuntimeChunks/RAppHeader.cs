using System;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.Utils;

namespace CTFAK_Runtime_Tools.RuntimeParsers.RuntimeChunks
{
    
    public class RAppHeader:AppHeader
    {
        public RAppHeader(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            var start = Reader.Tell();

            Size = Reader.ReadInt32();
            Flags.flag = (uint) Reader.ReadInt16();

            NewFlags.flag = (uint) Reader.ReadInt16();
            GraphicsMode = Reader.ReadInt16();
            Otherflags = Reader.ReadInt16();
            WindowWidth = Reader.ReadInt16();
            WindowHeight = Reader.ReadInt16();
            InitialScore = (int) (Reader.ReadUInt32() ^ 0xffffffff);
            InitialLives = (int) (Reader.ReadUInt32() ^ 0xffffffff);
            Controls = new Controls(Reader);

            // if (Settings.GameType == GameType.OnePointFive) Reader.Skip(56);
            // else Controls.Read();
            Controls.Read();
            BorderColor = Reader.ReadColor();
            NumberOfFrames = Reader.ReadInt32();
            FrameRate = Reader.ReadInt32();
            WindowsMenuIndex = Reader.ReadInt32();
        }
    }
}