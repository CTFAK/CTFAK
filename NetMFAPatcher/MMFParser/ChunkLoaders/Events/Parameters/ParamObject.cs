using NetMFAPatcher.Utils;

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
            ObjectInfoList = Reader.ReadInt16();
            ObjectInfo = Reader.ReadUInt16();
            ObjectType = Reader.ReadInt16();           
        }
        public override string ToString()
        {
            return $"Object {ObjectInfoList} {ObjectInfo} {ObjectType}";
        }
    }
}
