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
        public static void MakeKey(string STitle, string SCopyright,string SProject, byte MagicChar)
        {
            byte[] nameBytes = Encoding.ASCII.GetBytes(STitle);
            var name = Marshal.AllocHGlobal(STitle.Length);
            Marshal.Copy(nameBytes, 0, name, STitle.Length);

            byte[] copyrightBytes = Encoding.ASCII.GetBytes(SCopyright);
            var copyright = Marshal.AllocHGlobal(SCopyright.Length);
            Marshal.Copy(copyrightBytes, 0, copyright, SCopyright.Length);

            byte[] filenameBytes = Encoding.ASCII.GetBytes(SProject);           
            var pathfilename = Marshal.AllocHGlobal(SProject.Length);
            Marshal.Copy(filenameBytes, 0, pathfilename, SProject.Length);

            var ptr = Decryption.make_key(name, copyright, pathfilename, MagicChar);

            byte[] Key = new byte[257];
            Marshal.Copy(ptr, Key, 0, 257);
            Marshal.FreeHGlobal(name);
            Marshal.FreeHGlobal(copyright);
            Marshal.FreeHGlobal(pathfilename);
            key = Key;
            //some hardcoded checks for SL

           

            if (Console.ReadKey().Key==ConsoleKey.N)
            {
                if(key[73] == 0xB2)
                {
                    return;
                }
                MakeKey("Sister Location", "Scott Cawthon", @"C:\Users\Scott\Desktop\FNAF 5\FNaF 5-157.mfa", 54);
            }
        }
        

        public static byte[] DecodeMode3(byte[] ChunkData, int ChunkSize,int ChunkID, byte MagicChar)
        {
            var reader = new ByteIO(ChunkData);
            var DecompressedSize = reader.ReadUInt32();
            
            var chunkData = reader.ReadBytes((int)reader.Size());
            chunkData[0] ^= (byte)(((byte)ChunkID & 0xFF) ^ ((byte)ChunkID >> 0x8));
            var data = new ByteIO(DecodeChunk(chunkData,ChunkSize,MagicChar));
            var compressed_size = data.ReadUInt32();

            return Decompressor.decompress_block(data, (int)compressed_size, (int)DecompressedSize);
        }
        public static byte[] DecodeChunk(byte[] ChunkData, int ChunkSize,byte MagicChar)
        {
            //Console.WriteLine("Decoding: "+ChunkData.Log(false,"X2"));
            IntPtr InputChunkPtr = Marshal.AllocHGlobal(ChunkSize);
            Marshal.Copy(ChunkData, 0, InputChunkPtr, ChunkSize-4);
            var OutputChunkPtr = decode_chunk(InputChunkPtr, ChunkSize, MagicChar);
            byte[] DecodedChunk = new byte[ChunkSize];
            Marshal.Copy(OutputChunkPtr, DecodedChunk,0,ChunkSize);
            //Console.WriteLine("Result: " + DecodedChunk.Log(false, "X2"));
            return DecodedChunk;


        }
        [DllImport("Decrypter-x64.dll", EntryPoint = "decode_chunk", CharSet = CharSet.Auto)]
        public static extern IntPtr decode_chunk(IntPtr chunk_data,int chunk_size, byte magic_char);
        [DllImport("Decrypter-x64.dll", EntryPoint = "make_key", CharSet = CharSet.Auto)]
        public static extern IntPtr make_key(IntPtr c_title, IntPtr c_copyright, IntPtr c_project, byte magic_char);


    }
}
