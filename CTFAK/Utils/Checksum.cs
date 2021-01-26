using System;

namespace CTFAK.Utils
{
    public static class Checksum
    {
        public static int WrapSingleChar(int value)
        {
            value = value & 0xFF;
            if (value > 127)
            {
                value -= 256;
            }

            return value;
        }
        public static int MakeGroupChecksum(string pass, string name)
        {
            var groupWords = @"mqojhm:qskjhdsmkjsmkdjhq\x63clkcdhdlkjhd";
            var v4 = 57;
            foreach (char c in name)
            {
                v4 += c^0x7F;
            }

            var v5 = 0;
            foreach (char c in pass)
            {
                v4 += WrapSingleChar(groupWords[v5] + (c & 0xC3)) ^ 0xF3;
                v5+=1;
                if (v5 > groupWords.Length) v5 = 0;
            }
            return v4;
            
        }
        
    }
}