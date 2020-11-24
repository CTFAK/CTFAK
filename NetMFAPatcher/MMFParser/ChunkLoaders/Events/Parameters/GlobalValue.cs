using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class GlobalValue : Short
    {


        public GlobalValue(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            base.Read();           
        }
        public override string ToString()
        {
            if(value>26) return $"GlobalValue{value}";
            return $"GlobalValue{Convert.ToChar(value).ToString().ToUpper()}";
        }
    }
}
