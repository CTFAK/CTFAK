using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Create : ParameterCommon
    {
        public int ObjectInstances;
        public int ObjectInfo;
        public Position Position;

        public Create(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            Position = new Position(Reader);
            Position.Read();
            ObjectInstances = Reader.ReadUInt16();
            ObjectInfo = Reader.ReadUInt16();
            
            
        }
        public override string ToString()
        {
            return $"Create obj instance:{ObjectInstances} info:{ObjectInfo} pos:({Position.ToString()})";
        }
    }
}
