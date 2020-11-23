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
    class SoundBank : ChunkLoader
    {
        public int num_of_items = 0;
        public int references = 0;
        public List<SoundItem> items;
        public bool isCompressed = true;

        public override void Print(bool ext)
        {
        }

        public override void Read()
        {
            //Implementing for standalone-only because of my lazyness
            items = new List<SoundItem>();
            num_of_items = reader.ReadInt32();
            for (int i = 0; i < num_of_items; i++)
            {
                var item = new SoundItem(reader);
                item.isCompressed = isCompressed;
                item.Read();
                
                items.Add(item);
            }
        }

        public SoundBank(ByteIO reader) : base(reader)
        {
        }

        public SoundBank(Chunk chunk) : base(chunk)
        {
        }
    }

    public class SoundBase : ChunkLoader
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

        public SoundBase(ByteIO reader) : base(reader)
        {
        }

        public SoundBase(Chunk chunk) : base(chunk)
        {
        }
    }

    public class SoundItem : SoundBase
    {
        public bool compressed;
        public int checksum;
        public int references;
        public bool isCompressed = true;

        public override void Read()
        {
            var start = reader.Tell();
            
            handle = (int) reader.ReadUInt32();
            checksum = reader.ReadInt32();
            references = reader.ReadInt32();
            var decompressed_size = reader.ReadInt32();
            reader.ReadUInt32(); //flags
            var reserved = reader.ReadInt32();
            var name_lenght = reader.ReadInt32();
            ByteIO SoundData;
            if (isCompressed) //compressed
            {
                var size = reader.ReadInt32();
                SoundData = new ByteIO(Decompressor.decompress_block(reader, size, decompressed_size));
            }
            else
            {
                SoundData = new ByteIO(reader.ReadBytes(decompressed_size));
            }
            if (isCompressed)
            {
                name = SoundData.ReadWideString(name_lenght);
            }
            else
            {
                name = SoundData.ReadAscii(name_lenght);

            }


            this.data = SoundData.ReadBytes((int) SoundData.Size());
            name = Helper.CleanInput(name);
            Console.WriteLine($"Dumping {name}");

            string path = $"{Program.DumpPath}\\SoundBank\\{name}.wav";
            File.WriteAllBytes(path, data);
        }

        public SoundItem(ByteIO reader) : base(reader)
        {
        }

        public SoundItem(Chunk chunk) : base(chunk)
        {
        }
    }
}