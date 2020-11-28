using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public class FrameItems : ChunkLoader
    {
        public Dictionary<int, ObjectInfo> ItemDict = new Dictionary<int, ObjectInfo>();
        public List<string> Names = new List<string>();
        public FrameItems(Chunk chunk) : base(chunk) { }
        public FrameItems(ByteIO reader) : base(reader) { }
        public override void Print(bool ext)
        {
                       
        }

        public override void Read()
        {
            var count = Reader.ReadInt32();
            
            for (int i = 0; i < count; i++)
            {
                var item = new ObjectInfo(Reader);
                item.Verbose = false;
                item.Read();
                ItemDict.Add(item.Handle, item);
                Names.Add(item.Name);
                //Logger.Log($"Found FrameItem: '{item.name}' with handle ({item.handle})", true, ConsoleColor.Magenta);
            }
            GameData.TestItems = this;

        }
    }
}
