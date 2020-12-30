using System;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
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
