using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public abstract class ChunkLoader
    {
        public Chunk Chunk;
        public ByteReader Reader;

        public bool Verbose = false;

        public ChunkLoader(ByteReader reader)
        {
            this.Reader = reader;
        }

        public abstract void Read();
        public abstract void Write(ByteWriter Writer);
        public abstract string[] GetReadableData();


    }
}