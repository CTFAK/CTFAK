using System;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    class AlterableValue : Short
    {
        public int Unk;

        public AlterableValue(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            base.Read();

        }

        public override void Write(ByteWriter Writer)
        {
            Value = 5;
            base.Write(Writer);

        }

        public override string ToString()
        {
            return $"AlterableValue{Value.ToString().ToUpper()}";
        }
    }
}
