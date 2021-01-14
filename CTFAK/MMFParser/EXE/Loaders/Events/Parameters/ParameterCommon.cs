using System;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class ParameterCommon : DataLoader
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
