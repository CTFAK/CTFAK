using System;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.MFALoaders
{
    class Layer : DataLoader
    {
        public string Name="ERROR";
        public float XCoefficient;
        public float YCoefficient;
        public int Flags;


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Helper.AutoReadUnicode(Reader);
            Flags = Reader.ReadInt32();
            XCoefficient = Reader.ReadSingle();
            YCoefficient = Reader.ReadSingle();



        }
        public Layer(ByteIO reader):base(reader)
        {
        }
    }
}
