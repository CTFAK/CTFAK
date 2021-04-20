using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.Utils;

namespace CTFAK_Runtime_Tools.RuntimeParsers.RuntimeChunks
{
    public class RuntimeFrame:Frame
    {
        public RuntimeFrame(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            base.Read();
        }
    }
}