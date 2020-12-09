using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFALoaders.mfachunks
{
    class Active : AnimationObject
    {
        public override void Print()
        {
            base.Print();
            
        }

        public override void Read()
        {
            base.Read();
        }
        public Active(ByteReader reader) : base(reader) { }
    }
}
