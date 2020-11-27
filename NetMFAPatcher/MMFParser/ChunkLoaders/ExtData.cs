using NetMFAPatcher.chunkloaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    class ExtData : ChunkLoader
    {
        public ExtData(Chunk chunk) : base(chunk) { }
        public override void Print(bool ext)
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var filename = reader.ReadAscii();
            //var data = reader.ReadBytes();
        }
    }
}
