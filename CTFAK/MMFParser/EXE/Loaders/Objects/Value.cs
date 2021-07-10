using System;
using System.Collections.Generic;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Objects
{
    public class AlterableValues:ChunkLoader
    {
        public List<int> Items;

        public AlterableValues(ByteReader reader) : base(reader)
        {
        }

       

        public override void Read()
        {
            if (Settings.GameType == GameType.TwoFivePlus) Reader.Skip(-4);
            Items = new List<int>();
            Reader.ReadUInt16();
            var count = Reader.ReadUInt16();
            Logger.Log("Alterable value count "+count);
            Console.WriteLine(count);
            for (int i = 0; i < count; i++)
            {
                
                var item = Reader.ReadInt32();
                Logger.Log($"{i} - {item}");
                
                Items.Add(item);

            }
        }

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }


        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
    public class AlterableStrings:ChunkLoader
    {
        public List<string> Items;

        public AlterableStrings(ByteReader reader) : base(reader)
        {
        }

       

        public override void Read()
        {
            if (Settings.GameType == GameType.TwoFivePlus) Reader.Skip(-4);
            Items = new List<string>();

            var count = Reader.ReadUInt16();
            Logger.Log("Alterable string count "+count);

            for (int i = 0; i < count; i++)
            {
                var item = Reader.ReadWideString();
                Items.Add(item);

            }
        }

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }
        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
}