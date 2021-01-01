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
        public static byte MagicChar = 0;

        public static byte[] FixString(byte[] bytes)
        {
            List<byte> newBytes = new List<byte>();
            foreach (byte b in bytes)
            {
                if(b!=0) newBytes.Add(b);
            }

            return bytes; //newBytes.ToArray();

        }

        public static void MakeKey(string data1, string data2, string data3)
        {
            
            var russian = false;
            IntPtr keyPtr=new IntPtr();
            if (russian)
            {

                
                var fixed1 = FixString(Encoding.Unicode.GetBytes(data1));
                var data1Ptr = Marshal.AllocHGlobal(fixed1.Length);
                Marshal.Copy(fixed1,0,data1Ptr,fixed1.Length);
                
                var fixed2 = FixString(Encoding.Unicode.GetBytes(data2));
                var data2Ptr = Marshal.AllocHGlobal(fixed2.Length);
                Marshal.Copy(fixed2,0,data2Ptr,fixed2.Length);
                
                var fixed3 = FixString(Encoding.Unicode.GetBytes(data3));
                var data3Ptr = Marshal.AllocHGlobal(fixed3.Length);
                Marshal.Copy(fixed3,0,data1Ptr,fixed3.Length);

                
                keyPtr = make_key_w(data1Ptr,data2Ptr,data3Ptr,MagicChar);
                Marshal.FreeHGlobal(data1Ptr);
                Marshal.FreeHGlobal(data2Ptr);
                Marshal.FreeHGlobal(data3Ptr);
            }
            else
            {
                var rawKey = "";
                rawKey += data1;
                rawKey += data2;
                rawKey += data3;
                Logger.Log("Combined data " + rawKey, true, ConsoleColor.Yellow);
                keyPtr = Marshal.StringToHGlobalAnsi(rawKey);
                keyPtr = make_key_combined(keyPtr, MagicChar);
                
                
            }
            byte[] key = new byte[256];
            Marshal.Copy(keyPtr, key, 0, 256);
            _decryptionKey = key;

            


            
            Logger.Log($"First 16-Bytes of key: {_decryptionKey.GetHex(16)}", true, ConsoleColor.Yellow);
            Logger.Log(Encoding.Unicode.GetString(_decryptionKey));
            File.WriteAllBytes($"{Settings.DumpPath}\\key.bin", _decryptionKey);
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
                return Decompressor.decompress_block(data, (int) compressedSize, (int) decompressedSize);
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

        private const string _dllPath = "x64\\Decrypter-x64.dll";
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