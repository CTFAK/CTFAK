using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders.Banks
{
    public class FontBank : ChunkLoader
    {
        public int NumberOfItems;
        public override void Print(bool ext)
        {
            Logger.Log($"FontCount:{NumberOfItems.ToString()}");
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }

        public override void Read()
        {
            NumberOfItems = Reader.ReadInt32();
        }
        public override void Write(ByteWriter writer)
        {
            writer.WriteInt32(NumberOfItems);
            //i am testing with no fonts

        }
        public FontBank(ByteReader reader) : base(reader)
        {
        }

        public FontBank(Chunk chunk) : base(chunk)
        {
        }
    }
    public class FontItem:ChunkLoader
    {
        public FontItem(ByteReader reader) : base(reader)
        {
        }

        public FontItem(Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var handle = Reader.ReadUInt32();

        }

        public override void Write(ByteWriter Writer)
        {
            throw new System.NotImplementedException();
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
