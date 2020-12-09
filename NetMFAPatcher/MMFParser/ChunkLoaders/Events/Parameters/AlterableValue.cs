using System;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders.Events.Parameters
{
    class AlterableValue : Short
    {

        public AlterableValue(ByteReader reader) : base(reader) { }
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
