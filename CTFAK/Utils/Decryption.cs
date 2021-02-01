using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CTFAK.Utils
{
    static class Decryption
    {
        private static byte[] _decryptionKey;
        public static byte MagicChar = 54;

        
        public static void MakeKey(string data1, string data2, string data3)
        {
            // MakeKeyUnicode(data1,data2,data3);
            // return;
            IntPtr keyPtr;
            var combined = "";
            combined += data1;
            combined += data2;
            combined += data3;
            Logger.Log("Combined data " + combined, true, ConsoleColor.Yellow);
            keyPtr = Marshal.StringToHGlobalAnsi(combined);
            
            keyPtr = make_key_combined(keyPtr, MagicChar);
            byte[] key = new byte[256];
            Marshal.Copy(keyPtr, key, 0, 256);
            Marshal.FreeHGlobal(keyPtr);
            _decryptionKey = key;
            Logger.Log($"First 16-Bytes of key: {_decryptionKey.GetHex(16)}", true, ConsoleColor.Yellow);
            File.WriteAllBytes($"{Settings.DumpPath}\\key.bin", _decryptionKey);
        }

        public static void MakeKeyUnicode(string data1, string data2, string data3)
        {
            IntPtr data1ptr;
            IntPtr data2ptr;
            IntPtr data3ptr;
            IntPtr keyPtr;
            data1ptr = Marshal.StringToHGlobalUni(data1);
            data2ptr = Marshal.StringToHGlobalUni(data2);
            data3ptr = Marshal.StringToHGlobalUni(data3);
            keyPtr = make_key_w(data1ptr, data2ptr, data3ptr, MagicChar);
            byte[] key = new byte[256];
            Marshal.Copy(keyPtr, key, 0, 256);
            _decryptionKey = key;
            Marshal.FreeHGlobal(data1ptr);
            Marshal.FreeHGlobal(data2ptr);
            Marshal.FreeHGlobal(data3ptr);
            Logger.Log($"First 16-Bytes of key: {_decryptionKey.GetHex(16)}", true, ConsoleColor.Yellow);

            
        }

        public static byte[] MakeKeyFromComb(string data, byte magicChar = 54)
        {
            var rawKeyPtr = Marshal.StringToHGlobalAnsi(data);
            var ptr = Decryption.make_key_combined(rawKeyPtr, magicChar);

            byte[] key = new byte[256];
            Marshal.Copy(ptr, key, 0, 256);
            Marshal.FreeHGlobal(rawKeyPtr);
            Array.Resize(ref key,data.Length);
            Array.Resize(ref key,256);

            return key;
        }


        public static byte[] DecodeMode3(byte[] chunkData, int chunkSize, int chunkId, out int decompressed)
        {
            ByteReader reader = new ByteReader(chunkData);
            uint decompressedSize = reader.ReadUInt32();

            byte[] rawData = reader.ReadBytes((int) reader.Size());
            if ((chunkId & 1) == 1&&Settings.Build>284)
            {
                rawData[0] ^= (byte) ((byte) (chunkId & 0xFF) ^ (byte) (chunkId >> 0x8));
            }

            rawData = DecodeChunk(rawData, chunkSize);
            using (ByteReader data = new ByteReader(rawData))
            {
                uint compressedSize = data.ReadUInt32();
                decompressed = (int) decompressedSize;
                return Decompressor.DecompressBlock(data, (int) compressedSize, (int) decompressedSize);
            }
        }

        public static byte[] DecodeChunk(byte[] chunkData, int chunkSize)
        {
            IntPtr inputChunkPtr = Marshal.AllocHGlobal(chunkData.Length);
            Marshal.Copy(chunkData, 0, inputChunkPtr, chunkData.Length);

            IntPtr keyPtr = Marshal.AllocHGlobal(_decryptionKey.Length);
            Marshal.Copy(_decryptionKey, 0, keyPtr, _decryptionKey.Length);

            var outputChunkPtr = decode_chunk(inputChunkPtr, chunkSize, MagicChar, keyPtr);

            byte[] decodedChunk = new byte[chunkSize];
            Marshal.Copy(outputChunkPtr, decodedChunk, 0, chunkSize);

            Marshal.FreeHGlobal(inputChunkPtr);
            Marshal.FreeHGlobal(keyPtr);

            return decodedChunk;
        }
        

        #if WIN64
        private const string _dllPath = "x64\\Decrypter-x64.dll";
        #else
        private const string _dllPath = "x86\\Decrypter-x86.dll";
        #endif
        
        [DllImport(_dllPath, EntryPoint = "decode_chunk", CharSet = CharSet.Auto)]
        public static extern IntPtr decode_chunk(IntPtr chunkData, int chunkSize, byte magicChar, IntPtr wrapperKey);

        [DllImport(_dllPath, EntryPoint = "make_key", CharSet = CharSet.Auto)]
        public static extern IntPtr make_key(IntPtr cTitle, IntPtr cCopyright, IntPtr cProject, byte magicChar);
        [DllImport(_dllPath, EntryPoint = "make_key_w", CharSet = CharSet.Unicode)]
        public static extern IntPtr make_key_w(IntPtr cTitle, IntPtr cCopyright, IntPtr cProject, byte magicChar);


        [DllImport(_dllPath, EntryPoint = "make_key_combined", CharSet = CharSet.Auto)]
        public static extern IntPtr make_key_combined(IntPtr data, byte magicChar);

        [DllImport(_dllPath, EntryPoint = "make_key_w_combined", CharSet = CharSet.Auto)]
        public static extern IntPtr make_key_w_combined(IntPtr data, byte magicChar);
    }
}