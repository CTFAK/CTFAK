using System;
using System.IO;
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
        public DataLoader Loader;
        public Expression(ByteReader reader) : base(reader) { }
        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt16((short) ObjectType);
            Writer.WriteInt16((short) Num);
            if (ObjectType == 0 && Num == 0) return;
            var dataWriter = new ByteWriter(new MemoryStream());
            if(ObjectType==Constants.ObjectType.System)
            {
                Logger.Log("WRITING  EXPRESSION "+Num);
                    if (Loader != null)
                    {
                        Loader.Write(dataWriter);  
                        dataWriter.WriteInt32(0);
                        dataWriter.WriteInt16(0);
                        
                    }
                    else if ((int) ObjectType >= 2 || (int) ObjectType == 7)
                    {
                        Writer.WriteInt16((short) ObjectInfo);
                        Writer.WriteInt16((short) ObjectInfoList);
                        if (Num == 16 || Num == 19)
                        {
                            Writer.WriteInt32((short) value);
                        }
                    }
            }
            
            
       
            Writer.WriteUInt16((ushort) (dataWriter.Size()));
            Writer.WriteWriter(dataWriter);

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
            if(ObjectType==Constants.ObjectType.System)
            {
                switch (Num)
                    {
                        //Long
                        case 0:
                        {
                            Loader = new LongExp(Reader);
                            break;
                        }
                        //String
                        case 3:
                        {
                            Loader = new StringExp(Reader);
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
                        case 16:
                            // value = Reader.ReadInt16();
                            break;
                        case 19:
                            // value = Reader.ReadInt16();
                            break;
                        default:
                        {
                            if ((int)ObjectType >= 2 || (int)ObjectType == 7)
                            {
                                ObjectInfo = Reader.ReadUInt16();
                                ObjectInfoList = Reader.ReadUInt16();
                                if (Num == 16 || Num == 19)
                                {
                                    value = Reader.ReadInt16();
                                }
                            }
                            break;
                        }
                    }
                    Logger.Log("Reading Expression: "+Num);
                    Loader?.Read();
            }
            
            // Reader.Seek(currentPosition+size);

        }

        public override string ToString()
        {
            return $"Expression {ObjectType}=={Num}: {((ExpressionLoader)Loader)?.Value}";
        }
    }
    public class ExpressionLoader:DataLoader
    {
        public object Value;
        public ExpressionLoader(ByteReader reader) : base(reader)
        {
        }

        public ExpressionLoader(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            throw new NotImplementedException();
        }

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }
    }

    public class StringExp:ExpressionLoader
    {
        

        public StringExp(ByteReader reader) : base(reader)
        {
        }

        public StringExp(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Value = Reader.ReadWideString();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUnicode((string) Value,true);
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }
    }
    public class LongExp:ExpressionLoader
    {
        public int Val1;

        public LongExp(ByteReader reader) : base(reader)
        {
        }

        public LongExp(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Value = Reader.ReadInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32((int) Value);
        }
    }
}
