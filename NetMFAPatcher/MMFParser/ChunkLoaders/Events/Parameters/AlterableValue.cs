using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class AlterableValue : Short
    {

        public AlterableValue(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            base.Read();           
        }
        public override string ToString()
        {
            return $"AlterableValue{Convert.ToChar(Value).ToString().ToUpper()}";
        }
    }
}
