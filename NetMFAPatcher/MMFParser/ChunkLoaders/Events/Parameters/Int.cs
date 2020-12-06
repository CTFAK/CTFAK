using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Int : Short
    {


        public Int(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            Value = (short)Reader.ReadInt32();          
        }
        
    }
}
