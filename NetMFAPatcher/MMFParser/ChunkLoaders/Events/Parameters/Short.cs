using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Short : ParameterCommon
    {
        public short value;

        public Short(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            value = reader.ReadInt16();
            
        }
        public override string ToString()
        {
            return $"{this.GetType().Name} value: {value}";
        }
    }
}
