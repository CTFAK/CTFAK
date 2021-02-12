using System;
using System.Runtime.InteropServices;

namespace CTFAK.Utils
{
    public static class Checksum
    {
        
        public static UInt32 MakeChecksumNative(string name, string pass)
        {
            var namePtr = Marshal.StringToHGlobalUni(name);
            var passPtr = Marshal.StringToHGlobalUni(pass);
            var result = NativeLib.GenChecksum(namePtr,passPtr);
            Marshal.FreeHGlobal(namePtr);
            Marshal.FreeHGlobal(passPtr);
            return result;

        }
        }
}