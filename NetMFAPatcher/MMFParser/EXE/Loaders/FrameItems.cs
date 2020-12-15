using System;
using System.Collections.Generic;
using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.EXE.ChunkList;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders
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
                item.Read();
                ItemDict.Add(item.Handle, item);
                Names.Add(item.Name);
                // Logger.Log($"Found FrameItem: '{item.Name}' with handle ({item.Handle})", true, ConsoleColor.Magenta);
            }
            GameData.TestItems = this;

        }

        public ObjectInfo FromHandle(int handle)
        {
            ItemDict.TryGetValue(handle, out var ret);
            return ret;
        }

        public List<ObjectInfo> FromName(string name)
        {
            var tempList = new List<ObjectInfo>();
            foreach (var key in ItemDict.Keys)
            {
                var item = ItemDict[key];
                if(item.Name==name)tempList.Add(item);
            }

            return tempList;
        }
    }
}
