using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;

namespace NetMFAPatcher.utils
{
    public static class ImageHelper
    {
        public static (byte[], int) ReadPoint(byte[] data, int width, int height)
        {
            byte[] colorArray = new byte[width * height * 4];
            int stride = width * 4;
            int pad = GetPadding(width, 3);
            int position = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorArray[(y * stride) + (x * 4) + 0] = data[position];
                    colorArray[(y * stride) + (x * 4) + 1] = data[position + 1];
                    colorArray[(y * stride) + (x * 4) + 2] = data[position + 2];
                    colorArray[(y * stride) + (x * 4) + 3] = 255;
                    position += 3;
                }

                position += pad * 3;
            }

            return (colorArray, position);
        }

        public static (byte[], int) ReadSixteen(byte[] data, int width, int height)
        {
            byte[] colorArray = new byte[width * height * 4];
            int stride = width * 4;
            int pad = GetPadding(width, 3);
            int position = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    UInt16 newShort = (ushort) (data[position] | data[position + 1] << 8);
                    byte r = (byte) ((newShort & 31744) >> 10);
                    byte g = (byte) ((newShort & 992) >> 5);
                    byte b = (byte) ((newShort & 31));

                    colorArray[(y * stride) + (x * 4) + 0] = (byte) (r << 3);
                    colorArray[(y * stride) + (x * 4) + 1] = (byte) (g << 2);
                    colorArray[(y * stride) + (x * 4) + 2] = (byte) (b << 3);
                    colorArray[(y * stride) + (x * 4) + 3] = 255;
                    position += 2;
                }

                position += pad * 2;
            }

            return (colorArray, position);
        }

        public static (byte[], int) ReadFifteen(byte[] data, int width, int height)
        {
            byte[] colorArray = new byte[width * height * 4];
            int stride = width * 4;
            int pad = GetPadding(width, 3);
            int position = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    UInt16 newShort = (ushort) (data[position] | data[position + 1] << 8);
                    byte r = (byte) ((newShort & 31744) >> 10);
                    byte g = (byte) ((newShort & 992) >> 5);
                    byte b = (byte) ((newShort & 31));

                    colorArray[(y * stride) + (x * 4) + 0] = (byte) (r << 3);
                    colorArray[(y * stride) + (x * 4) + 1] = (byte) (g << 3);
                    colorArray[(y * stride) + (x * 4) + 2] = (byte) (b << 3);
                    colorArray[(y * stride) + (x * 4) + 3] = 255;
                    position += 2;
                }

                position += pad * 2;
            }

            return (colorArray, position);
        }

        public static byte[,] ReadAlpha(byte[] data, int width, int height, int possition)
        {
            int pad = GetPadding(width, 1, 4);
            byte[,] alpha = new byte[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    alpha[j, i] = data[possition];
                    possition += 1;
                }

                possition += pad;
            }

            return alpha;
        }

        public static int GetPadding(int width, int pointSize, int bytes = 2)
        {
            int pad = bytes - ((width * pointSize) % bytes);
            if (pad == bytes)
            {
                return 0;
            }

            return (int) Math.Ceiling((double) ((float) pad / (float) pointSize));
        }
    }
}