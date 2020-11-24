using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class ParamObject : ParameterCommon
    {
        public int ObjectInfoList;
        public int ObjectInfo;
        public int ObjectType;
        public ParamObject(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            ObjectInfoList = reader.ReadInt16();
            ObjectInfo = reader.ReadUInt16();
            ObjectType = reader.ReadInt16();           
        }
        public override string ToString()
        {
            return $"Object {ObjectInfoList} {ObjectInfo} {ObjectType}";
        }
    }
}
