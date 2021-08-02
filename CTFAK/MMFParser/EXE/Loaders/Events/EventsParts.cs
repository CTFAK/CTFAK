using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CTFAK.MMFParser.EXE.Loaders.Events.Parameters;
using CTFAK.MMFParser.EXE.Loaders.Events.Viewer;
using CTFAK.Utils;
using static CTFAK.Settings;

namespace CTFAK.MMFParser.EXE.Loaders.Events
{
    public class Condition : DataLoader
    {
        public int Flags;
        public int OtherFlags;
        public int DefType;
        public int NumberOfParameters;
        public int ObjectType;
        public int Num;
        public int ObjectInfo;
        public int Identifier;
        public int ObjectInfoList;
        public List<Parameter> Items = new List<Parameter>();

        public Condition(ByteReader reader) : base(reader) { }
        public override void Write(ByteWriter Writer)
        {
            ByteWriter newWriter = new ByteWriter(new MemoryStream());
            //if (Settings.GameType == GameType.TwoFivePlus) Logger.Log($"{ObjectType}-{Num}-{ObjectInfo}-{ObjectInfoList}-{Flags}-{OtherFlags}-{Items.Count}-{DefType}-{Identifier}");
            newWriter.WriteInt16((short) ObjectType);
            newWriter.WriteInt16((short) Num);
            newWriter.WriteUInt16((ushort) ObjectInfo);
            newWriter.WriteInt16((short) ObjectInfoList);
            newWriter.WriteUInt8((sbyte) Flags);
            newWriter.WriteUInt8((sbyte) OtherFlags);
            newWriter.WriteUInt8((sbyte) Items.Count);
            newWriter.WriteInt8((byte) DefType);
            newWriter.WriteUInt16((ushort) (Identifier));
            foreach (Parameter parameter in Items)
            {
                if (parameter.ToString() != "ERROR!") parameter.Write(newWriter);
            }
            Writer.WriteInt16((short) (newWriter.BaseStream.Position+2));
            Writer.WriteWriter(newWriter);


        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            if (Reader.Tell() > Reader.Size() - 12)
            {
                Console.WriteLine("E58:  Ran out of bytes reading EventParts ("+Reader.Tell()+"/"+Reader.Size()+")");
                return; //really hacky shit, but it works
            }
            var old =Old&&!Settings.DoMFA;
            var currentPosition = Reader.Tell();
            var size = Reader.ReadUInt16();

            ObjectType = old ? Reader.ReadSByte(): Reader.ReadInt16();
            Num = old ? Reader.ReadSByte(): Reader.ReadInt16();
            if ((int) ObjectType > 2 && Num <48)
            {
                if(old)Num -= 32;
            }
            ObjectInfo = Reader.ReadUInt16();
            ObjectInfoList = Reader.ReadInt16();
            Flags = Reader.ReadSByte();
            OtherFlags = Reader.ReadSByte();
            NumberOfParameters = Reader.ReadByte();
            DefType = Reader.ReadByte();
            Identifier = Reader.ReadInt16();
            //if (Settings.GameType == GameType.TwoFivePlus) Console.WriteLine("number of parameters: " + NumberOfParameters);
            for (int i = 0; i < NumberOfParameters; i++)
            {
                var item = new Parameter(Reader);
                item.Read();
                Items.Add(item);
            }
            if (Settings.GameType == GameType.TwoFivePlus) Logger.Log(this);
            

            
        }
        public override string ToString()
        {
            //return Preprocessor.ProcessCondition(this);
            //return $"Condition {(Constants.ObjectType)ObjectType}=={Names.ConditionNames[ObjectType][Num]}{(Items.Count > 0 ? "-"+Items[0].ToString() : " ")}";
            return $"Condition {(Constants.ObjectType)ObjectType}=={Num}{(Items.Count > 0 ? "-"+Items[0].ToString() : " ")}";
        }
    }

    public class Action : DataLoader
    {
        public int Flags;
        public int OtherFlags;
        public int DefType;
        public int ObjectType;
        public int Num;
        public int ObjectInfo;
        public int ObjectInfoList;
        public List<Parameter> Items = new List<Parameter>();
        public byte NumberOfParameters;
        public Action(ByteReader reader) : base(reader) { }
        public override void Write(ByteWriter Writer)
        {
            ByteWriter newWriter = new ByteWriter(new MemoryStream());
            newWriter.WriteInt16((short) ObjectType);
            newWriter.WriteInt16((short) Num);
            newWriter.WriteUInt16((ushort) ObjectInfo);
            newWriter.WriteInt16((short) ObjectInfoList);
            newWriter.WriteUInt8((sbyte) Flags);
            newWriter.WriteUInt8((sbyte) OtherFlags);
            newWriter.WriteUInt8((sbyte) Items.Count);
            newWriter.WriteInt8((byte) DefType);

            foreach (Parameter parameter in Items)
            {
                if (parameter.ToString() == "ERROR!") Console.WriteLine("Error in parameter: "+parameter);
                if (parameter.ToString() != "ERROR!") parameter.Write(newWriter);
            }
            Writer.WriteUInt16((ushort) (newWriter.BaseStream.Position+2));
            Writer.WriteWriter(newWriter);
            
        }

        public override void Print( )
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            if (Reader.Tell() > Reader.Size() - 12)
            {
                Console.WriteLine("E141: Ran out of bytes reading EventParts (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
            var old = Settings.GameType == GameType.OnePointFive&&!Settings.DoMFA;
            var currentPosition = Reader.Tell();
            var size = Reader.ReadUInt16();
            ObjectType =  old ? Reader.ReadSByte(): Reader.ReadInt16();
            Num = old ? Reader.ReadSByte(): Reader.ReadInt16();
            if ((int) ObjectType >= 2 && Num >= 48)
            {
                if(old)Num += 32;
            }
            ObjectInfo = Reader.ReadUInt16();
            ObjectInfoList = Reader.ReadInt16();
            Flags = Reader.ReadSByte();
            OtherFlags = Reader.ReadSByte();
            NumberOfParameters = Reader.ReadByte();
            DefType = Reader.ReadByte();
            for (int i = 0; i < NumberOfParameters; i++)
            {
                var item = new Parameter(Reader);
                item.Read();
                Items.Add(item);
            }
            //Logger.Log(this);

        }
        public override string ToString()
        {
            
            return $"Action {ObjectType}-{Num}{(Items.Count > 0 ? "-"+Items[0].ToString() : " ")}";

        }
    }

    public class Parameter : DataLoader
    {
        public int Code;
        public DataLoader Loader;

        public Parameter(ByteReader reader) : base(reader) { }

        public override void Write(ByteWriter Writer)
        {
            var newWriter = new ByteWriter(new MemoryStream());
            if (Code == 0) return;
            newWriter.WriteInt16((short) Code);
            if (newWriter == null) return;
            Loader.Write(newWriter);
            Writer.WriteUInt16((ushort) (newWriter.BaseStream.Position+2));
            Writer.WriteWriter(newWriter);
            
            
        }

        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            if (Reader.Tell() > Reader.Size() - 12)
            {
                Console.WriteLine("E205: Ran out of bytes reading EventParts (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
            var currentPosition = Reader.Tell();
            var size = Reader.ReadInt16();
            Code = Reader.ReadInt16();

            var actualLoader = Helper.LoadParameter(Code,Reader);
            this.Loader = actualLoader;
            // Loader?.Read();
            if (Loader != null) Loader.Read();
            else return; //throw new Exception("Loader is null: "+Code);
            if (currentPosition + size < 0)
            {
                Console.WriteLine("E219: Ran out of bytes reading EventParts (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
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
            else return "String: ERROR!"; 
            //else throw new Exception($"Unkown Parameter: {Code} ");
        }
    }

}
