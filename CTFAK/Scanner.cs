using NetMFAPatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Scanner
{
    static readonly int[] Empty = new int[0];
    private static byte[] GetBytes(string value)
    {
        var myStr = value;
        
        return myStr.Split(' ').Select(s => byte.Parse(s, System.Globalization.NumberStyles.HexNumber)).ToArray();
    }
    public static int[] Locate(this byte[] self,string candidate)
    {
        var actualBytes = GetBytes(candidate);//Encoding.ASCII.GetBytes(candidate);
        return self.Locate(actualBytes);
    }

    public static int[] Locate(this byte[] self, byte[] candidate)
    {
        if (IsEmptyLocate(self, candidate))
            return Empty;

        var list = new List<int>();

        for (int i = 0; i < self.Length; i++)
        {
            if (!IsMatch(self, i, candidate))
                continue;

            list.Add(i);
        }

        return list.Count == 0 ? Empty : list.ToArray();
    }

    static bool IsMatch(byte[] array, int position, byte[] candidate)
    {
        if (candidate.Length > (array.Length - position))
            return false;

        for (int i = 0; i < candidate.Length; i++)
            if (array[position + i] != candidate[i])
                return false;

        return true;
    }

    static bool IsEmptyLocate(byte[] array, byte[] candidate)
    {
        return array == null
            || candidate == null
            || array.Length == 0
            || candidate.Length == 0
            || candidate.Length > array.Length;
    }

    
}
