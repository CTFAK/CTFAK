using System.Collections.Generic;
using System.IO;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders.Banks
{
    public class FontBank : ChunkLoader
    {
        public int NumberOfItems;
        public bool Compressed;
        public bool Debug;
        public List<FontItem> Items;

        public override void Print(bool ext)
        {
            Logger.Log($"FontCount:{NumberOfItems.ToString()}");
        }

        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }

        public override void Read()
        {
            if (Debug)
            {
                //TODO
            }
            NumberOfItems = Reader.ReadInt32();
            int offset = 0;
            if (Settings.Build > 284 && !Debug) offset = -1;
            
            Items = new List<FontItem>();
            for (int i = 0; i < NumberOfItems; i++)
            {
                var item = new FontItem(Reader);
                item.Read();
                item.Handle += (uint)offset;
                Items.Add(item);
            }


        }
        public override void Write(ByteWriter writer)
        {
            writer.WriteInt32(Items.Count);
            foreach (FontItem item in Items)
            {
                item.Write(writer);
            }

        }
        public FontBank(ByteReader reader) : base(reader)
        {
        }

        public FontBank(Chunk chunk) : base(chunk)
        {
        }
    }
    public class FontItem:ChunkLoader
    {
        public bool Compressed;
        public uint Handle;
        public int Checksum;
        public int References;
        public LogFont Value;

        public FontItem(ByteReader reader) : base(reader)
        {
        }

        public FontItem(Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Handle = Reader.ReadUInt32();
            var dataReader = Decompressor.DecompressAsReader(Reader, out var decompSize);
            var currentPos = dataReader.Tell();
            Checksum = dataReader.ReadInt32();
            References = dataReader.ReadInt32();
            var size = dataReader.ReadInt32();
            Value = new LogFont(dataReader);
            Value.Read();


        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUInt32(Handle);
            var compressedWriter = new ByteWriter(new MemoryStream());
            compressedWriter.WriteInt32(Checksum);
            compressedWriter.WriteInt32(References);
            compressedWriter.WriteInt32(0);
            Value.Write(compressedWriter);
            if(Compressed) Writer.WriteBytes(Decompressor.compress_block(compressedWriter.GetBuffer()));
            else Writer.WriteWriter(compressedWriter);
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

    public class LogFont : ChunkLoader
    {
        private int _height;
        private int _width;
        private int _escapement;
        private int _orientation;
        private int _weight;
        private byte _italic;
        private byte _underline;
        private byte _strikeOut;
        private byte _charSet;
        private byte _outPrecision;
        private byte _clipPrecision;
        private byte _quality;
        private byte _pitchAndFamily;
        private string _faceName;

        public LogFont(ByteReader reader) : base(reader)
        {
        }

        public LogFont(Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            _height = Reader.ReadInt32();
            _width = Reader.ReadInt32();
            _escapement = Reader.ReadInt32();
            _orientation = Reader.ReadInt32();
            _weight = Reader.ReadInt32();
            _italic = Reader.ReadByte();
            _underline = Reader.ReadByte();
            _strikeOut = Reader.ReadByte();
            _charSet = Reader.ReadByte();
            _outPrecision = Reader.ReadByte();
            _clipPrecision = Reader.ReadByte();
            _quality = Reader.ReadByte();
            _pitchAndFamily = Reader.ReadByte();
            _faceName = Reader.ReadUniversal(32);
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt32(_height);
            Writer.WriteInt32(_width);
            Writer.WriteInt32(_escapement);
            Writer.WriteInt32(_orientation);
            Writer.WriteInt32(_weight);
            Writer.WriteInt8(_italic);
            Writer.WriteInt8(_underline);
            Writer.WriteInt8(_strikeOut);
            Writer.WriteInt8(_charSet);
            Writer.WriteInt8(_outPrecision);
            Writer.WriteInt8(_clipPrecision);
            Writer.WriteInt8(_quality);
            Writer.WriteInt8(_pitchAndFamily);
            Writer.WriteUniversal(_faceName);
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
