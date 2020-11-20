using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.utils
{
    public static class ByteFlag
    {
        public static bool getFlag(int flagbyte,int pos)
        {
            var mask = Math.Pow(2, pos);
            var result = flagbyte & (int)mask;
            return result == mask;

        }
    }
}
