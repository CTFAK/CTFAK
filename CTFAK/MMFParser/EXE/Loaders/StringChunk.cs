using System;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class StringChunk : ChunkLoader
    {
        public string Value;


        public override void Read()
        {
            Value = Reader.ReadUniversal();
            Print(false);
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUniversal(Value,true);
        }

        public override void Print(bool ext)
        {
            Logger.Log($"{Chunk?.Name} contains:  {Value}\n",true,ConsoleColor.DarkCyan);
        }

        public override string[] GetReadableData()
        {
            return new string[]
            {
                $"Value: {Value}"
            };
        }

        public override string ToString()
        {
            return Value;
        }
        public static implicit operator string(StringChunk chunk)
        {
            return chunk.Value;
        }


        public StringChunk(ByteReader reader) : base(reader)
        {
        }

   
    }

    public class AppName : StringChunk
    {
        public AppName(ByteReader reader) : base(reader)
        {
        }

        
    }

    public class AppAuthor : StringChunk
    {
        public AppAuthor(ByteReader reader) : base(reader)
        {
        }

        
    }

    class ExtPath : StringChunk
    {
        public ExtPath(ByteReader reader) : base(reader)
        {
        }

       
    }

    public class EditorFilename : StringChunk
    {
        public EditorFilename(ByteReader reader) : base(reader)
        {
        }

       
    }

    public class TargetFilename : StringChunk
    {
        public TargetFilename(ByteReader reader) : base(reader)
        {
        }
        
    }

    class AppDoc : StringChunk
    {
        public AppDoc(ByteReader reader) : base(reader)
        {
        }

        
    }

    class AboutText : StringChunk
    {
        public AboutText(ByteReader reader) : base(reader)
        {
        }

        
    }

    public class Copyright : StringChunk
    {
        public Copyright(ByteReader reader) : base(reader)
        {
        }

        
    }

    class DemoFilePath : StringChunk
    {
        public DemoFilePath(ByteReader reader) : base(reader)
        {
        }

        
    }
}