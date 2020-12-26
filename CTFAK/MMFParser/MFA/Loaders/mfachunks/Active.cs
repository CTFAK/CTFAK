using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders.mfachunks
{
    public class Active : AnimationObject
    {
        public override void Print()
        {
            base.Print();
            
        }

        public override void Read()
        {
            base.Read();
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
        }

        public Active(ByteReader reader) : base(reader) { }
    }
}
