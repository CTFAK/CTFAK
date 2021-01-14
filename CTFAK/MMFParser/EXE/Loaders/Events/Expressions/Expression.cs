using System;
using System.Windows.Forms.VisualStyles;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Expressions
{
    public class Expression : DataLoader
    {
        public Constants.ObjectType ObjectType;
        public int Num;
        public int ObjectInfo;
        public int ObjectInfoList;
        public object value;
        public object floatValue;
        public Expression(ByteReader reader) : base(reader) { }
        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            ObjectType = (Constants.ObjectType) Reader.ReadInt16();
            Num = Reader.ReadInt16();
            if (ObjectType == 0 & Num == 0) return;
            var size = Reader.ReadUInt16();
            switch (ObjectType)
            {
                case Constants.ObjectType.System:
                    switch (Num)
                    {
                        //Long
                        case 0:
                        {
                            value = Reader.ReadInt32();
                            break;
                        }
                        //String
                        case 3:
                        {
                            value = Reader.ReadWideString();
                            break;
                        }
                        //Double
                        case 23:
                        {
                            value = Reader.ReadDouble();
                            floatValue = Reader.ReadSingle();
                            break;
                        }
                        //GlobalString
                        case 24:
                            break;
                        //GlobalValue
                        case 50:
                            break;
                        default:
                        {
                            if ((int)ObjectType >= 2 || (int)ObjectType == 7)
                            {
                                ObjectInfo = Reader.ReadUInt16();
                                ObjectInfoList = Reader.ReadUInt16();
                                // if self.num in extensionLoaders:
                                    // loader = extensionLoaders[self.num]
                                    // self.loader = self.new(loader, reader)
                            }

                            break;
                        }
                            
                    }

                    break;
            }
            Reader.Seek(currentPosition+size);

        }

        public override string ToString()
        {
            return $"Expression {ObjectType}=={Num}: {value}";
        }
    }
}
