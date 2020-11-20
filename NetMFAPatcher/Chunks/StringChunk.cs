using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.Chunks
{
    class StringChunk : ChunkLoader
    {
        public string value;
        

        public override void Read()
        {
            reader = new ByteIO(chunk.chunk_data);
            value = reader.ReadWideString1();//reader.ReadWideString();
            

        }
        public override void Print()
        {
            //Logger.Log($"{chunk.name}: {value}");
        }
        

    }
    class AppName : StringChunk
    {
    }
    class AppAuthor : StringChunk
    {
    }
    class ExtPath : StringChunk
    {
    }
    class EditorFilename : StringChunk
    {
    }
    class TargetFilename : StringChunk
    {
    }
    class AppDoc : StringChunk
    {
    }
    class AboutText : StringChunk
    {
    }
    class Copyright : StringChunk
    {
    }
    class DemoFilePath : StringChunk
    {
    }

    
}
