using System.Collections.Generic;
using System.IO;
using CTFAK.MMFParser.Translation;
using CTFAK.Utils;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class Shaders:ChunkLoader
    {
        public List<Shader> ShaderList;

        public Shaders(ByteReader reader) : base(reader)
        {
        }

        

        public override void Read()
        {
            var start = Reader.Tell();
            var count = Reader.ReadInt32();
            List<int> offsets = new List<int>();
            ShaderList = new List<Shader>();
            for (int i = 0; i < count; i++)
            {
                offsets.Add(Reader.ReadInt32());
            }

            foreach (int offset in offsets)
            {
                Reader.Seek(start+offset);
                var shader = new Shader(Reader);
                shader.Read();
                ShaderList.Add(shader);
                
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
    public class Shader:ChunkLoader
    {
        public List<ShaderParameter> Parameters = new List<ShaderParameter>();
        public string Name;
        public string Data;
        public int BackgroundTexture;

        public Shader(ByteReader reader) : base(reader)
        {
        }

   

        public override void Read()
        {
            var start = Reader.Tell();
            var nameOffset = Reader.ReadInt32();
            var dataOffset = Reader.ReadInt32();
            var parameterOffset = Reader.ReadInt32();
            BackgroundTexture = Reader.ReadInt32();
            Reader.Seek(start+nameOffset);
            Name = Reader.ReadAscii();
            Reader.Seek(start+dataOffset);
            Data = Reader.ReadAscii();
            if (parameterOffset != 0)
            {
                parameterOffset = (int) (parameterOffset + start);
                Reader.Seek(parameterOffset);
                var paramCount = Reader.ReadInt32();
                
                for (int i = 0; i < paramCount; i++)
                {
                    var newParameter = new ShaderParameter((ByteReader) null);
                    Parameters.Add(newParameter);
                }

                var typeOffset = Reader.ReadInt32();
                var namesOffset = Reader.ReadInt32();
                Reader.Seek(parameterOffset+typeOffset);
                foreach (var parameter in Parameters)
                {
                    parameter.Type = Reader.ReadByte();
                }
                Reader.Seek(parameterOffset+namesOffset);
                foreach (ShaderParameter parameter in Parameters)
                {
                    parameter.Name = Reader.ReadAscii();
                }
            }
            ShaderGenerator.CreateAndDumpShader(this);
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
    public class ShaderParameter:ChunkLoader
    {
        public string Name;
        public int Type;
        public int Value;
        public ShaderParameter(ByteReader reader) : base(reader)
        {
        }

   

        public override void Read()
        {
            throw new System.NotImplementedException();
        }

        public override void Write(ByteWriter Writer)
        {
            throw new System.NotImplementedException();
        }
        public override string[] GetReadableData()
        {
            throw new System.NotImplementedException();
        }

        public string GetValueType()
        {
            switch (Type)
            {
                case 0: return "int";
                case 1: return "float";
                case 2: return "int_float4";
                case 3: return "image";
                default: return "unk";
                    
                
            }
        }
    }
}