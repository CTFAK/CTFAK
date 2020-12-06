using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace NetMFAPatcher.Utils
{
    public static class Decompressor
    {
        public static byte[] Decompress(ByteIO exeReader, out int decompressed)
        {
            Int32 decompSize = exeReader.ReadInt32();
            Int32 compSize = exeReader.ReadInt32();
            decompressed = decompSize;
            return decompress_block(exeReader, compSize, decompSize);
        }

        public static ByteIO DecompressAsReader(ByteIO exeReader, out int decompressed)
        {
            Int32 decompSize = exeReader.ReadInt32();
            Int32 compSize = exeReader.ReadInt32();
            byte[] compressedData = exeReader.ReadBytes(compSize);
            byte[] actualData = Ionic.Zlib.ZlibStream.UncompressBuffer(compressedData);
            Debug.Assert(actualData.Length == decompSize);
            decompressed = decompSize;
            return new ByteIO(actualData);
        }

        public static byte[] decompress_block(ByteIO reader, int size, int decompSize)
        {
            byte[] compressedData = reader.ReadBytes(size);
            byte[] actualData = Ionic.Zlib.ZlibStream.UncompressBuffer(compressedData);
            return actualData;
        }

        public static ByteIO decompress_asReader(ByteIO imageData, int v, int decompressedSize)
        {
            return new ByteIO(decompress_block(imageData, v, decompressedSize));
        }

        
    }
}