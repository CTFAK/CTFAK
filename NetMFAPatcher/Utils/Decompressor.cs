using System;
using System.Diagnostics;
using System.IO;
using Joveler.Compression.ZLib; 

namespace DotNetCTFDumper.Utils
{
    public static class Decompressor
    {
        public static byte[] Decompress(ByteReader exeReader, out int decompressed)
        {
            Int32 decompSize = exeReader.ReadInt32();
            Int32 compSize = exeReader.ReadInt32();
            decompressed = decompSize;
            return decompress_block(exeReader, compSize, decompSize);
        }

        public static ByteReader DecompressAsReader(ByteReader exeReader, out int decompressed)
        {
            Int32 decompSize = exeReader.ReadInt32();
            Int32 compSize = exeReader.ReadInt32();
            decompressed = decompSize;
            return new ByteReader(decompress_block(exeReader,compSize,decompSize));
        }

        public static byte[] decompress_block(ByteReader reader, int size, int decompSize)
        {
            ZLibDecompressOptions decompOpts = new ZLibDecompressOptions();
            MemoryStream compressedStream = new MemoryStream(reader.ReadBytes(size));
            MemoryStream decompressedStream = new MemoryStream();
            using (DeflateStream zs = new DeflateStream(compressedStream, decompOpts))
            {
                zs.CopyTo(decompressedStream);
                
            }
            return decompressedStream.GetBuffer();

        }


        
    }
}