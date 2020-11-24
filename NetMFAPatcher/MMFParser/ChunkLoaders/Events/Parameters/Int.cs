using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Int : Short
    {


        public Int(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            value = (short)reader.ReadInt32();          
        }
        
    }
}
