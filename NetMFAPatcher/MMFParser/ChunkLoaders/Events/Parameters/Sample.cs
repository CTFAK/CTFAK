using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Sample : ParameterCommon
    {
        public int handle;
        public string name;
        public int flags;

        public Sample(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            handle = reader.ReadInt16();
            flags = reader.ReadUInt16();
            name = reader.ReadWideString();
        }
        public override string ToString()
        {
            return $"Sample '{name}' handle: {handle}";
        }
    }
}
