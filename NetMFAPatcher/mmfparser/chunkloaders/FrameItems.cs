using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.mmfparser.chunkloaders
{
    class FrameItems : ChunkLoader
    {
        public Dictionary<int, ObjectInfo> itemDict = new Dictionary<int, ObjectInfo>();
        public List<string> names = new List<string>();
        public FrameItems(Chunk chunk) : base(chunk) { }
        public FrameItems(ByteIO reader) : base(reader) { }
        public override void Print(bool ext)
        {
                       
        }

        public override void Read()
        {
            Console.WriteLine("Reading FrameItems");
            var count = reader.ReadInt32();
            
            for (int i = 0; i < count; i++)
            {
                var item = new ObjectInfo(reader);
                item.verbose = false;
                item.Read();
                itemDict.Add(item.handle, item);
                names.Add(item.name);
                Logger.Log($"Found FrameItem: '{item.name}' with handle ({item.handle})", true, ConsoleColor.Magenta);
            }

        }
    }
}
