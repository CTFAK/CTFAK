using System;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders.Events.Parameters
{
    class ParameterCommon : DataLoader
    {
        

        public ParameterCommon(ByteReader reader) : base(reader) { }
        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {

            
        }
    }
}
