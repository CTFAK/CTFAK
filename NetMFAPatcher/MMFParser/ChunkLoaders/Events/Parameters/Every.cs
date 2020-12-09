using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.ChunkLoaders.Events.Parameters
{
    class Every : ParameterCommon
    {
        public int Delay;
        public int Compteur;


        public Every(ByteReader reader) : base(reader) { }
        public override void Read()
        {
            Delay = Reader.ReadInt32();
            Compteur = Reader.ReadInt32();
            
        }
        public override string ToString()
        {
            return $"Every {Delay/1000} sec";
        }
    }
}
