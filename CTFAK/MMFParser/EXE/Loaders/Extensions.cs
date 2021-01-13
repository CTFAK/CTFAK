using System.Collections.Generic;
using System.Runtime.InteropServices;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class Extensions:ChunkLoader
    {
        internal ushort PreloadExtensions;
        public List<Extension> Items;

        public Extensions(ByteReader reader) : base(reader)
        {
        }

        public Extensions(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var count = Reader.ReadUInt16();
            PreloadExtensions = Reader.ReadUInt16();
            Items = new List<Extension>();
            for (int i = 0; i < count; i++)
            {
                var ext = new Extension(Reader);
                ext.Read();
                Items.Add(ext);
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
    public class Extension:ChunkLoader
    {
        public short Handle;
        public int MagicNumber;
        public int VersionLs;
        public int VersionMs;
        public string Name;
        public string Ext;
        public string SubType;

        public Extension(ByteReader reader) : base(reader)
        {
        }

        public Extension(ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            var currentPosition = Reader.Tell();
            var size = Reader.ReadInt16();
            if (size < 0)
            {
                size = (short) (size*-1);
            }

            Handle = Reader.ReadInt16();
            MagicNumber = Reader.ReadInt32();
            VersionLs = Reader.ReadInt32();
            VersionMs = Reader.ReadInt32();
            var arr = Reader.ReadWideString().Split('.');
            Name = arr[0];
            Logger.Log("Found Extension: "+Name+" with id "+Handle);
            Ext = arr[1];
            SubType = Reader.ReadWideString();
            Reader.Seek(currentPosition+size);
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