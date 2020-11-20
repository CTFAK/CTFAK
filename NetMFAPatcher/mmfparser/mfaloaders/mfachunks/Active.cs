using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders.mfachunks
{
    class Active : AnimationObject
    {
        public override void Print()
        {
            base.Print();
            
        }

        public override void Read()
        {
            base.Read();
        }
        public Active(ByteIO reader) : base(reader) { }
    }
}
