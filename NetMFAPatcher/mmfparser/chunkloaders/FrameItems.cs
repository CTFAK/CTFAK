using NetMFAPatcher.MMFParser.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.MMFParser.MFALoaders;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public class FrameItems : ChunkLoader
    {
        public Dictionary<int, ObjectInfo> ItemDict = new Dictionary<int, ObjectInfo>();
        public List<string> Names = new List<string>();
        public int NumberOfItems;
        public FrameItems(Chunk chunk) : base(chunk) { }
        public FrameItems(ByteReader reader) : base(reader) { }
        public override void Print(bool ext)
        {
                       
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Number of items: {NumberOfItems}"
            };
        }

        public override void Read()
        {
            NumberOfItems = Reader.ReadInt32();
            
            for (int i = 0; i < NumberOfItems; i++)
            {
                var item = new ObjectInfo(Reader);
                item.Verbose = false;
                item.Read();
                ItemDict.Add(item.Handle, item);
                Names.Add(item.Name);
                Logger.Log($"Found FrameItem: '{item.Name}' with handle ({item.Handle})", true, ConsoleColor.Magenta);
            }
            GameData.TestItems = this;

        }

        public ObjectInfo GetItemByHandle(int handle)
        {
            ItemDict.TryGetValue(handle, out var ret);
            return ret;
        }
    }
}
