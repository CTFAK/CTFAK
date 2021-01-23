using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.MFA.Loaders.mfachunks
{
    public class Text:ObjectLoader
    {
        public List<Paragraph> Items;
        public uint Width;
        public uint Height;
        public uint Font;
        public Color Color;
        public uint Flags;

        public Text(ByteReader reader) : base(reader)
        {
        }

        public override void Read()
        {
            base.Read();
            Width = Reader.ReadUInt32();
            Height = Reader.ReadUInt32();
            Font = Reader.ReadUInt32();
            Color = Reader.ReadColor();
            Flags = Reader.ReadUInt32();
            Debug.Assert(Reader.ReadUInt32()==0);
            Items = new List<Paragraph>();
            var parCount = Reader.ReadUInt32();
            for (int i = 0; i < parCount; i++)
            {
                var par = new Paragraph(Reader);
                par.Read();
                Items.Add(par);
            }
        }

        public override void Write(ByteWriter Writer)
        {
            base.Write(Writer);
            Writer.WriteUInt32(Width);
            Writer.WriteUInt32(Height);
            Writer.WriteUInt32(Font);
            Writer.WriteColor(Color);
            Writer.WriteUInt32(Flags);
            Writer.WriteInt32(0);
            Writer.WriteUInt32((uint) Items.Count);
            foreach (Paragraph paragraph in Items)
            {
                paragraph.Write(Writer);
            }

            
        }
    }

    public class Paragraph : DataLoader
    {
        public string Value;
        public uint Flags;

        public Paragraph(ByteReader reader) : base(reader)
        {
        }

        public Paragraph(EXE.ChunkList.Chunk chunk) : base(chunk)
        {
        }

        public override void Read()
        {
            Value = Reader.AutoReadUnicode();
            Flags = Reader.ReadUInt32();
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.AutoWriteUnicode(Value);
            Writer.WriteUInt32(Flags);
        }

        public override void Print(){}
        
    }
}