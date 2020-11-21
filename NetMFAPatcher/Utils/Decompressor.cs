using System;

namespace NetMFAPatcher.Utils
{
    public static class Decompressor
    {
        public static byte[] Decompress(ByteIO exeReader)
        {
            Int32 decomp_size = exeReader.ReadInt32();
            Int32 comp_size = exeReader.ReadInt32();
            return decompress_block(exeReader, comp_size, decomp_size);
             
        }
        public static ByteIO DecompressAsReader(ByteIO exeReader)
        {
            Int32 decomp_size = exeReader.ReadInt32();
            Int32 comp_size = exeReader.ReadInt32();
            byte[] compressedData = exeReader.ReadBytes(comp_size);
            byte[] actualData = Ionic.Zlib.ZlibStream.UncompressBuffer(compressedData);
            return new ByteIO(actualData);
        }
        public static byte[] decompress_block(ByteIO reader,int size,int decomp_size)
        {
            byte[] compressedData = reader.ReadBytes(size);
            byte[] actualData = Ionic.Zlib.ZlibStream.UncompressBuffer(compressedData);
            return actualData;

        }

        public static ByteIO decompress_asReader(ByteIO image_data, int v, int decompressed_size)
        {
            return new ByteIO(decompress_block(image_data, v, decompressed_size));
        }

        internal static byte[] decompress_block(ByteIO data, uint compressed_size, uint decompressedSize)
        {
            throw new NotImplementedException();
        }
    }


}
