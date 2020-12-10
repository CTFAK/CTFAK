using System.Collections.Generic;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFALoaders
{
    public class ItemFolder:DataLoader
    {
        public List<uint> Items;
        public string Name;

        public ItemFolder(ByteReader reader) : base(reader)
        {
        }

        public ItemFolder(Data.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var value = Reader.ReadUInt32();
            if (value == 0x70000004)
            {
                Name = Helper.AutoReadUnicode(Reader);
                Items = new List<uint>();
                var count = Reader.ReadUInt32();
                for (int i = 0; i < count; i++)
                {
                    Items.Add(Reader.ReadUInt32());
                }
            }
            else
            {
                Name = "";
                Items = new List<uint>();
            }
            

        }

        public override void Write(ByteWriter Writer)
        {
            if (Name.Length == 0)
            {
                Writer.WriteUInt32(0x70000005);
            }
            else
            {
                Writer.WriteUInt32(0x70000004);
                Writer.AutoWriteUnicode(Name);
                Writer.WriteInt32(Items.Count);
            }

            foreach (var item in Items)
            {
                Writer.WriteUInt32(item);
            }
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }
    }
}