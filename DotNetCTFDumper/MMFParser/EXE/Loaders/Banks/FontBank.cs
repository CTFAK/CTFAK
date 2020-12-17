using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.EXE.ChunkList;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders.Banks
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
        public void Write(ByteWriter writer)
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

}
