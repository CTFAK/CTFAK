using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.utils;

namespace NetMFAPatcher.mmfparser.mfaloaders
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
