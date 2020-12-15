using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.EXE.ChunkList;

namespace DotNetCTFDumper.MMFParser.EXE
{
    public abstract class DataLoader
    {
        private Chunk _chunk;
        public ByteReader Reader;
        public bool Verbose = true;

        protected DataLoader(ByteReader reader)
        {
            this.Reader = reader;
        }
        protected DataLoader(Chunk chunk)
        {
            this._chunk = chunk;
            this.Reader = chunk.get_reader();
        }

        public abstract void Read();
        public abstract void Write(ByteWriter Writer);
        public abstract void Print();


    }
}
