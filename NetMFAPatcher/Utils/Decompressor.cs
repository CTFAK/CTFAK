using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace NetMFAPatcher.Utils
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
            byte[] compressedData = exeReader.ReadBytes(compSize);
            byte[] actualData = Ionic.Zlib.ZlibStream.UncompressBuffer(compressedData);
            Debug.Assert(actualData.Length == decompSize);
            decompressed = decompSize;
            return new ByteReader(actualData);
        }

        public static byte[] decompress_block(ByteReader reader, int size, int decompSize)
        {
            byte[] compressedData = reader.ReadBytes(size);
            byte[] actualData = Ionic.Zlib.ZlibStream.UncompressBuffer(compressedData);
            return actualData;
        }

        public static ByteReader decompress_asReader(ByteReader imageData, int v, int decompressedSize)
        {
            return new ByteReader(decompress_block(imageData, v, decompressedSize));
        }

        
    }
}