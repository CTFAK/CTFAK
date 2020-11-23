using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace mmfparser
{
    public abstract class DataLoader
    {
        private Chunk chunk;
        public ByteIO reader;
        public bool verbose = true;

        protected DataLoader(ByteIO reader)
        {
            this.reader = reader;
        }
        protected DataLoader(Chunk chunk)
        {
            this.chunk = chunk;
            this.reader = chunk.get_reader();
        }

        public abstract void Read();
        //public abstract void Write();
        public abstract void Print();


    }
}
