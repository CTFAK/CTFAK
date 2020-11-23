using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.utils
{
    public static class ImageHelper
    {
        public static Color ReadPoint(byte[] data,int position)
        {
            //byte b2 = binaryReader.ReadByte();
            //byte b3 = binaryReader.ReadByte();
            //byte b4 = binaryReader.ReadByte();
            byte b2 = data[position];
            byte b3 = data[position+1];
            byte b4 = data[position+2];
            return Color.FromArgb((int)b4, (int)b3, (int)b2);

        }
        public static Color ReadSixteen(byte[] data, int position)
        {
            var newShort = (data[position] | data[position + 1] << 8);
            byte r = (byte)((newShort & 31744) >> 10);
            byte g = (byte)((newShort & 992) >> 5);
            byte b = (byte)((newShort & 31));
            return Color.FromArgb((int)b, (int)g, (int)r);

        }
        public static int getPadding(int width, int pad = 2)
        {
            int num = pad - width * 3 % pad;
            if (num == pad)
            {
                num = 0;
            }
            return (int)Math.Ceiling((double)((float)num / 3f));
        }
    }
}
