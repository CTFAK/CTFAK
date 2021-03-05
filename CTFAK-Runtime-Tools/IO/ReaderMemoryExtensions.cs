using System.Diagnostics;
using CTFAK.Utils;

namespace CTFAK_Runtime_Tools.IO
{
    public static class ReaderMemoryExtensions
    {
        public static Process GetProc(this ByteReader reader)
        {
            return (reader.BaseStream as RuntimeStream).Process;
        }
       public static int MoveToOffsets(this ByteReader reader,int baseAddr, int[] offsets)
       {
           reader.Seek(baseAddr);
           return 0;
       }
    }
}