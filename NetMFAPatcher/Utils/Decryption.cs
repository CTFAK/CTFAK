using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.utils
{
    class Decryption
    {
        public static byte[] DecryptionKey;
        public static byte MagicChar=54;
        public static void MakeKey(string STitle, string SCopyright,string SProject)
        {
            var RawKey = "";
            RawKey += STitle;
            RawKey += SCopyright;
            RawKey += SProject;
            Logger.Log("Combined data "+RawKey,true,ConsoleColor.Yellow);
            var RawKeyPTR = Marshal.StringToHGlobalAnsi(RawKey);

            var ptr = Decryption.make_key_combined(RawKeyPTR, MagicChar);

            byte[] Key = new byte[257];
            Marshal.Copy(ptr, Key, 0, 256);
            Marshal.FreeHGlobal(RawKeyPTR);

            DecryptionKey = Key;
            Logger.Log($"First 16-Bytes of key: {DecryptionKey.GetHex(16)}",true,ConsoleColor.Yellow);
            File.WriteAllBytes($"{Program.DumpPath}\\key.bin", DecryptionKey);          
        }
        

        public static byte[] DecodeMode3(byte[] ChunkData, int ChunkSize,int ChunkID)
        {
            var reader = new ByteIO(ChunkData);
            var DecompressedSize = reader.ReadUInt32();
            
            var chunkData = reader.ReadBytes((int)reader.Size());
            if (ChunkID % 2 != 0)
            {
                chunkData[0] ^= (byte)(((byte)ChunkID & 0xFF) ^ ((byte)ChunkID >> 0x8));
            }
            var rawData = DecodeChunk(chunkData, ChunkSize);
            var data = new ByteIO(rawData);
            var compressed_size = data.ReadUInt32();

            return Decompressor.decompress_block(data, (int)compressed_size, (int)DecompressedSize);
        }
        public static byte[] DecodeChunk(byte[] ChunkData, int ChunkSize)
        {
            IntPtr InputChunkPtr = Marshal.AllocHGlobal(ChunkData.Length);
            Marshal.Copy(ChunkData, 0, InputChunkPtr, ChunkData.Length);

            IntPtr KeyPtr = Marshal.AllocHGlobal(DecryptionKey.Length);
            Marshal.Copy(DecryptionKey, 0, KeyPtr, DecryptionKey.Length);

            var OutputChunkPtr = decode_chunk(InputChunkPtr, ChunkSize, MagicChar, KeyPtr);

            byte[] DecodedChunk = new byte[ChunkSize];
            Marshal.Copy(OutputChunkPtr, DecodedChunk,0,ChunkSize);

            Marshal.FreeHGlobal(InputChunkPtr);
            Marshal.FreeHGlobal(KeyPtr);

            return DecodedChunk;
        }



        [DllImport("Decrypter-x64.dll", EntryPoint = "decode_chunk", CharSet = CharSet.Auto)]
        public static extern IntPtr decode_chunk(IntPtr chunk_data, int chunk_size, byte magic_char,IntPtr wrapper_key);
        [DllImport("Decrypter-x64.dll", EntryPoint = "make_key", CharSet = CharSet.Auto)]
        public static extern IntPtr make_key(IntPtr c_title, IntPtr c_copyright, IntPtr c_project, byte magic_char);
        [DllImport("Decrypter-x64.dll", EntryPoint = "make_key_combined", CharSet = CharSet.Auto)]
        public static extern IntPtr make_key_combined(IntPtr data, byte magic_char);      
    }
}
