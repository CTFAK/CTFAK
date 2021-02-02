using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using CTFAK.MMFParser.EXE.Loaders.Objects;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class ObjectHeaders:ChunkLoader
    {
        public Dictionary<int, ObjectHeader> Headers;

        public ObjectHeaders(ByteReader reader) : base(reader)
        {
        }

        public ObjectHeaders(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Headers = new Dictionary<int,ObjectHeader>();
            int current = 0;
            while (Reader.Tell()<Reader.Size())
            {
                var newh = new ObjectHeader(Reader);
                newh.Read();
                Headers.Add(current,newh);
                current++;

            }
        }

        public override void Write(ByteWriter Writer)
        {
            throw new System.NotImplementedException();
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
    public class ObjectPropertyList:ChunkLoader
    {
        public ObjectPropertyList(ByteReader reader) : base(reader)
        {
        }

        public ObjectPropertyList(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            
        }

        public override void Write(ByteWriter Writer)
        {
            throw new System.NotImplementedException();
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