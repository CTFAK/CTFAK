using System;

namespace CTFAK.Utils
{
    public static class ByteFlag
    {
        public static bool GetFlag(UInt32 flagbyte, int pos)
        {
            UInt32 mask = (uint) (1 << pos);
            UInt32 result = flagbyte & mask;
            return result == mask;
        }
    }
}