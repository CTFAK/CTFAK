using System.Collections.Generic;
using CTFAK.MMFParser.Attributes;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class FrameItems : ChunkLoader
    {
        public Dictionary<int, ObjectInfo> ItemDict = new Dictionary<int, ObjectInfo>();
        public FrameItems(ByteReader reader) : base(reader) { }
        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(ItemDict.Count);
            foreach (ObjectInfo objectInfo in ItemDict.Values)
            {
                objectInfo.Write(Writer);
            }
        }

        public override void Print(bool ext)
        {
                       
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Number of items: {ItemDict.Count}"
            };
        }

        public override void Read()
        {
            var count = Reader.ReadInt32();
            
            for (int i = 0; i < count; i++)
            {
                var item = new ObjectInfo(Reader);
                item.Read();
                ItemDict.Add(item.Handle, item);
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
