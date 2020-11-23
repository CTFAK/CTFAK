using NetMFAPatcher.mfa;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.chunkloaders
{
    public abstract class ChunkLoader//:DataLoader
    {
        public Chunk chunk;
        public ByteIO reader;
        public bool verbose = true;

        protected ChunkLoader(ByteIO reader)
        {
            this.reader = reader;
        }
        protected ChunkLoader(Chunk chunk)
        {
            this.chunk = chunk;
            this.reader = chunk.get_reader();
        }


        public abstract void Read();


        public abstract void Print(bool ext);
    }
}