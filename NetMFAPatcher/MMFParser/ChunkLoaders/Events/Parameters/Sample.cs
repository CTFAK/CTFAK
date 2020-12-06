using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events.Parameters
{
    class Sample : ParameterCommon
    {
        public int Handle;
        public string Name;
        public int Flags;

        public Sample(ByteIO reader) : base(reader) { }
        public override void Read()
        {
            Handle = Reader.ReadInt16();
            Flags = Reader.ReadUInt16();
            Name = Reader.ReadWideString();
        }
        public override string ToString()
        {
            return $"Sample '{Name}' handle: {Handle}";
        }
    }
}
