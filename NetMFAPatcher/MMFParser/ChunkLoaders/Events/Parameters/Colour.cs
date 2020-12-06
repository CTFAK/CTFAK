using System.Drawing;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Colour : ParameterCommon
    {
        public Color Value;

        public Colour(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            var bytes = Reader.ReadBytes(4);
            Value = Color.FromArgb(bytes[0], bytes[1], bytes[2]);           
            
        }
        
    }
}
