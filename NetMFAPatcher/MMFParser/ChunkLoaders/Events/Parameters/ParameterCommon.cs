using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class ParameterCommon : DataLoader
    {
        

        public ParameterCommon(ByteIO reader) : base(reader) { }
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {

            
        }
    }
}
