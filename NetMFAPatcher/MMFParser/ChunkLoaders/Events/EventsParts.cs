using NetMFAPatcher.MMFParser.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.Utils;


namespace NetMFAPatcher.MMFParser.ChunkLoaders.Events
{
    public class Condition : DataLoader
    {
        public int Flags;
        public int OtherFlags;
        public int DefType;
        public int NumberOfParameters;
        public Constants.ObjectType ObjectType;
        public int Num;
        public int ObjectInfo;
        public int Identifier;
        public int ObjectInfoList;
        public List<Parameter> Items = new List<Parameter>();
        public Condition(ByteReader reader) : base(reader) { }
        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            var size = Reader.ReadUInt16();
            ObjectType = (Constants.ObjectType)Reader.ReadInt16();
            Num = Reader.ReadInt16();
            ObjectInfo = Reader.ReadUInt16();
            ObjectInfoList = Reader.ReadInt16();
            Flags = Reader.ReadSByte();
            OtherFlags = Reader.ReadSByte();
            NumberOfParameters = Reader.ReadByte();
            DefType = Reader.ReadByte();
            Identifier = Reader.ReadInt16();
            for (int i = 0; i < NumberOfParameters; i++)
            {
                var item = new Parameter(Reader);
                item.Read();
                Items.Add(item);
            }
            Reader.Seek(currentPosition + size);
            

            
        }
        public override string ToString()
        {
            return $"Condition {ObjectType}-{Num}-{(Items.Count > 0 ? Items[0].ToString() : "cock")}";

        }
    }

    public class Action : DataLoader
    {
        public int Flags;
        public int OtherFlags;
        public int DefType;
        public Constants.ObjectType ObjectType;
        public int Num;
        public int ObjectInfo;
        public int ObjectInfoList;
        public List<Parameter> Items = new List<Parameter>();
        public Action(ByteReader reader) : base(reader) { }
        public override void Print( )
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            var size = Reader.ReadUInt16();
            ObjectType = (Constants.ObjectType)Reader.ReadInt16();
            Num = Reader.ReadInt16();
            ObjectInfo = Reader.ReadUInt16();
            ObjectInfoList = Reader.ReadInt16();
            Flags = Reader.ReadSByte();
            OtherFlags = Reader.ReadSByte();
            var numberOfParameters=Reader.ReadByte();
            DefType = Reader.ReadByte();
            for (int i = 0; i < DefType; i++)
            {
                var item = new Parameter(Reader);
                item.Read();
                Items.Add(item);
            }


        }
        public override string ToString()
        {
            
            return $"Action {ObjectType}-{Num}-{(Items.Count>0?Items[0].ToString():"cock")}";

        }
    }

    public class Parameter : DataLoader
    {
        public int Code;
        public DataLoader Loader;

        public Parameter(ByteReader reader) : base(reader) { }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            var size = Reader.ReadInt16();
            Code = Reader.ReadInt16();


            var actualLoader = Helper.LoadParameter(Code,Reader);
            this.Loader = actualLoader;
            if (Loader!=null)
            {
                
                Loader.Read();
            }
            else
            {
                //throw new Exception("Loader is null");
            }
            Reader.Seek(currentPosition+size);

        }
        public object Value
        {
            get
            {
                if (Loader != null)
                {


                    if (Loader.GetType().GetField("value") != null)
                    {
                        return Loader.GetType().GetField("value").GetValue(Loader);
                    }
                    else
                    {
                        return null;
                    }
                }
                else return null;
            }
        }
        public override string ToString()
        {
            if (Loader != null) return Loader.ToString();
            else return "UNK-PARAMETER";

        }
    }

}
