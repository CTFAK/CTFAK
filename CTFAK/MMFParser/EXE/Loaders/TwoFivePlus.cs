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
            Settings.GameType = GameType.TwoFivePlus;
            var start = Reader.Tell();
            var end = start + Reader.Size();
            
            Headers = new Dictionary<int, ObjectHeader>();
            int current = 0;
            while (Reader.Tell() < end)
            {
                var prop = new ObjectHeader(Reader);
                Logger.Log($"Reading object header: {current}");
                prop.Read();
                var chunkSize = Reader.ReadInt32();

                Headers.Add(current, prop);
                current++;
            }

        }

        public override void Write(ByteWriter Writer)
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
        public Dictionary<int, ObjectProperties> Props;
        public ObjectPropertyList(ByteReader reader) : base(reader)
        {
        }

    

        public override void Read()
        {
            
            var start = Reader.Tell();
            
            var end = start + Reader.Size();
            var chunkSize = Reader.ReadInt32();
            Props = new Dictionary<int, ObjectProperties>();
            int current = 0;
            while (Reader.Tell() < end)
            {
                var prop = new ObjectProperties(Reader);
                prop.ReadNew(2, null);
                Logger.Log($"Reading object prop: {current}");
                Props.Add(current, prop);
                current++;
            }
        }

        public override void Write(ByteWriter Writer)
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
                var name =Settings.GameType == GameType.NSwitch ? Reader.ReadAscii(): Reader.ReadWideString();
                Names.Add(current,name);
                current++;
            }

        }
        public ObjectNames(ByteReader reader) : base(reader) { }
    }
}