using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.MFALoaders.mfachunks
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
        public Active(ByteIO reader) : base(reader) { }
    }
}
