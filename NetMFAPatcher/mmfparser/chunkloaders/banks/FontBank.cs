
using NetMFAPatcher.mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.chunkloaders
{
    public class FontBank : ChunkLoader
    {
        public int numberOfItems;
        public override void Print(bool ext)
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

        public FontBank(Chunk chunk) : base(chunk)
        {
        }
    }

}
