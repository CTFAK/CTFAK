using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.ChunkLoaders.Objects
{
    public class AlterableValues:ChunkLoader
    {
        public AlterableValues(ByteReader reader) : base(reader)
        {
        }

        public AlterableValues(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var items = new List<int>();

            var count = Reader.ReadUInt16();
            Console.WriteLine(count);
            for (int i = 0; i < count; i++)
            {
                var item = Reader.ReadInt32();
                
                items.Add(item);
                Console.WriteLine("Found Value: "+item);
                
            }
        }

        public override void Print(bool ext)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
    public class AlterableStrings:ChunkLoader
    {
        public AlterableStrings(ByteReader reader) : base(reader)
        {
        }

        public AlterableStrings(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var items = new List<string>();

            var count = Reader.ReadUInt16();

            for (int i = 0; i < count; i++)
            {
                var item = Reader.ReadWideString();
                items.Add(item);
                Console.WriteLine("Found String: "+item);
                
            }
        }

        public override void Print(bool ext)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }
    }
}