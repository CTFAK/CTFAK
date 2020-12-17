using System;
using DotNetCTFDumper.Utils;
using static DotNetCTFDumper.MMFParser.EXE.ChunkList;

namespace DotNetCTFDumper.MMFParser.EXE.Loaders
{
    public class StringChunk : ChunkLoader
    {
        public string Value;


        public override void Read()
        {
            Reader = new ByteReader(Chunk.ChunkData);
            Value = Reader.ReadWideString();
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


        public StringChunk(ByteReader reader) : base(reader)
        {
        }

        public StringChunk(Chunk chunk) : base(chunk)
        {
        }
    }

    class AppName : StringChunk
    {
        public AppName(ByteReader reader) : base(reader)
        {
        }

        public AppName(Chunk chunk) : base(chunk)
        {
        }
    }

    class AppAuthor : StringChunk
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

    class EditorFilename : StringChunk
    {
        public EditorFilename(ByteReader reader) : base(reader)
        {
        }

        public EditorFilename(Chunk chunk) : base(chunk)
        {
        }
    }

    class TargetFilename : StringChunk
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

    class Copyright : StringChunk
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