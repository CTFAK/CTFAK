using System.Collections.Generic;
using System.Linq;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class FrameHandles:ChunkLoader
    {
        public Dictionary<int,int> Items;

        public FrameHandles(ByteReader reader) : base(reader)
        {
        }

        public FrameHandles(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            
            var len = Reader.Size() / 2;
            Items = new Dictionary<int,int>();
            for (int i = 0; i < len; i++)
            {
                var handle = Reader.ReadInt16();
                Logger.Log("Frame Handle: "+handle);
                Items.Add(i,handle);
            }

        }

        public override void Print(bool ext)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
}