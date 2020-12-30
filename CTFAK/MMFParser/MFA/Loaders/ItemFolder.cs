using System.Collections.Generic;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders
{
    public class ItemFolder:DataLoader
    {
        public List<uint> Items;
        public string Name;
        public uint UnkHeader;
        public uint NonFolder;

        public ItemFolder(ByteReader reader) : base(reader)
        {
        }

        public ItemFolder(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            UnkHeader = Reader.ReadUInt32();
            if (UnkHeader == 0x70000004)
            {
                Name = Reader.AutoReadUnicode();
                Items = new List<uint>();
                var count = Reader.ReadUInt32();
                for (int i = 0; i < count; i++)
                {
                    Items.Add(Reader.ReadUInt32());
                }
            }
            else
            {
                Name = null;
                Items = new List<uint>();
                Items.Add(Reader.ReadUInt32());
            }
        }

        public override void Write(ByteWriter Writer)
        {
            if (Name == null)
            {
                Writer.WriteInt32(0x70000005);
                Writer.WriteInt32(3);
            }
            else
            {
                Writer.WriteInt32(0x70000004);
                Writer.AutoWriteUnicode(Name);
                Writer.WriteInt32(Items.Count);
                foreach (var item in Items)
                {
                    Writer.WriteUInt32(item);
                }
            }

            
            
            
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }
    }
}