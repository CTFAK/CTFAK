using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders.mfachunks
{
    public class ExtensionObject:AnimationObject
    {
        public string ExtensionName;
        public int ExtensionType;
        public string Filename;
        public uint Magic;
        public string SubType;
        public int ExtensionVersion;
        public int ExtensionId;
        public int ExtensionPrivate;
        public byte[] ExtensionData;
        private byte[] _unk;

        public ExtensionObject(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            base.Read();
            ExtensionType = Reader.ReadInt32();
            if (ExtensionType == -1)
            {
                ExtensionName = Reader.AutoReadUnicode();
                Filename = Reader.AutoReadUnicode();
                Magic = Reader.ReadUInt32();
                SubType = Reader.AutoReadUnicode();
            }

            var newReader = new ByteReader(Reader.ReadBytes((int) Reader.ReadUInt32()));
            var dataSize = newReader.ReadInt32() - 20;
            _unk = newReader.ReadBytes(4);
            ExtensionVersion = newReader.ReadInt32();
            ExtensionId = newReader.ReadInt32();
            ExtensionPrivate = newReader.ReadInt32();
            ExtensionData = newReader.ReadBytes(dataSize);

        }

        public override void Write(ByteWriter Writer)
        {
            _isExt = true;
            base.Write(Writer);
            if (ExtensionType == -1)
            {
                Writer.AutoWriteUnicode(ExtensionName);
                Writer.AutoWriteUnicode(Filename);
                Writer.WriteUInt32(Magic);
                Writer.AutoWriteUnicode(SubType);
            }
            Writer.WriteInt32(ExtensionData.Length+20);
            Writer.WriteInt32(ExtensionData.Length+20);
            Writer.WriteInt32(-1);
            Writer.WriteInt32(ExtensionVersion);
            Writer.WriteInt32(ExtensionId);
            Writer.WriteInt32(ExtensionPrivate);
            Writer.WriteBytes(ExtensionData);
            

        }
    }
}