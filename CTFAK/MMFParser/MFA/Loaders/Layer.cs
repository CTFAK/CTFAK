using System;
using System.Drawing;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders
{
    public class Layer : DataLoader
    {
        public string Name="ERROR";
        public float XCoefficient;
        public float YCoefficient;

        public BitDict Flags = new BitDict(new string[]
            {
                "Visible",
                "Locked",
                "Obsolete",
                "HideAtStart",
                "NoBackground",
                "WrapHorizontally",
                "WrapVertically",
                "PreviousEffect"
            }
        );

        public Color RGBCoeff=Color.White;
        public int Unk1=0;
        public int Unk2=0;


        public override void Write(ByteWriter Writer)
        {
            Writer.AutoWriteUnicode(Name);
            Writer.WriteInt32((int) Flags.flag);
            Writer.WriteSingle(XCoefficient);
            Writer.WriteSingle(YCoefficient);
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            Name = Reader.AutoReadUnicode();
            Flags.flag = (uint) Reader.ReadInt32();
            XCoefficient = Reader.ReadSingle();
            YCoefficient = Reader.ReadSingle();
            
  



        }
        public Layer(ByteReader reader):base(reader)
        {
        }
    }
}
