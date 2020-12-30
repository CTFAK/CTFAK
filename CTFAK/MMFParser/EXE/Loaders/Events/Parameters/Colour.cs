using System.Drawing;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    class Colour : ParameterCommon
    {
        public Color Value;

        public Colour(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            var bytes = Reader.ReadBytes(4);
            Value = Color.FromArgb(bytes[0], bytes[1], bytes[2]);           
            
        }
        
    }
}
