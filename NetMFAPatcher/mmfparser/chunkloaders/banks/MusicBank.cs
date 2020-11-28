using System.Collections.Generic;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.banks
{
    public class MusicBank : ChunkLoader
    {
        public int NumOfItems = 0;
        public int References = 0;
        public List<MusicFile> Items;

        public override void Print(bool ext)
        {
        }

        public override void Read()
        {
            //Someone is using this lol?
            Items = new List<MusicFile>();
            NumOfItems = Reader.ReadInt32();
            for (int i = 0; i < NumOfItems; i++)
            {
                var item = new MusicFile(Reader);
                item.Read();
                Items.Add(item);
            }
        }

        public MusicBank(ByteIO reader) : base(reader)
        {
        }

        public MusicBank(Chunk chunk) : base(chunk)
        {
        }
    }

    public class MusicFile : ChunkLoader
    {
        public int Handle;
        public string Name = "ERROR";
        public byte[] Data;

        public override void Print(bool ext)
        {
        }

        public override void Read()
        {
        }

        public MusicFile(ByteIO reader) : base(reader)
        {
        }

        public MusicFile(Chunk chunk) : base(chunk)
        {
        }
    }

   
}