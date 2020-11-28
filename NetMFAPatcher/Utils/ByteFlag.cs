using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.utils
{
    public static class ByteFlag
    {
        public static bool GetFlag(UInt32 flagbyte, int pos)
        {
            UInt32 mask = (uint) (2 << pos);
            UInt32 result = flagbyte & mask;
            return result == mask;
        }
    }
}