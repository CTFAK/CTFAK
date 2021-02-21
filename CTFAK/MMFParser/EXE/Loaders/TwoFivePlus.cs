using System;
using System.Collections.Generic;
using System.Linq;
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
            return new[]
            {
                "Count: "+Headers.Count
            };
        }
    }
    public class ObjectPropertyList:ChunkLoader
    {
        public ObjectPropertyList(ByteReader reader) : base(reader)
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
    class ObjectNames : ChunkLoader//2.5+ trash
    {
        public Dictionary<int, string> Names;

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }

        public override void Print(bool ext){}
        

        public override string[] GetReadableData()
        {
            return new[]
            {
                "Count: "+Names.Count
            };
        }

        public override void Read()
        {
            var start = Reader.Tell();
            var end = start + Reader.Size();
            Names = new Dictionary<int,string>();
            int current = 0;
            while(Reader.Tell() < end)
            {
                var name = Reader.ReadWideString();
                Names.Add(current,name);
                current++;
            }

        }
        public ObjectNames(ByteReader reader) : base(reader) { }
    }
}