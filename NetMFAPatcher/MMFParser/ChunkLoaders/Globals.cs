using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public class GlobalValues : ChunkLoader
    {
        public List<float> Items = new List<float>();
        public GlobalValues(Chunk chunk) : base(chunk) { }
        public override void Print(bool ext)
        {
            
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var numberOfItems = Reader.ReadUInt16();
            var templist = new List<ByteReader>();
            for (int i = 0; i < numberOfItems; i++)
            {
                templist.Add(new ByteReader(Reader.ReadBytes(4)));
            }
            foreach (var item in templist)
            {
                var globalType = Reader.ReadSByte();
                float newGlobal = 0f;
                if((Constants.ValueType)globalType==Constants.ValueType.Float)
                {
                    newGlobal = item.ReadSingle();
                }
                else if ((Constants.ValueType)globalType == Constants.ValueType.Int)
                {
                    newGlobal = item.ReadInt32();
                }
                else
                {
                    throw new Exception("unknown global type");
                }
                Items.Add(newGlobal);               
            }

            
        }
    }
    public class GlobalStrings : ChunkLoader
    {
        public List<string> Items = new List<string>();
        public GlobalStrings(Chunk chunk) : base(chunk) { }
        public override void Print(bool ext)
        {

        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var count = Reader.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                Items.Add(Reader.ReadAscii());
            }

        }
    }
}
