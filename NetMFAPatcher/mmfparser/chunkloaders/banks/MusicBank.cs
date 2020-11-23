using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.mmfparser;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.chunkloaders
{
    class MusicBank : ChunkLoader
    {
        public int num_of_items = 0;
        public int references = 0;
        public List<MusicFile> items;

        public override void Print(bool ext)
        {
        }

        public override void Read()
        {
            //Someone is using this lol?
            items = new List<MusicFile>();
            num_of_items = reader.ReadInt32();
            for (int i = 0; i < num_of_items; i++)
            {
                var item = new MusicFile(reader);
                item.Read();
                items.Add(item);
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
        public int handle;
        public string name = "ERROR";
        public byte[] data;

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