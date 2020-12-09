using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders.Events.Parameters
{
    class Float : ParameterCommon
    {
        public float Value;

        public Float(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            Value = Reader.ReadSingle();
           
        }
        public override string ToString()
        {
            return $"{this.GetType().Name} value: {Value}";
        }
    }
}
