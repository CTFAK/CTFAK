using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetMFAPatcher.mmfparser.Constants;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Expressions
{
    class Expression : DataLoader
    {
        public ObjectType ObjectType;
        public int num;
        public int ObjectInfo;
        public int ObjectInfoList;
        public Expression(ByteIO reader) : base(reader) { }
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            
        }
    }
}
