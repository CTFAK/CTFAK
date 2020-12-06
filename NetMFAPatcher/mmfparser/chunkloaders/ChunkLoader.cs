using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public abstract class ChunkLoader//:DataLoader
    {
        public Chunk Chunk;
        public ByteReader Reader;
        public bool Verbose = false;

        protected ChunkLoader(ByteReader reader)
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