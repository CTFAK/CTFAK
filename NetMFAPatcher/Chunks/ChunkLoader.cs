using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.mmfparser.ChunkList;

namespace NetMFAPatcher.Chunks
{
    public abstract class ChunkLoader
    {
        public Chunk chunk;
        public ByteIO reader;


        public abstract void Read();


        public abstract void Print();
        




    }
}
