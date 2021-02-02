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

        protected ChunkLoader(Chunk chunk)
        {
            this.Chunk = chunk;
            this.Reader = chunk.GetReader();
        }


        public abstract void Read();
        public abstract void Write(ByteWriter Writer);


        public abstract void Print(bool ext);
        public abstract string[] GetReadableData();


    }
}