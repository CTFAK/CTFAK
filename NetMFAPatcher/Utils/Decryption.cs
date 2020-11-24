using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.utils
{
    class Decryption
    {
        public static byte[] key;
        public static string storedName;
        public static string storedCopyright;
        public static string storedPath;
        public static void MakeKey(string STitle, string SCopyright,string SProject, byte MagicChar)
        {
            storedName = STitle;
            storedCopyright = SCopyright;
            storedPath = SProject;
            byte[] nameBytes = Encoding.ASCII.GetBytes(STitle);
            var name = Marshal.StringToHGlobalAnsi(STitle);//Marshal.AllocHGlobal(STitle.Length);
            Marshal.Copy(nameBytes, 0, name, STitle.Length);

            byte[] copyrightBytes = Encoding.ASCII.GetBytes(SCopyright);
            var copyright = Marshal.StringToHGlobalAnsi(SCopyright);
            Marshal.Copy(copyrightBytes, 0, copyright, SCopyright.Length);

            byte[] filenameBytes = Encoding.ASCII.GetBytes(SProject);
            var pathfilename = Marshal.StringToHGlobalAnsi(SProject);
            Marshal.Copy(filenameBytes, 0, pathfilename, SProject.Length);

            var ptr = Decryption.make_key(name, copyright, pathfilename, MagicChar);

            byte[] Key = new byte[257];
            Marshal.Copy(ptr, Key, 0, 256);
            Marshal.FreeHGlobal(name);
            Marshal.FreeHGlobal(copyright);
            Marshal.FreeHGlobal(pathfilename);
            key = Key;
            key.Log(true, "X2");

           

            
        }
        

        public static byte[] DecodeMode3(byte[] ChunkData, int ChunkSize,int ChunkID, byte MagicChar)
        {
            var reader = new ByteIO(ChunkData);
            var DecompressedSize = reader.ReadUInt32();
            
            var chunkData = reader.ReadBytes((int)reader.Size());
            if (ChunkID % 2 != 0)
            {
                chunkData[0] ^= (byte)(((byte)ChunkID & 0xFF) ^ ((byte)ChunkID >> 0x8));
            }
            var data = new ByteIO(DecodeChunk(chunkData,ChunkSize,MagicChar));
            var compressed_size = data.ReadUInt32();

            return Decompressor.decompress_block(data, (int)compressed_size, (int)DecompressedSize);
        }
        public static byte[] DecodeChunk(byte[] ChunkData, int ChunkSize,byte MagicChar)
        {
            IntPtr InputChunkPtr = Marshal.AllocHGlobal(ChunkSize);
            Marshal.Copy(ChunkData, 0, InputChunkPtr, ChunkSize-4);
            var OutputChunkPtr = decode_chunk(InputChunkPtr, ChunkSize, MagicChar);
            byte[] DecodedChunk = new byte[ChunkSize];
            Marshal.Copy(OutputChunkPtr, DecodedChunk,0,ChunkSize);
            return DecodedChunk;


        }
        [DllImport("Decrypter-x64.dll", EntryPoint = "decode_chunk", CharSet = CharSet.Auto)]
        public static extern IntPtr decode_chunk(IntPtr chunk_data,int chunk_size, byte magic_char);
        [DllImport("Decrypter-x64.dll", EntryPoint = "make_key", CharSet = CharSet.Auto)]
        public static extern IntPtr make_key(IntPtr c_title, IntPtr c_copyright, IntPtr c_project, byte magic_char);


    }
}
