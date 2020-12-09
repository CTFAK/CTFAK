using System;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders.Events.Parameters
{
    class ParameterCommon : DataLoader
    {
        

        public ParameterCommon(ByteReader reader) : base(reader) { }
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {

            
        }
    }
}
