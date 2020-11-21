using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.chunkloaders
{
    class ObjectInfo : ChunkLoader
    {
        public ObjectInfo(ChunkList.Chunk chunk) : base(chunk) { }
        public ObjectInfo(ByteIO reader) : base(reader) { }
        public override void Print()
        {
            
        }

        public override void Read()
        {
            
        }
    }
}
