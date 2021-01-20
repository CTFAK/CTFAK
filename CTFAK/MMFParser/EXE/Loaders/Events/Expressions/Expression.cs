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
        public int Unk1;
        public ushort Unk2;
        public Expression(ByteReader reader) : base(reader) { }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt16((short) ObjectType);
            Writer.WriteInt16((short) Num);
            if (ObjectType == 0 && Num == 0) return;
            var newWriter = new ByteWriter(new MemoryStream());
            if (ObjectType == Constants.ObjectType.System &&
                (Num == 0 || Num == 3 || Num == 23 || Num == 24 || Num == 50))
            {
                Loader.Write(newWriter);
            }
            else if ((int) ObjectType >= 2 || (int) ObjectType == -7)
            {
                newWriter.WriteInt16((short) ObjectInfo);
                newWriter.WriteInt16((short) ObjectInfoList);
                if(Num==16||Num==19)Loader.Write(newWriter);

            }

            newWriter.WriteInt32(0);
            newWriter.WriteUInt16(0);
            Writer.WriteInt16((short) ((newWriter.Size())));
            Writer.WriteWriter(newWriter);


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
            
            if (ObjectType == 0 && Num == 0) return;

            var size = Reader.ReadInt16();
            if (ObjectType == Constants.ObjectType.System)
            {
                if(Num==0) Loader=new LongExp(Reader);
                else if(Num==3) Loader= new StringExp(Reader);
                else if (Num == 23) Loader = null;
                else if (Num == 24) Loader = null;
                else if (Num == 50) Loader = null;
                else if((int)ObjectType>=2|| (int)ObjectType==-7)
                {
                    ObjectInfo = Reader.ReadUInt16();
                    ObjectInfoList = Reader.ReadInt16();
                    if (Num == 16 || Num == 19)
                    {
                        Loader = new ExtensionExp(Reader);
                    }
                }
            }
            else if((int)ObjectType>=2|| (int)ObjectType==-7)
            {
                ObjectInfo = Reader.ReadUInt16();
                ObjectInfoList = Reader.ReadInt16();
                if (Num == 16 || Num == 19)
                {
                    Loader = new ExtensionExp(Reader);
                }
            }
            Loader?.Read();
            // Unk1 = Reader.ReadInt32();
            // Unk2 = Reader.ReadUInt16();
            Reader.Seek(currentPosition+size);


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
    public class ExtensionExp:ExpressionLoader
    {
        public ExtensionExp(ByteReader reader) : base(reader)
        {
        }

        public ExtensionExp(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Value = Reader.ReadInt16();
        }

        public override void Write(ByteWriter Writer)
        {
           Writer.WriteInt16((short) Value);
        }
    }
}
