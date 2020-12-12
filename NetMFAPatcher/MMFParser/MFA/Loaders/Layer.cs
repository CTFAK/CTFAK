using System;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.MFA.Loaders
{
    public class Layer : DataLoader
    {
        public string Name="ERROR";
        public float XCoefficient;
        public float YCoefficient;
        public int Flags;


        public override void Write(ByteWriter Writer)
        {
            Writer.AutoWriteUnicode(Name);
            Writer.WriteInt32(Flags);
            Writer.WriteSingle(XCoefficient);
            Writer.WriteSingle(YCoefficient);
        }

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
        public Layer(ByteReader reader):base(reader)
        {
        }
    }
}
