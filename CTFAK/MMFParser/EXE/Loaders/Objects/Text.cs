using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Xml.Schema;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders.Objects
{
    public class Text:ChunkLoader
    {
        public int Width;
        public int Height;
        public List<Paragraph> Items = new List<Paragraph>();

        public Text(ByteReader reader) : base(reader)
        {
        }

        

        public override void Read()
        {
            if (Settings.GameType == GameType.OnePointFive)
            {
                var currentPos = Reader.Tell();
                var size = Reader.ReadInt32();
                Width = Reader.ReadInt16();
                Height = Reader.ReadInt16();
                List<int> itemOffsets = new List<int>();
                var offCount = Reader.ReadInt16();
                for (int i = 0; i < offCount; i++)
                {
                    itemOffsets.Add(Reader.ReadInt16());
                }
                foreach (int itemOffset in itemOffsets)
                {
                    Reader.Seek(currentPos+itemOffset);
                    var par = new Paragraph(Reader);
                    par.Read();
                    Items.Add(par);
                } 
            }
            else
            {
                var currentPos = Reader.Tell();
                var size = Reader.ReadInt32();
                Width = Reader.ReadInt32();
                Height = Reader.ReadInt32();
                List<int> itemOffsets = new List<int>();
                var offCount = Reader.ReadInt32();
                for (int i = 0; i < offCount; i++)
                {
                    itemOffsets.Add(Reader.ReadInt32());
                }
                foreach (int itemOffset in itemOffsets)
                {
                    Reader.Seek(currentPos+itemOffset);
                    var par = new Paragraph(Reader);
                    par.Read();
                    Items.Add(par);
                } 
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

    public class Paragraph : ChunkLoader
    {
        public ushort FontHandle;
        public BitDict Flags = new BitDict(new string[]{
            "HorizontalCenter",
            "RightAligned",
            "VerticalCenter",
            "BottomAligned",
            "None", "None", "None", "None",
            "Correct",
            "Relief"});
        public string Value;
        public Color Color;

        public Paragraph(ByteReader reader) : base(reader)
        {
        }

        

        public override void Read()
        {
            if (Settings.GameType == GameType.OnePointFive)
            {
                var size = Reader.ReadUInt16();
                FontHandle = Reader.ReadUInt16();
                Color = Reader.ReadColor();
                Flags.flag = Reader.ReadUInt16();
                Value = Reader.ReadUniversal();

            }
            else
            {
                FontHandle = Reader.ReadUInt16();
                Flags.flag = Reader.ReadUInt16();
                Color = Reader.ReadColor();
                Value = Reader.ReadUniversal(); 
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
}