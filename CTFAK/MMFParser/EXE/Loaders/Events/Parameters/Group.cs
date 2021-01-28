using System;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class Group:ParameterCommon
    {
        public long Offset;
        public ushort Flags;
        public ushort Id;
        public string Name;
        public int Password;
        private short _unk;

        public Group(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            Offset = Reader.Tell() - 24;
            Flags = Reader.ReadUInt16();
            Id = Reader.ReadUInt16();
            Name = Reader.ReadWideString();
            Reader.Skip(178);
            Password = Reader.ReadInt32();
            _unk = Reader.ReadInt16();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUInt16(Flags);
            Writer.WriteUInt16(Id);
            Writer.WriteUnicode(Name,true);
            Writer.Skip(178);
            Writer.WriteInt32(Password);
            Writer.WriteInt16(_unk);
            
        }

        public override string ToString()
        {
            return $"Group: {Name}";
        }
    }
}