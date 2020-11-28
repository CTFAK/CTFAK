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
        private Chunk _chunk;
        public ByteIO Reader;
        public bool Verbose = true;

        protected DataLoader(ByteIO reader)
        {
            this.Reader = reader;
        }
        protected DataLoader(Chunk chunk)
        {
            this._chunk = chunk;
            this.Reader = chunk.get_reader();
        }

        public abstract void Read();
        //public abstract void Write();
        public abstract void Print();


    }
}
