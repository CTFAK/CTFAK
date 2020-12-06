using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Banks
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
            //i am testing with no fonts suck pinus haha

        }
        public FontBank(ByteIO reader) : base(reader)
        {
        }

        public FontBank(Chunk chunk) : base(chunk)
        {
        }
    }

}
