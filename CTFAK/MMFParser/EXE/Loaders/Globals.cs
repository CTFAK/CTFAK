using System;
using System.Collections.Generic;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class GlobalValues : ChunkLoader
    {
        public List<float> Items = new List<float>();
        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }


        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var numberOfItems = Reader.ReadUInt16();
            var templist = new List<ByteReader>();
            for (int i = 0; i < numberOfItems; i++)
            {
                templist.Add(new ByteReader(Reader.ReadBytes(4)));
            }
            foreach (var item in templist)
            {
                if (Reader.Tell() > Reader.Size() - 1)
                {
                    Console.WriteLine("E34:  Ran out of bytes reading Globals (" + Reader.Tell() + "/" + Reader.Size() + ")");
                    return; //really hacky shit, but it works
                }
                var globalType = Reader.ReadSByte();
                float newGlobal = 0f;
                if((Constants.ValueType)globalType==Constants.ValueType.Float)
                {
                    newGlobal = item.ReadSingle();
                }
                else if ((Constants.ValueType)globalType == Constants.ValueType.Int)
                {
                    newGlobal = item.ReadInt32();
                }
                else
                {
                    throw new Exception("unknown global type");
                }
                Items.Add(newGlobal);               
            }

            
        }

        public GlobalValues(ByteReader reader) : base(reader)
        {
        }
    }
    public class GlobalStrings : ChunkLoader
    {
        public List<string> Items = new List<string>();
        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }



        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            if (Reader.Tell() > Reader.Size() - 10)
            {
                Console.WriteLine("E80:  Ran out of bytes reading Globals (" + Reader.Tell() + "/" + Reader.Size() + ")");
                return; //really hacky shit, but it works
            }
            var count = Reader.ReadUInt32();
            for (int i = 0; i < count; i++)
            {
                Items.Add(Reader.ReadAscii());
            }

        }

        public GlobalStrings(ByteReader reader) : base(reader)
        {
        }
    }
}
