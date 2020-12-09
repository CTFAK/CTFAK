using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders.Events.Parameters
{
    class Int : Short
    {


        public Int(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            Value = (short)Reader.ReadInt32();          
        }
        
    }
}
