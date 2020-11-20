
using NetMFAPatcher.mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.chunkloaders
{
    class FontBank : ChunkLoader
    {
        int numberOfItems;
        public override void Print()
        {
            Logger.Log($"FontCount:{numberOfItems.ToString()}");
        }

        public override void Read()
        {
            numberOfItems = reader.ReadInt32();
            




        }
        public FontBank(ByteIO reader) : base(reader)
        {
        }

        public FontBank(ChunkList.Chunk chunk) : base(chunk)
        {
        }
    }

}
