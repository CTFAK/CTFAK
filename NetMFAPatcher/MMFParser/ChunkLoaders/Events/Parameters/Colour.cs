using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Colour : ParameterCommon
    {
        public Color value;

        public Colour(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            var bytes = reader.ReadBytes(4);
            value = Color.FromArgb(bytes[0], bytes[1], bytes[2]);           
            
        }
        
    }
}
