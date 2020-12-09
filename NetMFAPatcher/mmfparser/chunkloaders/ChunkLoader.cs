using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.Data.ChunkList;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders
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

        protected ChunkLoader(Chunk chunk)
        {
            this.Chunk = chunk;
            this.Reader = chunk.get_reader();
        }


        public abstract void Read();


        public abstract void Print(bool ext);
        public abstract string[] GetReadableData();


    }
}