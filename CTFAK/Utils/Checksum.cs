using System;
using System.Runtime.InteropServices;

namespace CTFAK.Utils
{
    public static class Checksum
    {
        [DllImport("x64\\Decrypter-x64.dll")]
        public static extern UInt32 GenChecksum(IntPtr name,IntPtr pass);
        public static UInt32 MakeGroupChecksum(string name, string pass)
        {
            var namePtr = Marshal.StringToHGlobalUni(name);
            var passPtr = Marshal.StringToHGlobalUni(pass);
            var result = GenChecksum(namePtr,passPtr);
            Marshal.FreeHGlobal(namePtr);
            Marshal.FreeHGlobal(passPtr);
            return result;

        }
        
    }
}