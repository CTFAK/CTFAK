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
            // Print(false);
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteUniversal(Value,true);
        }

        public override void Print(bool ext)
        {
            Logger.Log($"{Chunk.Name} contains:  {Value}\n",true,ConsoleColor.DarkCyan);
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


        public StringChunk(ByteReader reader) : base(reader)
        {
        }

        public StringChunk(Chunk chunk) : base(chunk)
        {
        }
    }

    public class AppName : StringChunk
    {
        public AppName(ByteReader reader) : base(reader)
        {
        }

        public AppName(Chunk chunk) : base(chunk)
        {
        }
    }

    public class AppAuthor : StringChunk
    {
        public AppAuthor(ByteReader reader) : base(reader)
        {
        }

        public AppAuthor(Chunk chunk) : base(chunk)
        {
        }
    }

    class ExtPath : StringChunk
    {
        public ExtPath(ByteReader reader) : base(reader)
        {
        }

        public ExtPath(Chunk chunk) : base(chunk)
        {
        }
    }

    public class EditorFilename : StringChunk
    {
        public EditorFilename(ByteReader reader) : base(reader)
        {
        }

        public EditorFilename(Chunk chunk) : base(chunk)
        {
        }
    }

    public class TargetFilename : StringChunk
    {
        public TargetFilename(ByteReader reader) : base(reader)
        {
        }

        public TargetFilename(Chunk chunk) : base(chunk)
        {
        }
    }

    class AppDoc : StringChunk
    {
        public AppDoc(ByteReader reader) : base(reader)
        {
        }

        public AppDoc(Chunk chunk) : base(chunk)
        {
        }
    }

    class AboutText : StringChunk
    {
        public AboutText(ByteReader reader) : base(reader)
        {
        }

        public AboutText(Chunk chunk) : base(chunk)
        {
        }
    }

    public class Copyright : StringChunk
    {
        public Copyright(ByteReader reader) : base(reader)
        {
        }

        public Copyright(Chunk chunk) : base(chunk)
        {
        }
    }

    class DemoFilePath : StringChunk
    {
        public DemoFilePath(ByteReader reader) : base(reader)
        {
        }

        public DemoFilePath(Chunk chunk) : base(chunk)
        {
        }
    }
}