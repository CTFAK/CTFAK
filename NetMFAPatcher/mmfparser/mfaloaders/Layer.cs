using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders
{
    class Layer : DataLoader
    {
        public string name="ERROR";
        public float xCoefficient;
        public float yCoefficient;
        public int flags;


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            name = reader.ReadAscii(reader.ReadInt32());
            flags = reader.ReadInt32();
            xCoefficient = reader.ReadSingle();
            yCoefficient = reader.ReadSingle();



        }
        public Layer(ByteIO reader):base(reader)
        {
        }
    }
}
