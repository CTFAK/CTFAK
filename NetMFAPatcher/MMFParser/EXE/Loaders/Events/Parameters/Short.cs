using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders.Events.Parameters
{
    class Short : ParameterCommon
    {
        public short Value;

        public Short(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            Value = Reader.ReadInt16();
            
        }
        public override string ToString()
        {
            return $"{this.GetType().Name} value: {Value}";
        }
    }
}
