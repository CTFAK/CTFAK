using System;
using CTFAK.Utils;

namespace CTFAK_Runtime_Tools
{
    public static class Memory
    {
        public static IntPtr FindFinalAddress(ByteReader reader,IntPtr baseAddr, int[] offsets)
        {
            var origPos = reader.Tell();
            long ptr = baseAddr.ToInt64();
            // ptr = reader.ReadInt32();
            reader.Seek(ptr);
            
            foreach (int offset in offsets)
            {
                if (offset == 0) continue;
                reader.Seek(ptr+offset);
                ptr = reader.ReadInt32();
                
            }
            reader.Seek(origPos);
            return new IntPtr(ptr);
     
        }

        public static void ReadAtOffset(this ByteReader reader,int offset, Action<ByteReader> action)
        {
            if (offset == 0) return;
            var start = reader.Tell();
            reader.Seek(offset);
            action.Invoke(reader);
            reader.Seek(start);
        }
    }
}