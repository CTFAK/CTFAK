using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            return new ByteReader(decompress_block(exeReader, compSize, decompSize));
        }

        public static byte[] decompress_block(ByteReader reader, int size, int decompSize)
        {
            ZLibDecompressOptions decompOpts = new ZLibDecompressOptions();
            MemoryStream compressedStream = new MemoryStream(reader.ReadBytes(size));
            MemoryStream decompressedStream = new MemoryStream();
            using (ZLibStream zs = new ZLibStream(compressedStream, decompOpts))
            {
                zs.CopyTo(decompressedStream);
            }

            byte[] decompressedData = decompressedStream.GetBuffer();
            // Trimming array to decompSize,
            // because ZlibStream always pads to 0x100
            Array.Resize<byte>(ref decompressedData, decompSize);
            return decompressedData;
        }
    }
}