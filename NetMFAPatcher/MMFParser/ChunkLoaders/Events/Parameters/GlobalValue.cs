using System;
using NetMFAPatcher.Utils;

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
            if(Value>26) return $"GlobalValue{Value}";
            return $"GlobalValue{Convert.ToChar(Value).ToString().ToUpper()}";
        }
    }
}
