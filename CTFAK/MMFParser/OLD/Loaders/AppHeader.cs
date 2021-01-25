using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.Utils;

namespace CTFAK.MMFParser.OLD.Loaders
{
    public class AppHeader:ChunkLoader
    {
        public AppHeader(ByteReader reader) : base(reader)
        {
        }

        public AppHeader(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var flags = Reader.ReadUInt16();
            var newFlags = Reader.ReadUInt16();
            var mode = Reader.ReadUInt16();
            var otherFlags = Reader.ReadUInt16();
            
            var windowWidth = Reader.ReadUInt16();
            var windowHeight = Reader.ReadUInt16();
            var initialScore = Reader.ReadUInt32() ^ 0xffffffff;
            var initialLives = Reader.ReadUInt32() ^ 0xffffffff;
            var controls = new Controls(Reader);
            controls.Read();
            var borderColor = Reader.ReadColor();
            var frameCount = Reader.ReadInt32();
            Logger.Log(frameCount);
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